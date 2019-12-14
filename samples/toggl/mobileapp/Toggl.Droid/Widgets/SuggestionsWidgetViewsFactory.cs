using Android.Content;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Droid.Extensions;
using static Toggl.Droid.Widgets.WidgetsConstants;
using Color = Android.Graphics.Color;

namespace Toggl.Droid.Widgets
{
    public sealed class SuggestionsWidgetViewsFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        private Context context;
        private List<WidgetSuggestionItem> items = new List<WidgetSuggestionItem>();


        public SuggestionsWidgetViewsFactory(Context context)
        {
            this.context = context;
        }

        public void OnCreate()
        {
            updateData();
        }

        public void OnDataSetChanged()
        {
            updateData();
        }

        public int Count
            => items?.Count ?? 0;

        public bool HasStableIds
            => false;

        public RemoteViews GetViewAt(int position)
        {
            var view = new RemoteViews(context.PackageName, Resource.Layout.SuggestionsWidgetItem);

            var item = items[position];
            var hasDescription = !string.IsNullOrEmpty(item.Description);
            var hasProject = !string.IsNullOrEmpty(item.ProjectName);

            view.SetViewVisibility(Resource.Id.DescriptionTextView, hasDescription.ToVisibility());
            if (hasDescription)
                view.SetTextViewText(Resource.Id.DescriptionTextView, item.Description);

            view.SetViewVisibility(Resource.Id.ProjectClientRow, hasProject.ToVisibility());

            if (hasProject)
            {
                view.SetTextViewText(Resource.Id.ProjectNameTextView, item.ProjectName);
                view.SetTextColor(Resource.Id.ProjectNameTextView, Color.ParseColor(item.ProjectColor));
                view.SetTextViewText(Resource.Id.ClientNameTextView, item.ClientName);
            }

            var bottomBorderVisibility = (position != Count - 1).ToVisibility();
            view.SetViewVisibility(Resource.Id.BottomSeparator, bottomBorderVisibility);

            var intent = new Intent();
            intent.PutExtra(TappedSuggestionIndex, position);
            view.SetOnClickFillInIntent(Resource.Id.RootLayout, intent);

            return view;
        }

        // We are using a default loading view
        public RemoteViews LoadingView => null;

        // This requires only a single view type, so we're using number 1 to reference that type.
        public int ViewTypeCount => 1;

        public long GetItemId(int position) => position;

        private void updateData()
        {
            items.Clear();
            items.AddRange(WidgetSuggestionItem.SuggestionsFromSharedPreferences());
        }

        public void OnDestroy()
        {
            items.Clear();
        }
    }
}
