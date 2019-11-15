namespace MyShop.Finance.Endpoint.Application
{
    using System.Threading.Tasks;

    using MyShop.Finance.Endpoint.Domain;
    using MyShop.Finance.Messages;
    using MyShop.ItOps.Messages;

    using NServiceBus;
    using NServiceBus.Logging;

    public class ChargeCreditCardResponseHandler : IHandleMessages<ChargeCreditCardResponse>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChargeCreditCardResponseHandler));

        private readonly IOrderRepository orders;

        public ChargeCreditCardResponseHandler(IOrderRepository orders)
        {
            this.orders = orders;
        }

        public async Task Handle(ChargeCreditCardResponse message, IMessageHandlerContext context)
        {
            var order = orders.GetById(message.CorrelationId);

            order.Status = message.CardHasBeenCharged ? OrderStatus.Paid : OrderStatus.PaymentFailed;

            Log.Info($"Payment Status for Order with Id {order.OrderId} is {order.Status}");

            orders.Save(order);

            if (order.Status == OrderStatus.Paid)
            {
                await context.Publish<IOrderCharged>(orderCharged => { orderCharged.OrderId = order.OrderId; });
            }
        }
    }
}