using System;
using System.Threading.Tasks;
using MyShop.Finance.Messages;
using MyShop.Inventory.Messages;
using MyShop.ItOps.Messages;
using MyShop.Library;
using NServiceBus;

namespace MyShop.Shipping.Endpoint
{
    internal class Program
    {
        internal static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            const string EndpointName = "Shipping.Endpoint";
            Console.Title = EndpointName;
            var endpointConfiguration = new EndpointConfiguration(EndpointName);

            var transport = endpointConfiguration.UseTransport<MsmqTransport>();

            endpointConfiguration.ApplyDefaults();

            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(ShipWithFanCourierRequest), "ItOps.FanCourier.Gateway");
            routing.RouteToEndpoint(typeof(CancelFanCourierShipping), "ItOps.FanCourier.Gateway");
            routing.RouteToEndpoint(typeof(ShipWithUrgentCargusRequest), "ItOps.UrgentCargus.Gateway");

            routing.RegisterPublisher(typeof(IOrderCharged), "Finance.Endpoint");
            routing.RegisterPublisher(typeof(IOrderPacked), "Inventory.Endpoint");

            var recoverability = endpointConfiguration.Recoverability();
            recoverability.AddUnrecoverableException<CannotShipOrderException>();
            var endpoint = await NServiceBus.Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}