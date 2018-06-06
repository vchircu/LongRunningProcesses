namespace MyShop.ItOps.Messages
{
    using System;

    using NServiceBus;

    public class ShipWithUrgentCargusRequest : IMessage
    {
        public Guid CorrelationId { get; set; }
    }
}