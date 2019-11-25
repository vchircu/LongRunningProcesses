using MyShop.Library;

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

            TransportExtensions<MsmqTransport> transport = endpointConfiguration.UseTransport<MsmqTransport>();
            RoutingSettings<MsmqTransport> routing = transport.Routing();
            routing.RouteToEndpoint(typeof(ChargeCreditCardRequest), "ItOps.CreditCardProcessor.Gateway");
            routing.RegisterPublisher(typeof(Sales.Messages.IOrderPlaced), "Sales.Endpoint");
            endpointConfiguration.EnableFeature<RoutingSlips>();

            endpointConfiguration.ApplyDefaults();

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