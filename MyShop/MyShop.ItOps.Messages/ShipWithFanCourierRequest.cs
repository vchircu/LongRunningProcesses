namespace MyShop.ItOps.Messages
{
    using System;

    using NServiceBus;

    public class ShipWithFanCourierRequest : IMessage
    {
        public Guid CorrelationId { get; set; }
    }
}