namespace Toggl.Core.UI.Views
{
    public interface IView : IDialogProviderView, IPermissionRequester, IGoogleTokenProvider
    {
        void Close();
    }
}
