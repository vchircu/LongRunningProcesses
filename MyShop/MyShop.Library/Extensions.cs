using NServiceBus.MessageRouting.RoutingSlips;

namespace MyShop.Library
{
    public static class Extensions
    {
        public static void RouteToLastStep(this RoutingSlip slip)
        {
            while (slip.Itinerary.Count > 2) slip.RecordStep();
        }
    }
}