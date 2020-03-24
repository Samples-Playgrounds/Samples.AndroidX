namespace Toggl.Core
{
    public interface IPlatformInfo
    {
        Platform Platform { get; }

        string HelpUrl { get; }
        string Version { get; }
        string PhoneModel { get; }
        string BuildNumber { get; }
        string OperatingSystem { get; }
        string TimezoneIdentifier { get; }
        string StoreUrl { get; }
        string CurrentNativeLanguageCode { get; }
        
        ApplicationInstallLocation InstallLocation { get; }
    }

    public enum Platform
    {
        Daneel,
        Giskard
    }
}
