namespace MyShop.Shipping.Messages
{
    using System;

    using NServiceBus;

    public class ShipHighVolumeOrder : ICommand
    {
        public Guid OrderId { get; set; }
    }
}