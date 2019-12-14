using System.Threading.Tasks;

namespace Toggl.iOS.ViewControllers
{
    public interface IReactiveViewController
    {
        Task<bool> DismissFromNavigationController();
        void ViewcontrollerWasPopped();
    }
}
