namespace MyShop.Finance.Endpoint.Domain
{
    public enum OrderStatus 
    {
        Pending,

        Paid,

        PaymentFailed,

        Validating,

        ValidationFailed
    }
}