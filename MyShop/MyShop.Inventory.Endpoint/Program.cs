using System;
using System.Threading.Tasks;
using MyShop.Library;
using MyShop.Sales.Messages;
using NServiceBus;

namespace MyShop.Inventory.Endpoint
{
    internal class Program
    {
        internal static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            const string EndpointName = "Inventory.Endpoint";
            Console.Title = EndpointName;
            var endpointConfiguration = new EndpointConfiguration(EndpointName);

            var transport = endpointConfiguration.UseTransport<MsmqTransport>();
            var routing = transport.Routing();
            routing.RegisterPublisher(typeof(IOrderPlaced), "Sales.Endpoint");

            endpointConfiguration.ApplyDefaults();

            var endpoint = await NServiceBus.Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}