namespace MyShop.Finance.Endpoint.Domain
{
    using System;

    public interface IOrderRepository
    {
        void Save(Order order);

        Order GetById(Guid orderId);
    }
}