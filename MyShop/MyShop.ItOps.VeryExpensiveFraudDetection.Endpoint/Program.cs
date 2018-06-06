namespace MyShop.ItOps.VeryExpensiveFraudDetection.Endpoint
{
    using System;
    using System.Threading.Tasks;

    using NServiceBus;
    using NServiceBus.MessageRouting.RoutingSlips;

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

            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseTransport<LearningTransport>();
            endpointConfiguration.EnableFeature<RoutingSlips>();

            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}