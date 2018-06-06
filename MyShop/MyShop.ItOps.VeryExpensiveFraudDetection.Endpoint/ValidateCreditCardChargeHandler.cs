namespace MyShop.ItOps.VeryExpensiveFraudDetection.Endpoint
{
    using System;
    using System.Threading.Tasks;

    using MyShop.ItOps.Messages;
    using MyShop.Library;

    using NServiceBus;
    using NServiceBus.Logging;
    using NServiceBus.MessageRouting.RoutingSlips;

    public class ValidateCreditCardChargeHandler : IHandleMessages<ValidateCreditCardCharge>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ValidateCreditCardChargeHandler));

        public Task Handle(ValidateCreditCardCharge message, IMessageHandlerContext context)
        {
            Log.Info($"Validating Credit Card Charge with Id {message.CorrelationId} is very expensive!");

            if (!IsValid(message))
            {
                var routingSlip = context.Extensions.Get<RoutingSlip>();
                routingSlip.Attachments["VeryExpensiveFraudDetection.ValidationError"] =
                    "VeryExpensiveFraudDetection found an issue with the charge";
                routingSlip.RouteToLastStep();
            }

            return Task.CompletedTask;
        }

        private bool IsValid(ValidateCreditCardCharge message)
        {
            if (GlobalConfig.CreditCardValidationAlwaysSucceeds)
            {
                return true;
            }

            return new Random().Next(10) > 5;
        }
    }
}