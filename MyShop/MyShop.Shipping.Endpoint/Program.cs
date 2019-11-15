namespace MyShop.Shipping.Endpoint
{
    using System;
    using System.Threading.Tasks;

    using MyShop.ItOps.Messages;

    using NServiceBus;

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

            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            var transport = endpointConfiguration.UseTransport<MsmqTransport>();
           
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.EnableInstallers();

            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(ShipWithFanCourierRequest), "ItOps.FanCourier.Gateway");
            routing.RouteToEndpoint(typeof(CancelFanCourierShipping), "ItOps.FanCourier.Gateway");
            routing.RouteToEndpoint(typeof(ShipWithUrgentCargusRequest), "ItOps.UrgentCargus.Gateway");

            routing.RegisterPublisher(typeof(Finance.Messages.IOrderCharged), "Finance.Endpoint");
            routing.RegisterPublisher(typeof(Inventory.Messages.IOrderPacked), "Inventory.Endpoint");

            var recoverability = endpointConfiguration.Recoverability();
            recoverability.AddUnrecoverableException<CannotShipOrderException>();
            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            await endpoint.Stop().ConfigureAwait(false);
        }
    }
}