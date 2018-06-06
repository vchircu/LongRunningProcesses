namespace MyShop.ItOps.Monitoring.Endpoint
{
    using System;
    using System.Threading.Tasks;

    using MyShop.Finance.Messages;
    using MyShop.Inventory.Messages;
    using MyShop.Sales.Messages;
    using MyShop.Shipping.Messages;

    using NServiceBus;
    using NServiceBus.Logging;

    public class Monitoring : Saga<MonitoringData>,
                              IAmStartedByMessages<IOrderPlaced>,
                              IAmStartedByMessages<IOrderCharged>,
                              IAmStartedByMessages<IOrderPacked>,
                              IAmStartedByMessages<IOrderShipped>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Monitoring));

        public bool CanComplete =>
            Data.IsOrderPlaced && Data.IsOrderCharged && Data.IsOrderPacked && Data.IsOrderShipped;

        public Task Handle(IOrderPlaced message, IMessageHandlerContext context)
        {
            Log.Info($"Received order placed event for Order with Id {message.OrderId}.");

            Data.OrderId = message.OrderId;
            Data.IsOrderPlaced = true;
            if (CanComplete)
            {
                Log.Info($"Complete Order with Id {message.OrderId}.");
                MarkAsComplete();
            }

            return Task.CompletedTask;
        }

        public Task Handle(IOrderCharged message, IMessageHandlerContext context)
        {
            Log.Info($"Received order charged event for Order with Id {message.OrderId}.");

            Data.OrderId = message.OrderId;
            Data.IsOrderCharged = true;
            if (CanComplete)
            {
                Log.Info($"Complete Order with Id {message.OrderId}.");
                MarkAsComplete();
            }

            return Task.CompletedTask;
        }

        public Task Handle(IOrderPacked message, IMessageHandlerContext context)
        {
            Log.Info($"Received order packed event for Order with Id {message.OrderId}.");

            Data.OrderId = message.OrderId;
            Data.IsOrderPacked = true;
            if (CanComplete)
            {
                Log.Info($"Complete Order with Id {message.OrderId}.");
                MarkAsComplete();
            }

            return Task.CompletedTask;
        }

        public Task Handle(IOrderShipped message, IMessageHandlerContext context)
        {
            Log.Info($"Received order shipped event for Order with Id {message.OrderId}.");

            Data.OrderId = message.OrderId;
            Data.IsOrderShipped = true;
            if (CanComplete)
            {
                Log.Info($"Complete Order with Id {message.OrderId}.");
                MarkAsComplete();
            }

            return Task.CompletedTask;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MonitoringData> mapper)
        {
            mapper.ConfigureMapping<IOrderPlaced>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
            mapper.ConfigureMapping<IOrderCharged>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
            mapper.ConfigureMapping<IOrderPacked>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
            mapper.ConfigureMapping<IOrderShipped>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
        }
    }

    public class MonitoringData : ContainSagaData
    {
        public Guid OrderId { get; set; }

        public bool IsOrderPlaced { get; set; }

        public bool IsOrderCharged { get; set; }

        public bool IsOrderShipped { get; set; }

        public bool IsOrderPacked { get; set; }
    }
}