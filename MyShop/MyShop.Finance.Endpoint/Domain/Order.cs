namespace MyShop.Finance.Endpoint.Domain
{
    using System;

    public class Order
    {
        public decimal TotalValue { get; }

        public OrderStatus Status { get; set; }

        public Order(Guid orderId, decimal totalValue)
        {
            this.OrderId = orderId;
            this.TotalValue = totalValue;
        }

        public Guid OrderId { get; }
    }
}