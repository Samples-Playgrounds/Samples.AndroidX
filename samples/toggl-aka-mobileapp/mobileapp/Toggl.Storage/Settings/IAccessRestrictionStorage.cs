namespace Toggl.Storage.Settings
{
    public interface IAccessRestrictionStorage
    {
        void SetClientOutdated();
        void SetApiOutdated();
        void SetUnauthorizedAccess(string apiToken);
        void SetNoWorkspaceStateReached(bool hasNoWorkspace);
        void SetNoDefaultWorkspaceStateReached(bool hasNoDefaultWorkspace);
        bool IsClientOutdated();
        bool IsApiOutdated();
        bool IsUnauthorized(string apiToken);
        bool HasNoWorkspace();
        bool HasNoDefaultWorkspace();
    }
}
