using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using CoreGraphics;
using Foundation;
using Toggl.Core.Sync;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Helper;
using Toggl.iOS.Presentation.Transition;
using Toggl.iOS.ViewControllers.Settings.Models;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    using SettingSection = SectionModel<string, ISettingRow>;

    public partial class SettingsViewController : ReactiveViewController<SettingsViewModel>
    {
        private readonly float bottomInset = 24;

        public SettingsViewController(SettingsViewModel viewModel)
            : base(viewModel, nameof(SettingsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = ColorAssets.TableBackground;
            ((ReactiveNavigationController)NavigationController).SetBackgroundColor(ColorAssets.TableBackground);

            NavigationItem.RightBarButtonItem = ReactiveNavigationController.CreateSystemItem(
                UIBarButtonSystemItem.Done,
                () => ViewModel.Close()
            );

            var source = new SettingsTableViewSource(tableView);
            tableView.Source = source;
            tableView.TableFooterView = new UIView(frame: new CGRect(0, 0, 0, bottomInset));
            tableView.BackgroundColor = ColorAssets.TableBackground;

            settingsSections()
                .Subscribe(tableView.Rx().ReloadSections(source))
                .DisposedBy(DisposeBag);

            source.Rx().ModelSelected()
                .Subscribe(handleSettingTap)
                .DisposedBy(DisposeBag);

            FeedbackToastTitleLabel.Text = Resources.DoneWithExclamationMark.ToUpper();
            FeedbackToastTextLabel.Text = Resources.ThankYouForTheFeedback;

            Title = ViewModel.Title;

            SendFeedbackSuccessView.Hidden = true;

            ViewModel.IsFeedbackSuccessViewShowing
                .Subscribe(SendFeedbackSuccessView.Rx().AnimatedIsVisible())
                .DisposedBy(DisposeBag);

            SendFeedbackSuccessView.Rx().Tap()
                .Subscribe(ViewModel.CloseFeedbackSuccessView)
                .DisposedBy(DisposeBag);
        }

        private void handleSettingTap(ISettingRow setting)
        {
            setting.Action.Execute();
        }

        private IObservable<IImmutableList<SettingSection>> settingsSections()
        {
            var sections = new List<IObservable<SettingSection>>();

            var profileSection = Observable.CombineLatest(ViewModel.Name, ViewModel.Email, ViewModel.WorkspaceName,
                (name, email, workspace)
                    => new SettingSection(Resources.YourProfile, new ISettingRow[]
                    {
                        new InfoRow(Resources.Name, name),
                        new InfoRow(Resources.EmailAddress, email),
                        new NavigationRow(Resources.Workspace, workspace, ViewModel.PickDefaultWorkspace)
                    }));

            sections.Add(profileSection);

            var dateTimeSection = Observable.CombineLatest(
                ViewModel.DateFormat,
                ViewModel.UseTwentyFourHourFormat,
                ViewModel.DurationFormat,
                ViewModel.BeginningOfWeek,
                ViewModel.IsGroupingTimeEntries,
                (dateFormat, useTwentyFourHourClock, durationFormat, firstDayOfWeek, groupTEs)
                    => new SettingSection(Resources.DateAndTime, new ISettingRow[]
                    {
                        new NavigationRow(Resources.DateFormat, dateFormat, ViewModel.SelectDateFormat),
                        new ToggleRow(Resources.Use24HourClock, useTwentyFourHourClock,
                            ViewModel.ToggleTwentyFourHourSettings),
                        new NavigationRow(Resources.DurationFormat, durationFormat, ViewModel.SelectDurationFormat),
                        new NavigationRow(Resources.FirstDayOfTheWeek, firstDayOfWeek, ViewModel.SelectBeginningOfWeek),
                        new ToggleRow(Resources.GroupTimeEntries, groupTEs, ViewModel.ToggleTimeEntriesGrouping)
                    }));

            sections.Add(dateTimeSection);

            var timerDefaultsSection = Observable.CombineLatest(ViewModel.IsManualModeEnabled, ViewModel.SwipeActionsEnabled,
                (isManualModeEnabled, areSwipeActionsEnabled)
                    => new SettingSection(Resources.TimerDefaults, new ISettingRow[]
                    {
                        new ToggleRow(Resources.SwipeActions, areSwipeActionsEnabled, ViewModel.ToggleSwipeActions),
                        new ToggleRow(Resources.ManualMode, isManualModeEnabled, ViewModel.ToggleManualMode),
                        new AnnotationRow(Resources.ManualModeDescription)
                    }));

            sections.Add(timerDefaultsSection);


            var calendarSection = new SettingSection(Resources.Calendar, new ISettingRow[]
            {
                new NavigationRow(Resources.CalendarSettingsTitle, ViewModel.OpenCalendarSettings),
                new NavigationRow(Resources.SmartReminders, ViewModel.OpenNotificationSettings),
            });

            sections.Add(Observable.Return(calendarSection));


            if (UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
            {
                var siriSection = new SettingSection(Resources.Siri, new ISettingRow[]
                {
                    new NavigationRow(Resources.SiriShortcuts, ViewModel.OpenSiriShortcuts)
                });

                sections.Add(Observable.Return(siriSection));
            }

            var generalSection = Observable.Return(
                new SettingSection(Resources.General, new ISettingRow[]
                {
                    new NavigationRow(Resources.SubmitFeedback, ViewModel.SubmitFeedback),
                    new NavigationRow(Resources.About, ViewModel.Version, ViewModel.OpenAboutView),
                    new NavigationRow(Resources.Help, ViewModel.OpenHelpView)
                }));

            sections.Add(generalSection);

            var footerSection = ViewModel.CurrentSyncStatus.Select(syncStatus
                => new SettingSection("", new ISettingRow[]
                {
                    new CustomRow<PresentableSyncStatus>(syncStatus),
                    new ButtonRow(Resources.SettingsDialogButtonSignOut, ViewModel.TryLogout)
                }));

            sections.Add(footerSection);

            return sections.CombineLatest().Select(list => list.ToImmutableList());
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            var activity = new NSUserActivity(Handoff.Action.Settings);
            activity.EligibleForHandoff = true;
            activity.WebPageUrl = Handoff.Url.Settings;
            UserActivity = activity;
            activity.BecomeCurrent();

# if DEBUG
            recognizer = new UILongPressGestureRecognizer(recognizer =>
            {
                if (recognizer.State != UIGestureRecognizerState.Began)
                    return;

                showErrorTriggeringView();
            });

            NavigationController.NavigationBar.AddGestureRecognizer(recognizer);
#endif
        }

#if DEBUG
        private UILongPressGestureRecognizer recognizer;

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (recognizer != null)
            {
                NavigationController.NavigationBar.RemoveGestureRecognizer(recognizer);
            }
        }

        private void showErrorTriggeringView()
        {
            PresentViewController(new Toggl.iOS.DebugHelpers.ErrorTriggeringViewController
            {
                ModalPresentationStyle = UIModalPresentationStyle.Custom,
                TransitioningDelegate = new ModalDialogTransitionDelegate()
            }, true, null);
        }
#endif
    }
}
