namespace MyShop.ItOps.CreditCardProcessor.Gateway
{
    using System.Threading.Tasks;

    using MyShop.ItOps.Messages;

    using NServiceBus;
    using NServiceBus.Logging;

    public class ChargeCreditCardRequestHandler : IHandleMessages<ChargeCreditCardRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChargeCreditCardRequestHandler));

        public Task Handle(ChargeCreditCardRequest message, IMessageHandlerContext context)
        {
            Log.Info($"Charging for Order with Id { message.CorrelationId }");
            return context.Reply(new ChargeCreditCardResponse { CorrelationId = message.CorrelationId, CardHasBeenCharged = true });
        }
    }
}