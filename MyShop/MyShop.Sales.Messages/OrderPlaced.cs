namespace MyShop.Sales.Messages
{
    using System;

    using NServiceBus;

    public interface IOrderPlaced : IEvent
    {
        Guid OrderId { get; set; }

        decimal TotalValue { get; set; }
    }
}