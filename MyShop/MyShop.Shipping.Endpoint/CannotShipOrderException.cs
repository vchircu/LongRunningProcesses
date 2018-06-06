namespace MyShop.Shipping.Endpoint
{
    using System;

    public class CannotShipOrderException : Exception
    {
        public override string ToString()
        {
            return $"Couldn't ship order with Id {orderId}";
        }

        public CannotShipOrderException(Guid orderId)
        {
            this.orderId = orderId;
        }

        private Guid orderId;
    }
}