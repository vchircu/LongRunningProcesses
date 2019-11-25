using System.Threading.Tasks;
using NServiceBus.MessageMutator;

namespace MyShop.Library
{
    public class MutateOutgoingMessages :
        IMutateOutgoingMessages
    {
        public Task MutateOutgoing(MutateOutgoingMessageContext context)
        {
            context.TryGetIncomingHeaders(out var incomingHeaders);

            if (incomingHeaders == null) return Task.CompletedTask;

            var contextHeaderName = "MyShop.Audit";
            incomingHeaders.TryGetValue(contextHeaderName, out var incomingContextValue);
            if (!string.IsNullOrEmpty(incomingContextValue))
            {
                var headers = context.OutgoingHeaders;
                headers[contextHeaderName] = incomingContextValue;
            }

            return Task.CompletedTask;
        }
    }
}