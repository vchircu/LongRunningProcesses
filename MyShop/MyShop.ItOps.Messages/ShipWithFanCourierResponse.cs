namespace MyShop.ItOps.Messages
{
    using System;

    using NServiceBus;

    public class ShipWithFanCourierResponse : IMessage
    {
        public Guid CorrelationId { get; set; }

        public bool PackageShipped { get; set; }
    }
}