namespace MyShop.Finance.Messages
{
    using System;

    using NServiceBus;

    public interface IOrderCharged : IEvent
    {
        Guid OrderId { get; set; }
    }
}