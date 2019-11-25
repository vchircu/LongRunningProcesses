using NServiceBus;
using NServiceBus.MessageMutator;

namespace MyShop.Library
{
    public static class CustomEndpointConfigurationExtensions
    {
        public static void ApplyDefaults(this EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.SendFailedMessagesTo("error");
            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.RegisterMessageMutator(new MutateOutgoingMessages());
        }
    }
}