namespace MyShop.ItOps.FanCourier.Gateway
{
    using System.Threading.Tasks;

    using MyShop.ItOps.Messages;

    using NServiceBus;
    using NServiceBus.Logging;

    public class CancelFanCourierShippingHandler : IHandleMessages<CancelFanCourierShipping>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ShipWithFanCourierRequestHandler));

        public Task Handle(CancelFanCourierShipping message, IMessageHandlerContext context)
        {
            Log.Info($"Cancelling Order with Id {message.CorrelationId}");

            return Task.CompletedTask;
        }
    }
}