using System;
using System.Threading.Tasks;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using Colors = Toggl.Core.UI.Helper.Colors;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    public sealed partial class CalendarSettingsViewController : ReactiveViewController<CalendarSettingsViewModel>
    {
        private const int tableViewHeaderHeight = 106;

        public CalendarSettingsViewController(CalendarSettingsViewModel viewModel)
            : base(viewModel, nameof(CalendarSettingsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.Title = Resources.CalendarSettingsTitle;

            var header = CalendarSettingsTableViewHeader.Create();
            UserCalendarsTableView.TableHeaderView = header;
            UserCalendarsTableView.AllowsSelection = false;
            header.TranslatesAutoresizingMaskIntoConstraints = false;
            header.HeightAnchor.ConstraintEqualTo(tableViewHeaderHeight).Active = true;
            header.WidthAnchor.ConstraintEqualTo(UserCalendarsTableView.WidthAnchor).Active = true;

            var source = new SelectUserCalendarsTableViewSource(UserCalendarsTableView, ViewModel.SelectCalendar);
            UserCalendarsTableView.Source = source;

            ViewModel.Calendars
                .Subscribe(UserCalendarsTableView.Rx().ReloadSections(source))
                .DisposedBy(DisposeBag);

            ViewModel.PermissionGranted
                .Subscribe(header.SetCalendarPermissionStatus)
                .DisposedBy(DisposeBag);

            header.EnableCalendarAccessTapped
                .Subscribe(ViewModel.RequestAccess.Inputs)
                .DisposedBy(DisposeBag);

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectCalendar.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.RequestCalendarPermissionsIfNeeded.Execute();
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (parent == null)
            {
                ViewModel.Save.Execute();
            }
        }

        public override Task<bool> DismissFromNavigationController()
            => Task.FromResult(true);
    }
}
