namespace MyShop.Shipping.Endpoint
{
    using System;
    using System.Threading.Tasks;

    using MyShop.ItOps.Messages;
    using MyShop.Shipping.Messages;

    using NServiceBus;
    using NServiceBus.Logging;

    public class ShipOrderProcessManager : Saga<ShipOrderProcessData>,
                                          IAmStartedByMessages<ShipOrder>,
                                          IHandleMessages<ShipWithFanCourierResponse>,
                                          IHandleMessages<ShipWithUrgentCargusResponse>,
                                          IHandleTimeouts<DidNotReceiveAResponseFromFanCourierTimeout>,
                                          IHandleTimeouts<DidNotReceiveAResponseFromUrgentCargusTimeout>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ShipOrderProcessManager));

        private readonly TimeSpan shipmentSla = TimeSpan.FromSeconds(10);

        public Task Handle(ShipOrder message, IMessageHandlerContext context)
        {
            Log.Info($"Received ShipOrder command for Order with Id {message.OrderId}.");
            Log.Info($"Attempting to ship Order with Id {Data.OrderId} with FanCourier.");

            Data.OrderId = message.OrderId;
            Data.Status = ShippingStatus.ShippingWithFanCourier;

            context.Send(new ShipWithFanCourierRequest { CorrelationId = Data.OrderId });

            RequestTimeout(context, shipmentSla, new DidNotReceiveAResponseFromFanCourierTimeout());

            return Task.CompletedTask;
        }

        public Task Handle(ShipWithFanCourierResponse message, IMessageHandlerContext context)
        {
            if (!message.PackageShipped)
            {
                ShipWithUrgentCargus(context);
            }
            else
            {
                if (Data.Status == ShippingStatus.ShippingWithFanCourier)
                {
                    context.Publish<IOrderShipped>(m => { m.OrderId = Data.OrderId; });
                    MarkAsComplete();

                    Log.Info($"Done shipping Order with Id {Data.OrderId}.");
                }
                else
                {
                    Log.Info(
                        "Uh-oh. We received the response too late. "
                        + $"Cancel Fan Courier shipping command for Order with Id {Data.OrderId} has already been sent.");
                }
            }

            return Task.CompletedTask;
        }

        public Task Handle(ShipWithUrgentCargusResponse message, IMessageHandlerContext context)
        {
            if (message.PackageShipped)
            {
                context.Publish<IOrderShipped>(m => { m.OrderId = Data.OrderId; });
                MarkAsComplete();

                Log.Info($"Done shipping Order with Id {Data.OrderId}.");
            }
            else
            {
                throw new CannotShipOrderException(Data.OrderId);
            }

            return Task.CompletedTask;
        }

        public Task Timeout(DidNotReceiveAResponseFromFanCourierTimeout state, IMessageHandlerContext context)
        {
            if (Data.Status == ShippingStatus.ShippingWithFanCourier)
            {
                context.Send(new CancelFanCourierShipping { CorrelationId = Data.OrderId });
                ShipWithUrgentCargus(context);
            }

            return Task.CompletedTask;
        }

        public Task Timeout(DidNotReceiveAResponseFromUrgentCargusTimeout state, IMessageHandlerContext context)
        {
            throw new CannotShipOrderException(Data.OrderId);
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipOrderProcessData> mapper)
        {
            mapper.ConfigureMapping<ShipOrder>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
        }

        private void ShipWithUrgentCargus(IMessageHandlerContext context)
        {
            Log.Info($"Attempting to ship Order with Id {Data.OrderId} with UrgentCargus.");

            Data.Status = ShippingStatus.ShippingWithUrgentCargus;
            context.Send(new ShipWithUrgentCargusRequest { CorrelationId = Data.OrderId });

            RequestTimeout(context, shipmentSla, new DidNotReceiveAResponseFromUrgentCargusTimeout());
        }
    }

    public class ShipOrderProcessData : ContainSagaData
    {
        public Guid OrderId { get; set; }

        public ShippingStatus Status { get; set; }
    }

    public enum ShippingStatus
    {
        ShippingWithUrgentCargus,

        ShippingWithFanCourier
    }
}