namespace Toggl.Core.Analytics
{
    public enum LoginErrorSource
    {
        InvalidEmailOrPassword,
        GoogleLoginError,
        Offline,
        ServerError,
        MissingApiToken,
        Other
    }
}
