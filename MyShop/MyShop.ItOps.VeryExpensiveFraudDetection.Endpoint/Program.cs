using System;
using System.Threading.Tasks;
using MyShop.Library;
using NServiceBus;
using NServiceBus.MessageRouting.RoutingSlips;

namespace MyShop.ItOps.VeryExpensiveFraudDetection.Endpoint
{
    internal class Program
    {
        internal static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        internal static async Task MainAsync()
        {
            const string EndpointName = "ItOps.VeryExpensiveFraudDetection.Endpoint";
            Console.Title = EndpointName;
            var endpointConfiguration = new EndpointConfiguration(EndpointName);

            endpointConfiguration.UseTransport<MsmqTransport>();
            endpointConfiguration.EnableFeature<RoutingSlips>();

            endpointConfiguration.ApplyDefaults();

            var endpoint = await NServiceBus.Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}