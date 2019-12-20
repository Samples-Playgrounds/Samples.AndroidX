using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Toggl.Shared;

namespace Toggl.Droid.Helper
{
    public static class PopupWindowFactory
    {
        public static PopupWindow PopupWindowWithText(Context context, int contentViewLayoutId, int tooltipTextViewId, string tooltipTextString)
        {
            Ensure.Argument.IsNotNull(context, nameof(context));
            Ensure.Argument.IsNotNull(tooltipTextViewId, nameof(tooltipTextViewId));
            Ensure.Argument.IsNotZero(tooltipTextViewId, nameof(tooltipTextViewId));
            Ensure.Argument.IsNotNullOrEmpty(tooltipTextString, nameof(tooltipTextString));

            var popupWindow = new PopupWindow(context);
            var popupWindowContentView = LayoutInflater.From(context).Inflate(contentViewLayoutId, null, false);
            var tooltipTextView = popupWindowContentView.FindViewById<TextView>(tooltipTextViewId);

            if (tooltipTextView == null)
            {
                throw new AndroidRuntimeException("The tooltipTextViewId must be present and must be a TextView");
            }

            tooltipTextView.Text = tooltipTextString;
            popupWindow.ContentView = popupWindowContentView;
            popupWindow.SetBackgroundDrawable(null);
            return popupWindow;
        }
    }
}
