namespace MyShop.ItOps.Messages
{
    using System;

    using NServiceBus;
    public class ChargeCreditCardResponse : IMessage
    {
        public Guid CorrelationId { get; set; }

        public bool CardHasBeenCharged { get; set; }
    }
}