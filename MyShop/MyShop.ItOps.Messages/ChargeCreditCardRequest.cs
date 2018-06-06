namespace MyShop.ItOps.Messages
{
    using System;

    using NServiceBus;

    public class ChargeCreditCardRequest : IMessage
    {
        public Guid CorrelationId { get; set; }

        public decimal Amount { get; set; }
    }
}