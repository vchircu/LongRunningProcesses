namespace MyShop.ItOps.FanCourier.Gateway
{
    using System;
    using System.Threading.Tasks;

    using MyShop.ItOps.Messages;
    using MyShop.Library;

    using NServiceBus;
    using NServiceBus.Logging;

    public class ShipWithFanCourierRequestHandler : IHandleMessages<ShipWithFanCourierRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ShipWithFanCourierRequestHandler));

        private static readonly Random Random = new Random();

        public Task Handle(ShipWithFanCourierRequest message, IMessageHandlerContext context)
        {
            Log.Info($"Attempting to ship Order with Id {message.CorrelationId}");

            return context.Reply(
                new ShipWithFanCourierResponse
                    {
                        CorrelationId = message.CorrelationId,
                        PackageShipped = IsPackageShipped()
                    });
        }

        private static bool IsPackageShipped()
        {
            switch (GlobalConfig.FanCourierResponse)
            {
                case FanCourierResponse.CannotShip:
                    return false;
                case FanCourierResponse.PackageShipped:
                    return true;
                case FanCourierResponse.EndpointUnreliable:
                    if (Random.Next(10) > 5)
                    {
                        throw new InvalidOperationException("Endpoint down!");
                    }

                    return true;
                default:
                    return Random.Next(10) > 3;
            }
        }
    }
}