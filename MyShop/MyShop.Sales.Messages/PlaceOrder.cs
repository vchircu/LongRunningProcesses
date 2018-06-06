namespace MyShop.Sales.Messages
{
    using System;

    using NServiceBus;

    public class PlaceOrder : ICommand
    {
        public Guid OrderId { get; set; }

        public decimal TotalValue { get; set; }
    }
}