namespace Toggl.Storage.Onboarding
{
    public interface IDismissable
    {
        string Key { get; }
        void Dismiss();
    }
}
