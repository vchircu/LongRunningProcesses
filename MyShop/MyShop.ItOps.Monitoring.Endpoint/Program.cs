using System;
using System.Threading.Tasks;
using MyShop.Finance.Messages;
using MyShop.Inventory.Messages;
using MyShop.Library;
using MyShop.Sales.Messages;
using MyShop.Shipping.Messages;
using NServiceBus;

namespace MyShop.ItOps.Monitoring.Endpoint
{
    internal class Program
    {
        internal static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            const string EndpointName = "ItOps.Monitoring.Endpoint";
            Console.Title = EndpointName;
            var endpointConfiguration = new EndpointConfiguration(EndpointName);

            var transport = endpointConfiguration.UseTransport<MsmqTransport>();
            var routing = transport.Routing();
            routing.RegisterPublisher(typeof(IOrderPlaced), "Sales.Endpoint");
            routing.RegisterPublisher(typeof(IOrderCharged), "Finance.Endpoint");
            routing.RegisterPublisher(typeof(IOrderPacked), "Inventory.Endpoint");
            routing.RegisterPublisher(typeof(IOrderShipped), "Shipping.Endpoint");

            endpointConfiguration.ApplyDefaults();

            var endpoint = await NServiceBus.Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}