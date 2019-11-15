namespace MyShop.ItOps.UrgentCargus.Gateway
{
    using System.Threading.Tasks;

    using MyShop.ItOps.Messages;

    using NServiceBus;
    using NServiceBus.Logging;

    public class ShipWithUrgentCargusRequestHandler : IHandleMessages<ShipWithUrgentCargusRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ShipWithUrgentCargusRequestHandler));

        public Task Handle(ShipWithUrgentCargusRequest message, IMessageHandlerContext context)
        {
            Log.Info($"Attempting to ship Order with Id {message.CorrelationId}");

            return context.Reply(
                new ShipWithUrgentCargusResponse { CorrelationId = message.CorrelationId, PackageShipped = true });
        }
    }
}