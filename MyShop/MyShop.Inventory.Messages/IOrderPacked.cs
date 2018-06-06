namespace MyShop.Inventory.Messages
{
    using System;

    using NServiceBus;

    public interface IOrderPacked : IEvent
    {
        Guid OrderId { get; set; }
    }
}