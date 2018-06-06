namespace MyShop.Library
{
    public static class GlobalConfig
    {
        public static bool CreditCardValidationAlwaysSucceeds => true;

        public static FanCourierResponse FanCourierResponse => FanCourierResponse.Random;

        public static bool IsHighVolumeOrder => true;
    }

    public enum FanCourierResponse
    {
        CannotShip,
        PackageShipped,
        EndpointUnreliable,
        Random
    }
}