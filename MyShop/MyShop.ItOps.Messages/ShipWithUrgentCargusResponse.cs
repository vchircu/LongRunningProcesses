namespace MyShop.ItOps.Messages
{
    using System;

    using NServiceBus;

    public class ShipWithUrgentCargusResponse : IMessage
    {
        public bool PackageShipped { get; set; }

        public Guid CorrelationId { get; set; }
    }
}