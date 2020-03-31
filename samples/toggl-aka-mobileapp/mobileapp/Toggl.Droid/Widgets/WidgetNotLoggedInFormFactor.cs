using Android.App;
using Android.Content;
using Android.Widget;
using System;
using System.Linq;
using Toggl.Droid.Extensions;
using static Android.App.PendingIntentFlags;
using static Android.Content.ActivityFlags;

namespace Toggl.Droid.Widgets
{
    public abstract class WidgetNotLoggedInFormFactor
    {
        private bool isFullView;

        protected abstract string LabelText { get; }

        public WidgetNotLoggedInFormFactor(int columnCount)
        {
            isFullView = columnCount > 3;
        }

        protected RemoteViews Setup(Context context)
        {
            var view = new RemoteViews(context.PackageName, Resource.Layout.WidgetNotLoggedIn);

            view.SetOnClickPendingIntent(Resource.Id.RootLayout, getOpenAppToLoginPendingIntent(context));

            view.SetTextViewText(Resource.Id.NotLoggedInLabel, LabelText);

            view.SetViewVisibility(Resource.Id.NotLoggedInLabel, isFullView.ToVisibility());
            view.SetViewVisibility(Resource.Id.TogglLogo, isFullView.ToVisibility());
            view.SetViewVisibility(Resource.Id.PadLock, isFullView.ToVisibility());
            view.SetViewVisibility(Resource.Id.PadLockBig, (!isFullView).ToVisibility());

            return view;
        }

        private PendingIntent getOpenAppToLoginPendingIntent(Context context)
        {
            var intent = new Intent(context, typeof(SplashScreen)).AddFlags(TaskOnHome);
            return PendingIntent.GetActivity(context, 0, intent, UpdateCurrent);
        }
    }
}
