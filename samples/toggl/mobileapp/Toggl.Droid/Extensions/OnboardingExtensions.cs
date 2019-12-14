using Android.App;
using Android.Views;
using Android.Widget;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Extensions;
using Toggl.Storage.Onboarding;
using Toggl.Storage.Settings;

namespace Toggl.Droid.Extensions
{
    public static class OnboardingExtensions
    {
        private const int windowTokenCheckInterval = 100;

        public static IDisposable ManageDismissableTooltip(
            this IOnboardingStep step,
            IObservable<bool> componentIsVisible,
            PopupWindow tooltip,
            View anchor,
            Func<PopupWindow, View, PopupOffsets> popupOffsetsGenerator,
            IOnboardingStorage storage)
        {
            Ensure.Argument.IsNotNull(anchor, nameof(anchor));

            var dismissableStep = step.ToDismissable(step.GetType().FullName, storage);

            dismissableStep.DismissByTapping(tooltip, () => { });

            return dismissableStep.ManageVisibilityOf(componentIsVisible, tooltip, anchor, popupOffsetsGenerator);
        }

        public static IDisposable ManageVisibilityOf(
            this IOnboardingStep step,
            IObservable<bool> componentIsVisible,
            PopupWindow tooltip,
            View anchor,
            Func<PopupWindow, View, PopupOffsets> popupOffsetsGenerator)
        {
            Ensure.Argument.IsNotNull(anchor, nameof(anchor));

            void toggleVisibilityOnMainThread(bool shouldBeVisible)
            {
                if (shouldBeVisible)
                {
                    showPopupTooltip(tooltip, anchor, popupOffsetsGenerator);
                }
                else
                {
                    tooltip?.Dismiss();
                }
            }

            return step.ShouldBeVisible
                .CombineLatest(componentIsVisible, CommonFunctions.And)
                .ObserveOn(AndroidDependencyContainer.Instance.SchedulerProvider.MainScheduler)
                .combineWithWindowTokenAvailabilityFrom(anchor)
                .Subscribe(toggleVisibilityOnMainThread);
        }

        public static void DismissByTapping(this IDismissable step, PopupWindow popupWindow, Action cleanup = null)
        {
            if (popupWindow == null) return;
            
            void OnDismiss(object sender, EventArgs args)
            {
                popupWindow.Dismiss();
                step.Dismiss();
                cleanup();
            }
            
            popupWindow.ContentView.Click += OnDismiss;
        }

        private static void showPopupTooltip(PopupWindow popupWindow, View anchor, Func<PopupWindow, View, PopupOffsets> popupOffsetsGenerator)
        {
            anchor.Post(() =>
            {
                var activity = anchor.Context as Activity;
                if (popupWindow == null || activity == null || activity.IsFinishing)
                    return;

                popupWindow.ContentView.Measure(View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified), View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
                popupWindow.Height = ViewGroup.LayoutParams.WrapContent;
                popupWindow.Width = ViewGroup.LayoutParams.WrapContent;
                var offsets = popupOffsetsGenerator(popupWindow, anchor);
                popupWindow.ShowAsDropDown(anchor, offsets.HorizontalOffset, offsets.VerticalOffset);
            });
        }

        private static IObservable<bool> combineWithWindowTokenAvailabilityFrom(this IObservable<bool> shouldBeVisibleObservable, View anchor)
        {
            var viewTokenObservable = Observable.Create<bool>(observer =>
            {
                if (anchor == null)
                {
                    observer.OnNext(false);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }

                void checkForToken()
                {
                    if (anchor.WindowToken == null)
                    {
                        observer.OnNext(false);
                    }
                    else
                    {
                        observer.OnNext(true);
                        observer.OnCompleted();
                    }
                }

                return Observable
                    .Interval(TimeSpan.FromMilliseconds(windowTokenCheckInterval))
                    .Subscribe(_ => checkForToken());
            });

            return shouldBeVisibleObservable.CombineLatest(viewTokenObservable,
                (shouldBeVisible, windowTokenIsReady)
                    => visibleWhenBothAreReady(shouldBeVisible, windowTokenIsReady, viewTokenObservable));
        }

        private static bool visibleWhenBothAreReady(bool shouldBeVisible, bool windowTokenIsReady, IObservable<bool> tokenObservable)
        {
            if (shouldBeVisible)
            {
                return windowTokenIsReady;
            }

            return false;
        }
    }
}
