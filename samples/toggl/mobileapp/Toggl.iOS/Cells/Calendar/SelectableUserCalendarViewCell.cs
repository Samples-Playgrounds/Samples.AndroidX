using Foundation;
using System;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Cells.Calendar
{
    public sealed partial class SelectableUserCalendarViewCell : BaseTableViewCell<SelectableUserCalendarViewModel>
    {
        public static readonly string Identifier = nameof(SelectableUserCalendarViewCell);
        public static readonly NSString Key = new NSString(nameof(SelectableUserCalendarViewCell));
        public static readonly UINib Nib;

        public InputAction<SelectableUserCalendarViewModel> SelectCalendar { get; set; }

        static SelectableUserCalendarViewCell()
        {
            Nib = UINib.FromName(nameof(SelectableUserCalendarViewCell), NSBundle.MainBundle);
        }

        protected SelectableUserCalendarViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            IsSelectedSwitch.Rx()
                .BindAction(() => SelectCalendar?.Execute(Item));

            FadeView.FadeRight = true;
            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            CalendarNameLabel.Text = Item.Name;
            IsSelectedSwitch.SetState(Item.Selected, animated: false);
        }
    }
}
