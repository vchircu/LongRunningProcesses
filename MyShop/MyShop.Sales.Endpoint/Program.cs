namespace MyShop.Sales.Endpoint
{
    using System;
    using System.Threading.Tasks;

    using MyShop.Sales.Messages;

    using NServiceBus;

    internal class Program
    {
        internal static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            const string EndpointName = "Sales.Endpoint";
            Console.Title = EndpointName;
            var endpointConfiguration = new EndpointConfiguration(EndpointName);

            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseTransport<LearningTransport>();

            var endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);

            Console.WriteLine("1. Place an order with a value lower than 500");
            Console.WriteLine("2. Place an order with a value between 500 and 2000");
            Console.WriteLine("3. Place an order with a value greater than 2000");
            Console.WriteLine("4. Place an order then cancel it before buyer's remorse is over");
            Console.WriteLine("Press any other key to exit");

            var isDone = false;
            while (!isDone)
            {
                var key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        await SendOrderWithValue(endpoint, 200);
                        break;
                    case ConsoleKey.D2:
                        await SendOrderWithValue(endpoint, 1000);
                        break;
                    case ConsoleKey.D3:
                        await SendOrderWithValue(endpoint, 5000);
                        break;
                    case ConsoleKey.D4:
                        await SendOrderThanCancelIt(endpoint);
                        break;
                    default:
                        isDone = true;
                        break;
                }
            }

            await endpoint.Stop().ConfigureAwait(false);
        }

        private static async Task SendOrderThanCancelIt(IEndpointInstance endpoint)
        {
            var orderId = Guid.NewGuid();

            await endpoint.SendLocal(new PlaceOrder { OrderId = orderId, TotalValue = 900 });

            await endpoint.SendLocal(new CancelOrder { OrderId = orderId });
        }

        private static Task SendOrderWithValue(IEndpointInstance endpoint, decimal totalValue)
        {
            var orderId = Guid.NewGuid();

            return endpoint.SendLocal(new PlaceOrder { OrderId = orderId, TotalValue = totalValue });
        }
    }
}