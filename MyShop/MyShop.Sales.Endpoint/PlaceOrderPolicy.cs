namespace MyShop.Sales.Endpoint
{
    using System;
    using System.Threading.Tasks;

    using MyShop.Sales.Messages;

    using NServiceBus;
    using NServiceBus.Logging;

    public class PlaceOrderPolicy : Saga<PlaceOrderPolicyData>,
                                    IAmStartedByMessages<PlaceOrder>,
                                    IHandleMessages<CancelOrder>,
                                    IHandleTimeouts<BuyersRemorseTimeout>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PlaceOrderPolicy));

        public Task Handle(PlaceOrder message, IMessageHandlerContext context)
        {
            Data.OrderId = message.OrderId;
            Data.TotalValue = message.TotalValue;

            Log.Info($"Placing Order with Id {message.OrderId}");

            RequestTimeout(context, TimeSpan.FromSeconds(1), new BuyersRemorseTimeout());

            return Task.CompletedTask;
        }

        public Task Handle(CancelOrder message, IMessageHandlerContext context)
        {
            Log.Info($"Cancelling Order with Id {message.OrderId}");

            MarkAsComplete();
            return Task.CompletedTask;
        }

        public Task Timeout(BuyersRemorseTimeout state, IMessageHandlerContext context)
        {
            context.Publish<IOrderPlaced>(
                o =>
                    {
                        o.OrderId = Data.OrderId;
                        o.TotalValue = Data.TotalValue;
                    });

            MarkAsComplete();

            return Task.CompletedTask;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PlaceOrderPolicyData> mapper)
        {
            mapper.ConfigureMapping<PlaceOrder>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
            mapper.ConfigureMapping<CancelOrder>(message => message.OrderId).ToSaga(sagaData => sagaData.OrderId);
        }
    }

    public class PlaceOrderPolicyData : ContainSagaData
    {
        public Guid OrderId { get; set; }

        public decimal TotalValue { get; set; }
    }

    public class BuyersRemorseTimeout
    {
    }
}