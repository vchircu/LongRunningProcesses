namespace MyShop.ItOps.Messages
{
    using System;

    using NServiceBus;

    public class ValidateCreditCardCharge : ICommand
    {
        public Guid CorrelationId { get; set; }
        public decimal Amount { get; set; }
    }
}