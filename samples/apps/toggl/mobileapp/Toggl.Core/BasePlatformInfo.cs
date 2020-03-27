using System.Globalization;
using Xamarin.Essentials;

namespace Toggl.Core
{
    public abstract class BasePlatformInfo : IPlatformInfo
    {
        protected BasePlatformInfo(string helpUrl, string storeUrl, Platform platform)
        {
            HelpUrl = helpUrl;
            Platform = platform;
            StoreUrl = storeUrl;
        }

        public Platform Platform { get; }
        public string HelpUrl { get; }
        public string StoreUrl { get; }

        public virtual string CurrentNativeLanguageCode { get; } = "en"; 

        public virtual string TimezoneIdentifier { get; }

        public virtual string Version { get; } = AppInfo.VersionString;

        public virtual string BuildNumber { get; } = AppInfo.BuildString;

        public virtual string PhoneModel { get; } = DeviceInfo.Model;

        public virtual string OperatingSystem { get; } = $"{DeviceInfo.Platform} {DeviceInfo.VersionString}";

        public virtual ApplicationInstallLocation InstallLocation => ApplicationInstallLocation.Internal;
    }
}
