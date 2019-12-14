using Toggl.Core.Interactors;

namespace Toggl.Core.Shortcuts
{
    public interface IApplicationShortcutCreator
    {
        void OnLogin(IInteractorFactory interactorFactory);
        void OnLogout();
    }
}
