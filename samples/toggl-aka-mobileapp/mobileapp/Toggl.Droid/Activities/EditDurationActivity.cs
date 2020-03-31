using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Droid.ViewHelpers;
using Toggl.Shared.Extensions;
using static Toggl.Core.UI.Helper.TemporalInconsistency;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        WindowSoftInputMode = SoftInput.StateHidden | SoftInput.AdjustResize,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class EditDurationActivity : ReactiveActivity<EditDurationViewModel>
    {
        private readonly Dictionary<TemporalInconsistency, string> inconsistencyMessages = new Dictionary<TemporalInconsistency, string>
        {
            [StartTimeAfterCurrentTime] = Shared.Resources.StartTimeAfterCurrentTimeWarning,
            [StartTimeAfterStopTime] = Shared.Resources.StartTimeAfterStopTimeWarning,
            [StopTimeBeforeStartTime] = Shared.Resources.StopTimeBeforeStartTimeWarning,
            [DurationTooLong] = Shared.Resources.DurationTooLong,
        };

        private readonly Subject<Unit> viewClosedSubject = new Subject<Unit>();
        private readonly Subject<DateTimeOffset> activeEditionChangedSubject = new Subject<DateTimeOffset>();

        private DateTimeOffset minDateTime;
        private DateTimeOffset maxDateTime;
        private EditMode editMode = EditMode.None;
        private bool canDismiss = true;
        private bool is24HoursFormat;

        private Dialog editDialog;
        private Toast toast;

        public EditDurationActivity() : base(
            Resource.Layout.EditDurationActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromBottom)
        { }

        public EditDurationActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            ViewModel.TimeFormat
                .Subscribe(v => is24HoursFormat = v.IsTwentyFourHoursFormat)
                .DisposedBy(DisposeBag);

            ViewModel.StartTimeString
                .Subscribe(startTimeText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.StartDateString
                .Subscribe(startDateText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.StopTimeString
                .Subscribe(stopTimeText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.StopDateString
                .Subscribe(stopDateText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.IsRunning
                .Subscribe(updateStopTimeUIVisibility)
                .DisposedBy(DisposeBag);

            ViewModel.MinimumDateTime
                .Subscribe(min => minDateTime = min)
                .DisposedBy(DisposeBag);

            ViewModel.MaximumDateTime
                .Subscribe(max => maxDateTime = max)
                .DisposedBy(DisposeBag);

            stopTimerLabel.Rx()
                .BindAction(ViewModel.StopTimeEntry)
                .DisposedBy(DisposeBag);

            startTimeText.Rx().Tap()
                .Subscribe(_ => { editMode = EditMode.Time; })
                .DisposedBy(DisposeBag);

            startDateText.Rx().Tap()
                .Subscribe(_ => { editMode = EditMode.Date; })
                .DisposedBy(DisposeBag);

            startTimeText.Rx()
                .BindAction(ViewModel.EditStartTime)
                .DisposedBy(DisposeBag);

            startDateText.Rx()
                .BindAction(ViewModel.EditStartTime)
                .DisposedBy(DisposeBag);

            stopTimeText.Rx().Tap()
                .Subscribe(_ => { editMode = EditMode.Time; })
                .DisposedBy(DisposeBag);

            stopDateText.Rx().Tap()
                .Subscribe(_ => { editMode = EditMode.Date; })
                .DisposedBy(DisposeBag);

            stopTimeText.Rx()
                .BindAction(ViewModel.EditStopTime)
                .DisposedBy(DisposeBag);

            stopDateText.Rx()
                .BindAction(ViewModel.EditStopTime)
                .DisposedBy(DisposeBag);

            ViewModel.TemporalInconsistencies
                .Subscribe(onTemporalInconsistency)
                .DisposedBy(DisposeBag);

            ViewModel.IsEditingStartTime
                .Where(CommonFunctions.Identity)
                .SelectMany(_ => ViewModel.StartTime)
                .Subscribe(startEditing)
                .DisposedBy(DisposeBag);

            ViewModel.IsEditingStopTime
                .Where(CommonFunctions.Identity)
                .SelectMany(_ => ViewModel.StopTime)
                .Subscribe(startEditing)
                .DisposedBy(DisposeBag);

            activeEditionChangedSubject
                .Subscribe(ViewModel.ChangeActiveTime.Inputs)
                .DisposedBy(DisposeBag);

            viewClosedSubject
                .Subscribe(ViewModel.StopEditingTime.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.IsEditingTime
                .Invert()
                .Subscribe(wheelNumericInput.Rx().Enabled())
                .DisposedBy(DisposeBag);

            ViewModel.Duration
                .Where(_ => !wheelNumericInput.HasFocus)
                .Subscribe(wheelNumericInput.SetDuration)
                .DisposedBy(DisposeBag);

            wheelNumericInput.Duration
                .Subscribe(ViewModel.ChangeDuration.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.MinimumStartTime
                .Subscribe(v => wheelForeground.MinimumStartTime = v)
                .DisposedBy(DisposeBag);

            ViewModel.MaximumStartTime
                .Subscribe(v => wheelForeground.MaximumStartTime = v)
                .DisposedBy(DisposeBag);

            ViewModel.MinimumStopTime
                .Subscribe(v => wheelForeground.MinimumEndTime = v)
                .DisposedBy(DisposeBag);

            ViewModel.MaximumStopTime
                .Subscribe(v => wheelForeground.MaximumEndTime = v)
                .DisposedBy(DisposeBag);

            ViewModel.StartTime
                .DistinctUntilChanged()
                .Subscribe(v => wheelForeground.StartTime = v)
                .DisposedBy(DisposeBag);

            ViewModel.StopTime
                .Select(endTime => endTime.RoundDownToMinute())
                .DistinctUntilChanged()
                .Subscribe(v => wheelForeground.EndTime = v)
                .DisposedBy(DisposeBag);

            ViewModel.IsRunning
                .Subscribe(v => wheelForeground.IsRunning = v)
                .DisposedBy(DisposeBag);

            wheelForeground.StartTimeObservable
                .Subscribe(ViewModel.ChangeStartTime.Inputs)
                .DisposedBy(DisposeBag);

            wheelForeground.EndTimeObservable
                .Subscribe(ViewModel.ChangeStopTime.Inputs)
                .DisposedBy(DisposeBag);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.OneButtonMenu, menu);
            var saveMenuItem = menu.FindItem(Resource.Id.ButtonMenuItem);
            saveMenuItem.SetTitle(Shared.Resources.Save);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.ButtonMenuItem)
            {
                wheelNumericInput.ApplyDurationIfBeingEdited();
                ViewModel.Save.Execute();
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void updateStopTimeUIVisibility(bool isRunning)
        {
            var stopDateTimeViewsVisibility = (!isRunning).ToVisibility();
            stopTimerLabel.Visibility = isRunning.ToVisibility();
            stopTimeText.Visibility = stopDateTimeViewsVisibility;
            stopDateText.Visibility = stopDateTimeViewsVisibility;
            stopDotSeparator.Visibility = stopDateTimeViewsVisibility;
        }

        protected override void OnStop()
        {
            base.OnStop();
            canDismiss = true;
            editDialog?.Dismiss();
        }

        private void onTemporalInconsistency(TemporalInconsistency temporalInconsistency)
        {
            canDismiss = false;
            toast?.Cancel();
            toast = null;
            
            var message = inconsistencyMessages[temporalInconsistency];

            toast = Toast.MakeText(this, message, ToastLength.Short);
            toast.Show();
        }

        private void startEditing(DateTimeOffset initialValue)
        {
            if (editMode == EditMode.None)
                return;

            var localInitialValue = initialValue.ToLocalTime();

            if (editMode == EditMode.Time)
            {
                editTime(localInitialValue);
            }
            else
            {
                editDate(localInitialValue);
            }
        }

        private void editTime(DateTimeOffset currentTime)
        {
            if (editDialog == null)
            {
                var timePickerDialog = new TimePickerDialog(this, Resource.Style.WheelDialogStyle, new TimePickerListener(currentTime, activeEditionChangedSubject.OnNext),
                    currentTime.Hour, currentTime.Minute, is24HoursFormat);

                void resetAction()
                {
                    timePickerDialog.UpdateTime(currentTime.Hour, currentTime.Minute);
                }

                editDialog = timePickerDialog;
                editDialog.DismissEvent += (_, __) => onCurrentEditDialogDismiss(resetAction);
                editDialog.Show();
            }
        }
        private void editDate(DateTimeOffset currentDate)
        {
            if (editDialog == null)
            {
                var datePickerDialog = new DatePickerDialog(this, Resource.Style.WheelDialogStyle, new DatePickerListener(currentDate, activeEditionChangedSubject.OnNext),
                    currentDate.Year, currentDate.Month - 1, currentDate.Day);

                // FirstDayOfWeek days start with sunday at 1 and finish with saturday at 7
                var normalizedBeginningOfWeek = (int)ViewModel.BeginningOfWeek + 1;
                datePickerDialog.DatePicker.FirstDayOfWeek = normalizedBeginningOfWeek;

                void updateDateBounds()
                {
                    datePickerDialog.DatePicker.MinDate = minDateTime.ToUnixTimeMilliseconds();
                    datePickerDialog.DatePicker.MaxDate = maxDateTime.ToUnixTimeMilliseconds();
                    datePickerDialog.SetTitle("");
                }

                updateDateBounds();

                void resetAction()
                {
                    updateDateBounds();
                    datePickerDialog.UpdateDate(currentDate.Year, currentDate.Month - 1, currentDate.Day);
                }

                editDialog = datePickerDialog;
                editDialog.DismissEvent += (_, __) => onCurrentEditDialogDismiss(resetAction);
                editDialog.Show();
            }
        }

        private void onCurrentEditDialogDismiss(Action resetAction)
        {
            if (canDismiss)
            {
                editDialog = null;
                viewClosedSubject.OnNext(Unit.Default);
                editMode = EditMode.None;
            }
            else
            {
                resetAction();
                editDialog.Show();
                canDismiss = true;
            }
        }

        private enum EditMode
        {
            Time,
            Date,
            None
        }
    }
}
