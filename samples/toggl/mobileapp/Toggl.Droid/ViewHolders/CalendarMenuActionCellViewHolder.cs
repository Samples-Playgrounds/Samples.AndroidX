using System;
using System.Collections.Generic;
using System.Reactive;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels.Calendar.ContextualMenu;
using Toggl.Droid.Extensions;
using Color = Android.Graphics.Color;

namespace Toggl.Droid.ViewHolders
{
    public class CalendarMenuActionCellViewHolder: BaseRecyclerViewHolder<CalendarMenuAction>
    {
        private FrameLayout iconCircleOverlay;
        private ImageView icon;
        private TextView title;
        private Color iconTint;
        
        public static CalendarMenuActionCellViewHolder Create(View itemView)
            => new CalendarMenuActionCellViewHolder(itemView);
        
        public CalendarMenuActionCellViewHolder(View itemView) : base(itemView)
        {
        }

        public CalendarMenuActionCellViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            icon = ItemView.FindViewById<ImageView>(Resource.Id.Icon);
            title = ItemView.FindViewById<TextView>(Resource.Id.Text);
            iconCircleOverlay = ItemView.FindViewById<FrameLayout>(Resource.Id.Circle);
            iconTint = ItemView.Context.SafeGetColor(Resource.Color.black40percent);
        }

        protected override void UpdateView()
        {
            title.Text = Item.Title;
            icon.SetImageResource(icons[Item.ActionKind]);
            var overlayColor = ItemView.Context.SafeGetColor(colors[Item.ActionKind]);
            iconCircleOverlay.BackgroundTintList = ColorStateList.ValueOf(overlayColor);
            icon.ImageTintList = ColorStateList.ValueOf(iconTint);
        }

        protected override void OnItemViewClick(object sender, EventArgs args)
        {
            Item.MenuItemAction.Execute(Unit.Default);
        }
        
        private static readonly Dictionary<CalendarMenuActionKind, int> icons = new Dictionary<CalendarMenuActionKind, int>
        {
            { CalendarMenuActionKind.Discard, Resource.Drawable.close },
            { CalendarMenuActionKind.Edit, Resource.Drawable.ic_edit },
            { CalendarMenuActionKind.Save, Resource.Drawable.ic_save },
            { CalendarMenuActionKind.Delete, Resource.Drawable.ic_delete },
            { CalendarMenuActionKind.Copy, Resource.Drawable.ic_copy },
            { CalendarMenuActionKind.Start, Resource.Drawable.ic_play },
            { CalendarMenuActionKind.Continue, Resource.Drawable.ic_play },
            { CalendarMenuActionKind.Stop, Resource.Drawable.ic_stop }
        };
        
        private static readonly Dictionary<CalendarMenuActionKind, int> colors = new Dictionary<CalendarMenuActionKind, int>
        {
            { CalendarMenuActionKind.Discard, Resource.Color.contextualMenuDiscardIconOverlay },
            { CalendarMenuActionKind.Edit, Resource.Color.contextualMenuEditIconOverlay },
            { CalendarMenuActionKind.Save, Resource.Color.contextualMenuSaveIconOverlay },
            { CalendarMenuActionKind.Delete, Resource.Color.contextualMenuDeleteIconOverlay },
            { CalendarMenuActionKind.Copy, Resource.Color.contextualMenuCopyIconOverlay },
            { CalendarMenuActionKind.Start, Resource.Color.contextualMenuStartIconOverlay },
            { CalendarMenuActionKind.Continue, Resource.Color.contextualMenuContinueIconOverlay },
            { CalendarMenuActionKind.Stop, Resource.Color.contextualMenuStopIconOverlay }
        };
    }
}