using System;
using System.Threading.Tasks;
using MyShop.Library;
using NServiceBus;

namespace MyShop.ItOps.CreditCardProcessor.Gateway
{
    internal class Program
    {
        internal static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            const string EndpointName = "ItOps.CreditCardProcessor.Gateway";
            Console.Title = EndpointName;
            var endpointConfiguration = new EndpointConfiguration(EndpointName);

            endpointConfiguration.UseTransport<MsmqTransport>();
            endpointConfiguration.ApplyDefaults();

            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}