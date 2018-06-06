namespace MyShop.Finance.Endpoint.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MyShop.Finance.Endpoint.Domain;

    public class InMemoryOrders : IOrderRepository
    {
        private readonly IList<Order> orders = new List<Order>();

        public void Save(Order order)
        {
            var storedOrder = GetById(order.OrderId);
            if (storedOrder != null)
            {
                orders.Remove(storedOrder);
            }
         
            orders.Add(order);
        }

        public Order GetById(Guid orderId)
        {
            return orders.SingleOrDefault(o => o.OrderId == orderId);
        }
    }
}