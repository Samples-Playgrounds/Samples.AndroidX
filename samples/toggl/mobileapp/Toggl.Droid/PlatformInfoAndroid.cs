using Android.App;
using Android.Content.PM;
using Java.Util;
using Toggl.Core;
using static Toggl.Core.Helper.Constants;

namespace Toggl.Droid
{
    public sealed class PlatformInfoAndroid : BasePlatformInfo
    {
        public override string TimezoneIdentifier => TimeZone.Default.ID;

        public PlatformInfoAndroid()
            : base("https://support.toggl.com/toggl-timer-for-android/",
                   "https://play.google.com/store/apps/details?id=com.toggl.giskard",
                    Platform.Giskard)
        {
        }

        public override ApplicationInstallLocation InstallLocation
        {
            get
            {
                try
                {
                    var context = Application.Context;
                    var packageInfo = context.PackageManager.GetPackageInfo(context.PackageName, 0);
                    return (packageInfo.ApplicationInfo.Flags & ApplicationInfoFlags.ExternalStorage) == ApplicationInfoFlags.ExternalStorage
                        ? ApplicationInstallLocation.External
                        : ApplicationInstallLocation.Internal;
                }
                catch
                {
                    return ApplicationInstallLocation.Unknown;
                }
            }
        }

        public override string CurrentNativeLanguageCode
            => Locale.Default?.ToString() ?? DefaultLanguageCode;
    }
}
