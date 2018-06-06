namespace MyShop.Finance.Endpoint
{
    using System;
    using System.Threading.Tasks;

    using MyShop.Finance.Endpoint.Infrastructure;
    using MyShop.ItOps.Messages;

    using NServiceBus;
    using NServiceBus.MessageRouting.RoutingSlips;

    internal class Program
    {
        internal static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            const string EndpointName = "Finance.Endpoint";
            Console.Title = EndpointName;
            var endpointConfiguration = new EndpointConfiguration(EndpointName);

            endpointConfiguration.UsePersistence<LearningPersistence>();
            TransportExtensions<LearningTransport> transport = endpointConfiguration.UseTransport<LearningTransport>();
            RoutingSettings<LearningTransport> routing = transport.Routing();
            routing.RouteToEndpoint(typeof(ChargeCreditCardRequest), "ItOps.CreditCardProcessor.Gateway");
            endpointConfiguration.EnableFeature<RoutingSlips>();

            endpointConfiguration.RegisterComponents(
                configureComponents =>
                    {
                        configureComponents.ConfigureComponent<InMemoryOrders>(DependencyLifecycle.SingleInstance);
                    });

            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}