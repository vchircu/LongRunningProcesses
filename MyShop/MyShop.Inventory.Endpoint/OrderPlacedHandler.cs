namespace MyShop.Inventory.Endpoint
{
    using System.Threading.Tasks;

    using MyShop.Inventory.Messages;
    using MyShop.Sales.Messages;

    using NServiceBus;

    public class OrderPlacedHandler : IHandleMessages<IOrderPlaced>
    {
        public Task Handle(IOrderPlaced message, IMessageHandlerContext context)
        {
            context.Publish<IOrderPacked>(m => { m.OrderId = message.OrderId; });

            return Task.CompletedTask;
        }
    }
}