using Android.OS;

namespace Toggl.Droid.Helper
{
    internal static class VibratorExtensions
    {
        public static void ActivateVibration(this Vibrator vibrator, int duration, int amplitude)
        {
            // We can't control the amplitude/intensity of the vibration before Oreo
            // It might give a bad experience on lower end phones
            if (!vibrator.HasVibrator || !OreoApis.AreAvailable)
                return;

            try
            {
                vibrator.Vibrate(VibrationEffect.CreateOneShot(duration, amplitude));
            }
            catch
            {
                // Ignore potential permission exceptions
            }
        }
    }
}
