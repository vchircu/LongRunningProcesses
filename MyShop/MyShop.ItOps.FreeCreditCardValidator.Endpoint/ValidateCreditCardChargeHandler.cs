namespace MyShop.ItOps.FreeCreditCardValidator.Endpoint
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
            Log.Info($"Validating Credit Card Charge with Id {message.CorrelationId} for free!");

            if (!IsValid(message))
            {
                var routingSlip = context.Extensions.Get<RoutingSlip>();
                routingSlip.Attachments["FreeCreditCardValidator.ValidationError"] =
                    "FreeCreditCardValidator found an issue with the charge";
                routingSlip.RouteToLastStep();
            }

            return Task.CompletedTask;
        }

        private static bool IsValid(ValidateCreditCardCharge message)
        {
            if (GlobalConfig.CreditCardValidationAlwaysSucceeds)
            {
                return true;
            }

            return new Random().Next(10) > 2;
        }
    }
}