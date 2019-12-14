using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.Calendar;
using Toggl.Droid.Adapters.Calendar;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.ViewHolders
{
    public class CalendarEntryViewHolder : BaseRecyclerViewHolder<CalendarItem>
    {
        private TextView label;
        private GradientDrawable secondBackgroundLayer;

        private readonly int regularEntryTextSize = 12;
        private readonly int shortEntryTextSize = 10;
        private float defaultElevation;
        private float lastAnchoredDataHeight;
        private float shortTimeEntryHeight;
        private float regularEntryVerticalPadding;
        private float regularEntryHorizontalPadding;
        private float shortEntryVerticalPadding;
        private float shortEntryHorizontalPadding;

        private bool isInEditMode;

        public CalendarEntryViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CalendarEntryViewHolder(View itemView) : base(itemView)
        {
        }

        protected override void InitializeViews()
        {
            //This view holder layout background is a layer-list that contains one shape with the id: CalendarEntryBackgroundToOverride
            var layerDrawable = ItemView.Background as LayerDrawable;
            secondBackgroundLayer = layerDrawable?.FindDrawableByLayerId(Resource.Id.CalendarEntryBackgroundToOverride) as GradientDrawable;
            label = ItemView.FindViewById<TextView>(Resource.Id.EntryLabel);
            defaultElevation = ItemView.Elevation;
            shortTimeEntryHeight = 18.DpToPixels(ItemView.Context);
            regularEntryVerticalPadding = 2.DpToPixels(ItemView.Context);
            regularEntryHorizontalPadding = 4.DpToPixels(ItemView.Context);
            shortEntryVerticalPadding = 0.5f.DpToPixels(ItemView.Context);
            shortEntryHorizontalPadding = 2.DpToPixels(ItemView.Context);
        }

        protected override void UpdateView()
        {
            adjustTextSizeAndPaddings();
            updateBackground();
            updateTextColor();
            label.Text = Item.Description;
        }

        public void UpdateAnchoringInfo(AnchorData anchorData)
        {
            var layoutParams = ItemView.LayoutParameters;
            layoutParams.Width = anchorData.Width;
            layoutParams.Height = anchorData.Height;
            ItemView.LayoutParameters = layoutParams;
            lastAnchoredDataHeight = anchorData.Height;
        }

        public void SetIsInEditMode(bool editModeEnabled)
        {
            isInEditMode = editModeEnabled;
            ItemView.Elevation = isInEditMode
                ? defaultElevation + 4.DpToPixels(ItemView.Context)
                : defaultElevation;
        }

        private void adjustTextSizeAndPaddings()
        {
            var descriptionFontSize = calculateFontSizeForItemDuration();
            var verticalPaddingForItemDuration = calculateVerticalPaddingForItemDuration();
            var verticalPadding = (int)Math.Min((lastAnchoredDataHeight - descriptionFontSize.DpToPixels(ItemView.Context)) / 2f, verticalPaddingForItemDuration);
            var horizontalPaddingForItemDuration = calculateHorizontalPaddingForItemDuration();
            ItemView.SetPadding(horizontalPaddingForItemDuration, verticalPadding, horizontalPaddingForItemDuration, verticalPadding);
            label.SetTextSize(ComplexUnitType.Dip, descriptionFontSize);
        }

        private void updateBackground()
        {
            var color = Color.ParseColor(Item.Color);
            if (Item.Source == CalendarItemSource.Calendar)
                color.A = (byte)(color.A * 0.25);

            secondBackgroundLayer?.SetColor(color);
        }

        private void updateTextColor()
        {
            var color = Item.Source == CalendarItemSource.Calendar ? Color.ParseColor(Item.Color) : Color.White;
            label.SetTextColor(color);
        }

        private int calculateFontSizeForItemDuration() =>
            isEntryShort() ? shortEntryTextSize : regularEntryTextSize;

        private int calculateVerticalPaddingForItemDuration()
            => (int)(isEntryShort() ? shortEntryVerticalPadding : regularEntryVerticalPadding);

        private int calculateHorizontalPaddingForItemDuration()
            => (int)(isEntryShort() ? shortEntryHorizontalPadding : regularEntryHorizontalPadding);

        private bool isEntryShort()
            => lastAnchoredDataHeight <= shortTimeEntryHeight;
    }
}
