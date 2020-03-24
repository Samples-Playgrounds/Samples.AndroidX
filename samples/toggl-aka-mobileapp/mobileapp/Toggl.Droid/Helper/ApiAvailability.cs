using Android.OS;

namespace Toggl.Droid.Helper
{
    public static class MarshmallowApis
    {
        public static bool AreAvailable
            => Build.VERSION.SdkInt >= BuildVersionCodes.M;

        public static bool AreNotAvailable
            => !AreAvailable;
    }

    public static class NougatApis
    {
        public static bool AreNotAvailable
            => Build.VERSION.SdkInt < BuildVersionCodes.NMr1;
    }

    public static class OreoApis
    {
        public static bool AreAvailable
            => Build.VERSION.SdkInt >= BuildVersionCodes.O;
    }

    public static class PieApis
    {
        public static bool AreAvailable
            => Build.VERSION.SdkInt >= BuildVersionCodes.P;
    }

    public static class QApis
    {
        public static bool AreAvailable
            => Build.VERSION.SdkInt >= BuildVersionCodes.Q;
    }
}
