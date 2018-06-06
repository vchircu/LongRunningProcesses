namespace MyShop.Shipping.Messages
{
    using System;

    using NServiceBus;

    public interface IOrderShipped : IEvent
    {
        Guid OrderId { get; set; }
    }
}