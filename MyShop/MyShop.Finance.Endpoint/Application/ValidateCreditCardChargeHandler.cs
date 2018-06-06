namespace MyShop.Finance.Endpoint.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MyShop.Finance.Endpoint.Domain;
    using MyShop.ItOps.Messages;

    using NServiceBus;
    using NServiceBus.Logging;
    using NServiceBus.MessageRouting.RoutingSlips;

    public class ValidateCreditCardChargeHandler : IHandleMessages<ValidateCreditCardCharge>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ValidateCreditCardChargeHandler));

        private readonly IOrderRepository orders;

        public ValidateCreditCardChargeHandler(IOrderRepository orders)
        {
            this.orders = orders;
        }

        public Task Handle(ValidateCreditCardCharge message, IMessageHandlerContext context)
        {
            var routingSlip = context.Extensions.Get<RoutingSlip>();
            var order = orders.GetById(message.CorrelationId);

            if (routingSlip.Attachments.Any())
            {
                order.Status = OrderStatus.ValidationFailed;
                LogResponse(routingSlip.Attachments, order.OrderId);
            }
            else
            {
                order.Status = OrderStatus.Pending;
                context.Send(new ChargeCreditCardRequest { CorrelationId = order.OrderId, Amount = order.TotalValue });
            }

            orders.Save(order);

            return Task.CompletedTask;
        }

        private static void LogResponse(IDictionary<string, string> routingSlipAttachments, Guid orderId)
        {
            foreach (var value in routingSlipAttachments.Values)
            {
                Log.Info($"{value} with Id {orderId}");
            }
        }
    }
}