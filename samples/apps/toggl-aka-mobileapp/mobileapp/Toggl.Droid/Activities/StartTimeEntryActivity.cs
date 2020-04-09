using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Toggl.Core.Autocomplete;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Onboarding.StartTimeEntryView;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Helper;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              WindowSoftInputMode = SoftInput.StateVisible,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class StartTimeEntryActivity : ReactiveActivity<StartTimeEntryViewModel>
    {
        private static readonly TimeSpan typingThrottleDuration = TimeSpan.FromMilliseconds(300);

        private PopupWindow onboardingPopupWindow;
        private IDisposable onboardingDisposable;

        public StartTimeEntryActivity() : base(
            Resource.Layout.StartTimeEntryActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromBottom)
        {
        }

        public StartTimeEntryActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            ViewModel.Suggestions
                .SubscribeOn(AndroidDependencyContainer.Instance.SchedulerProvider.BackgroundScheduler)
                .Subscribe(adapter.Rx().Items())
                .DisposedBy(DisposeBag);

            adapter.ItemTapObservable
                .Subscribe(ViewModel.SelectSuggestion.Inputs)
                .DisposedBy(DisposeBag);

            adapter.ToggleTasks
                .Subscribe(ViewModel.ToggleTasks.Inputs)
                .DisposedBy(DisposeBag);

            // Displayed time
            ViewModel.DisplayedTime
                .Subscribe(durationLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            // Toggle project suggestions toolbar button
            selectProjectToolbarButton.Rx()
                .BindAction(ViewModel.ToggleProjectSuggestions)
                .DisposedBy(DisposeBag);

            ViewModel.IsSuggestingProjects
                .Select(isSuggesting => isSuggesting ? Resource.Drawable.te_project_active : Resource.Drawable.project)
                .Subscribe(selectProjectToolbarButton.SetImageResource)
                .DisposedBy(DisposeBag);

            // Toggle tag suggestions toolbar button
            selectTagToolbarButton.Rx()
                .BindAction(ViewModel.ToggleTagSuggestions)
                .DisposedBy(DisposeBag);

            ViewModel.IsSuggestingTags
                .Select(isSuggesting => isSuggesting ? Resource.Drawable.te_tag_active : Resource.Drawable.tag)
                .Subscribe(selectTagToolbarButton.SetImageResource)
                .DisposedBy(DisposeBag);

            // Billable toolbar button
            selectBillableToolbarButton.Rx()
                .BindAction(ViewModel.ToggleBillable)
                .DisposedBy(DisposeBag);

            ViewModel.IsBillable
                .Select(isSuggesting => isSuggesting ? Resource.Drawable.te_billable_active : Resource.Drawable.billable)
                .Subscribe(selectBillableToolbarButton.SetImageResource)
                .DisposedBy(DisposeBag);

            ViewModel.IsBillableAvailable
                .Subscribe(selectBillableToolbarButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            // Description text field
            descriptionField.Hint = ViewModel.PlaceholderText;

            ViewModel.TextFieldInfo
                .Subscribe(onTextFieldInfo)
                .DisposedBy(DisposeBag);

            durationLabel.Rx()
                .BindAction(ViewModel.ChangeTime)
                .DisposedBy(DisposeBag);

            descriptionField.TextObservable
                .SubscribeOn(ThreadPoolScheduler.Instance)
                .Throttle(typingThrottleDuration)
                .Select(text => text.AsImmutableSpans(descriptionField.SelectionStart))
                .Subscribe(ViewModel.SetTextSpans)
                .DisposedBy(DisposeBag);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.OneButtonMenu, menu);
            var doneMenuItem = menu.FindItem(Resource.Id.ButtonMenuItem);
            doneMenuItem.SetTitle(Shared.Resources.Done);
            
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.ButtonMenuItem)
            {
                ViewModel.Done.Execute();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        protected override void OnResume()
        {
            base.OnResume();
            descriptionField.RequestFocus();
            selectProjectToolbarButton.LayoutChange += onSelectProjectToolbarButtonLayoutChanged;
        }

        private void onSelectProjectToolbarButtonLayoutChanged(object sender, View.LayoutChangeEventArgs changeEventArgs)
        {
            selectProjectToolbarButton.Post(setupStartTimeEntryOnboardingStep);
        }

        protected override void OnStop()
        {
            base.OnStop();
            selectProjectToolbarButton.LayoutChange -= onSelectProjectToolbarButtonLayoutChanged;
            onboardingPopupWindow?.Dismiss();
            onboardingPopupWindow = null;
        }

        private void setupStartTimeEntryOnboardingStep()
        {
            clearPreviousOnboardingSetup();

            onboardingPopupWindow = PopupWindowFactory.PopupWindowWithText(
                this,
                Resource.Layout.TooltipWithCenteredBottomArrow,
                Resource.Id.TooltipText,
                Shared.Resources.AddProjectBubbleText);

            var storage = ViewModel.OnboardingStorage;

            onboardingDisposable = new AddProjectOrTagOnboardingStep(storage, ViewModel.DataSource)
                .ManageDismissableTooltip(
                    Observable.Return(true),
                    onboardingPopupWindow,
                    selectProjectToolbarButton,
                    (popup, anchor) => popup.TopHorizontallyCenteredOffsetsTo(anchor, 8),
                    storage);
        }

        private void clearPreviousOnboardingSetup()
        {
            onboardingDisposable?.Dispose();
            onboardingDisposable = null;
            onboardingPopupWindow?.Dismiss();
            onboardingPopupWindow = null;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            DisposeBag?.Dispose();
            onboardingDisposable?.Dispose();
        }

        private void onTextFieldInfo(TextFieldInfo textFieldInfo)
        {
            var (formattedText, cursorPosition) = textFieldInfo.AsSpannableTextAndCursorPosition();
            if (descriptionField.TextFormatted.ToString() == formattedText.ToString())
                return;

            descriptionField.TextFormatted = formattedText;
            descriptionField.SetSelection(cursorPosition);
        }
    }
}