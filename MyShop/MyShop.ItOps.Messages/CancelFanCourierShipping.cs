namespace MyShop.ItOps.Messages
{
    using System;

    using NServiceBus;

    public class CancelFanCourierShipping : ICommand
    {
        public Guid CorrelationId { get; set; }
    }
}