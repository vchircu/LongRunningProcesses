namespace MyShop.Shipping.UnitTests
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using MyShop.ItOps.Messages;
    using MyShop.Shipping.Endpoint;
    using MyShop.Shipping.Messages;

    using NServiceBus.Testing;

    using Xunit;

    public class ShipOrderProcessManagerTests
    {
        private readonly ShipOrderProcessManager saga =
            new ShipOrderProcessManager { Data = new ShipOrderProcessData() };

        private readonly TestableMessageHandlerContext context = new TestableMessageHandlerContext();

        private readonly Guid orderId = Guid.NewGuid();

        [Fact]
        public async Task Should_Attempt_To_Ship_With_FanCourier_First()
        {
            await saga.Handle(new ShipOrder { OrderId = orderId }, context);

            context.SentMessages.Containing<ShipWithFanCourierRequest>().Should().HaveCount(1);
            context.TimeoutMessages.Should().HaveCount(1);
            context.TimeoutMessages[0].Message.Should().BeOfType<DidNotReceiveAResponseFromFanCourierTimeout>();
        }

        [Fact]
        public async Task Should_Publish_OrderShipped_If_Can_Ship_With_FanCourier()
        {
            await saga.Handle(new ShipOrder { OrderId = orderId }, context);
            await saga.Handle(
                new ShipWithFanCourierResponse { CorrelationId = orderId, PackageShipped = true },
                context);

            context.PublishedMessages.Containing<IOrderShipped>().Should().HaveCount(1);
        }

        [Fact]
        public async Task Should_Ship_With_UrgentCargus_If_Cannot_Ship_With_FanCourier()
        {
            await saga.Handle(new ShipOrder { OrderId = orderId }, context);
            await saga.Handle(
                new ShipWithFanCourierResponse { CorrelationId = orderId, PackageShipped = false },
                context);

            context.SentMessages.Containing<ShipWithUrgentCargusRequest>().Should().HaveCount(1);
            context.TimeoutMessages.Should().HaveCount(2);
            context.TimeoutMessages[1].Message.Should().BeOfType<DidNotReceiveAResponseFromUrgentCargusTimeout>();
        }

        [Fact]
        public async Task Should_Publish_OrderShipped_If_Can_Ship_With_UrgentCargus()
        {
            await saga.Handle(new ShipOrder { OrderId = orderId }, context);
            await saga.Handle(
                new ShipWithFanCourierResponse { CorrelationId = orderId, PackageShipped = false },
                context);
            await saga.Handle(
                new ShipWithUrgentCargusResponse { CorrelationId = orderId, PackageShipped = true },
                context);

            context.PublishedMessages.Containing<IOrderShipped>().Should().HaveCount(1);
        }

        [Fact]
        public async Task Should_Error_If_Cannot_Ship_With_UrgentCargus()
        {
            await saga.Handle(new ShipOrder { OrderId = orderId }, context);
            await saga.Handle(
                new ShipWithFanCourierResponse { CorrelationId = orderId, PackageShipped = false },
                context);

            Func<Task> action = async () =>
                {
                    await saga.Handle(
                        new ShipWithUrgentCargusResponse { CorrelationId = orderId, PackageShipped = false },
                        context);
                };

            action.Should().Throw<CannotShipOrderException>();
        }

        [Fact]
        public async Task
            Should_Ship_With_UrgentCargus_And_Cancel_FanCourier_Shipping_If_DidNotReceiveAResponseFromFanCourierTimeout()
        {
            await saga.Handle(new ShipOrder { OrderId = orderId }, context);
            await saga.Timeout(new DidNotReceiveAResponseFromFanCourierTimeout(), context);

            context.SentMessages.Containing<CancelFanCourierShipping>().Should().HaveCount(1);

            context.SentMessages.Containing<ShipWithUrgentCargusRequest>().Should().HaveCount(1);
            context.TimeoutMessages.Should().HaveCount(2);
            context.TimeoutMessages[1].Message.Should().BeOfType<DidNotReceiveAResponseFromUrgentCargusTimeout>();
        }

        [Fact]
        public async Task
            Should_Not_Update_ShippingStatus_If_Receiving_Positive_Response_From_FanCourier_After_DidNotReceiveAResponseFromFanCourierTimeout()
        {
            await saga.Handle(new ShipOrder { OrderId = orderId }, context);
            await saga.Timeout(new DidNotReceiveAResponseFromFanCourierTimeout(), context);
            await saga.Handle(
                new ShipWithFanCourierResponse { CorrelationId = orderId, PackageShipped = true },
                context);

            saga.Data.Status.Should().Be(ShippingStatus.ShippingWithUrgentCargus);
        }

        [Fact]
        public async Task Should_Error_If_DidNotReceiveAResponseFromUrgentCargusTimeout()
        {
            await saga.Handle(new ShipOrder { OrderId = orderId }, context);
            await saga.Handle(
                new ShipWithFanCourierResponse { CorrelationId = orderId, PackageShipped = false },
                context);

            Func<Task> action = async () =>
                {
                    await saga.Timeout(new DidNotReceiveAResponseFromUrgentCargusTimeout(), context);
                };

            action.Should().Throw<CannotShipOrderException>();
        }
    }
}