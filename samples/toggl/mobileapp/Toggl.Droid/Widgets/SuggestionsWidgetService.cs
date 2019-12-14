using Android.App;
using Android.Content;
using Android.Widget;

namespace Toggl.Droid.Widgets
{
    [Service(Permission = Android.Manifest.Permission.BindRemoteviews)]
    public sealed class SuggestionsWidgetService : RemoteViewsService
    {
        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
            => new SuggestionsWidgetViewsFactory(ApplicationContext);
    }
}
