using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using Android.App;
using static Android.Views.ViewStates;
using Toggl.Droid.Extensions;
using Android.Graphics;
using Color = Android.Graphics.Color;
using Toggl.Shared;

namespace Toggl.Droid.Widgets
{
    public sealed class TimeEntryWidgetDefaultFormFactor : TimeEntryWidgetStartStopButtonFormFactor
    {
        public override RemoteViews Setup(Context context, TimeEntryWidgetInfo widgetInfo)
        {
            var view = new RemoteViews(context.PackageName, Resource.Layout.TimeEntryWidget);

            SetupActionsForStartAndStopButtons(context, view);
            view.SetOnClickPendingIntent(Resource.Id.RootLayout, getOpenAppToLoginPendingIntent(context));

            if (widgetInfo.IsRunning)
            {
                view.SetViewVisibility(Resource.Id.StartButton, Gone);
                view.SetViewVisibility(Resource.Id.StopButton, Visible);

                var duration = (DateTimeOffset.Now - widgetInfo.StartTime).TotalMilliseconds;
                view.SetChronometer(Resource.Id.DurationTextView, SystemClock.ElapsedRealtime() - (long)duration, "%s", true);

                if (string.IsNullOrEmpty(widgetInfo.Description))
                {
                    view.SetTextViewText(Resource.Id.DescriptionTextView, Resources.NoDescription);
                    view.SetTextColor(Resource.Id.DescriptionTextView, context.SafeGetColor(Resource.Color.secondaryText));
                }
                else
                {
                    view.SetTextViewText(Resource.Id.DescriptionTextView, widgetInfo.Description);
                    view.SetTextColor(Resource.Id.DescriptionTextView, context.SafeGetColor(Resource.Color.primaryText));
                }

                view.SetViewVisibility(Resource.Id.DotView, widgetInfo.HasProject.ToVisibility());
                view.SetViewVisibility(Resource.Id.ProjectTextView, widgetInfo.HasProject.ToVisibility());
                if (widgetInfo.HasProject)
                {
                    // Project
                    var projectColor = widgetInfo.ProjectColor != null
                        ? Color.ParseColor(widgetInfo.ProjectColor)
                        : Color.Black;
                    view.SetInt(Resource.Id.DotView, "setBackgroundColor", projectColor);
                    view.SetTextViewText(Resource.Id.ProjectTextView, widgetInfo.ProjectName ?? "");
                    view.SetTextColor(Resource.Id.ProjectTextView, projectColor);

                    // Client
                    view.SetViewVisibility(Resource.Id.ClientTextView, widgetInfo.HasClient.ToVisibility());
                    if (widgetInfo.HasClient)
                        view.SetTextViewText(Resource.Id.ClientTextView, widgetInfo.ClientName);
                }
            }
            else
            {
                view.SetViewVisibility(Resource.Id.StartButton, Visible);
                view.SetViewVisibility(Resource.Id.StopButton, Gone);

                view.SetChronometer(Resource.Id.DurationTextView, SystemClock.ElapsedRealtime(), "%s", false);

                view.SetTextViewText(Resource.Id.DescriptionTextView, Resources.NoDescription);
                view.SetTextColor(Resource.Id.DescriptionTextView, context.SafeGetColor(Resource.Color.secondaryText));

                view.SetViewVisibility(Resource.Id.DotView, false.ToVisibility());
                view.SetViewVisibility(Resource.Id.ProjectTextView, false.ToVisibility());
                view.SetViewVisibility(Resource.Id.ClientTextView, false.ToVisibility());
            }

            return view;
        }

        private PendingIntent getOpenAppToLoginPendingIntent(Context context)
        {
            var intent = new Intent(context, typeof(SplashScreen)).AddFlags(ActivityFlags.TaskOnHome);
            return PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }
    }
}
