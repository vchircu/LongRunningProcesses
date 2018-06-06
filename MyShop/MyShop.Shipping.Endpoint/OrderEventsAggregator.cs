namespace MyShop.Shipping.Endpoint
{
    using System;
    using System.Threading.Tasks;

    using MyShop.Finance.Messages;
    using MyShop.Inventory.Messages;
    using MyShop.Library;
    using MyShop.Shipping.Messages;

    using NServiceBus;
    using NServiceBus.Logging;

    public class OrderEventsAggregator : Saga<OrderEventsAggregatorData>,
                                         IAmStartedByMessages<IOrderPacked>,
                                         IAmStartedByMessages<IOrderCharged>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OrderEventsAggregator));

        public bool CanShip => Data.IsOrderCharged && Data.IsOrderPacked;

        public Task Handle(IOrderPacked message, IMessageHandlerContext context)
        {
            Log.Info($"Received order packed event for Order with Id {message.OrderId}.");

            Data.OrderId = message.OrderId;
            Data.IsOrderPacked = true;

            if (CanShip)
            {
                ShipOrder(message.OrderId, context);
            }

            return Task.CompletedTask;
        }

        public Task Handle(IOrderCharged message, IMessageHandlerContext context)
        {
            Log.Info($"Received order charged event for Order with Id {message.OrderId}.");

            Data.OrderId = message.OrderId;
            Data.IsOrderCharged = true;

            if (CanShip)
            {
                ShipOrder(message.OrderId, context);
            }

            return Task.CompletedTask;
        }

        private void ShipOrder(Guid orderId, IMessageHandlerContext context)
        {
            if (GlobalConfig.IsHighVolumeOrder)
            {
                context.SendLocal(new ShipHighVolumeOrder { OrderId = orderId });
            }
            else
            {
                context.SendLocal(new ShipOrder { OrderId = orderId });
            }

            MarkAsComplete();
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderEventsAggregatorData> mapper)
        {
            mapper.ConfigureMapping<IOrderPacked>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
            mapper.ConfigureMapping<IOrderCharged>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
        }
    }

    public class OrderEventsAggregatorData : ContainSagaData
    {
        public Guid OrderId { get; set; }

        public bool IsOrderPacked { get; set; }

        public bool IsOrderCharged { get; set; }
    }
}