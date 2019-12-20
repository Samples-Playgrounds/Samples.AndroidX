using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.Animation;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.Constraints;
using Android.Support.Transitions;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.ViewHolders
{
    public class CalendarWeekSectionViewHolder
    {
        private CompositeDisposable disposableBag = new CompositeDisposable();
        private readonly InputAction<CalendarWeeklyViewDayViewModel> dayInputAction;
        private readonly ConstraintLayout rootView;
        private readonly TextView[] dayTextViews;
        private readonly View currentDayIndicator;
        private const long animationDuration = 250;

        private DateTime currentlySelectedDate;
        private ImmutableList<CalendarWeeklyViewDayViewModel> currentWeekSection = ImmutableList<CalendarWeeklyViewDayViewModel>.Empty;

        private enum CalendarWeekDayType
        {
            Disabled,
            Enabled,
            Today,
            Selected,
            TodaySelected
        }
        
        private static Dictionary<CalendarWeekDayType, Typeface> typeFaces = new Dictionary<CalendarWeekDayType, Typeface>
        {
            { CalendarWeekDayType.Disabled, Typeface.Create(Typeface.SansSerif, TypefaceStyle.Normal) },
            { CalendarWeekDayType.Enabled, Typeface.Create(Typeface.SansSerif, TypefaceStyle.Normal) },
            { CalendarWeekDayType.Today, Typeface.Create(Typeface.SansSerif, TypefaceStyle.Bold) },
            { CalendarWeekDayType.Selected, Typeface.Create("sans-serif-medium", TypefaceStyle.Normal) },
            { CalendarWeekDayType.TodaySelected, Typeface.Create(Typeface.SansSerif, TypefaceStyle.Bold) }
        };

        public CalendarWeekSectionViewHolder(ConstraintLayout view, InputAction<CalendarWeeklyViewDayViewModel> dayInputAction)
        {
            this.dayInputAction = dayInputAction;
            rootView = view;
            dayTextViews = view.GetChildren<TextView>().ToArray();
            currentDayIndicator = view.FindViewById(Resource.Id.CurrentDayIndicator);

            for (var dayIndex = 0; dayIndex < dayTextViews.Length; dayIndex++)
            {
                var index = dayIndex;
                dayTextViews[dayIndex].Rx().Tap()
                    .Select(_ => index)
                    .Subscribe(onDayClicked)
                    .DisposedBy(disposableBag);
            }
        }

        private void onDayClicked(int dayIndex)
        {
            if (currentWeekSection.Count <= dayIndex)
                return;

            var dayViewModel = currentWeekSection[dayIndex];
            if (!dayViewModel.Enabled)
                return;

            dayInputAction.Inputs.OnNext(dayViewModel);
        }

        public void InitDaysAndSelectedDate(ImmutableList<CalendarWeeklyViewDayViewModel> weekSection, DateTime newSelectedDate)
        {
            currentlySelectedDate = newSelectedDate;
            currentWeekSection = weekSection;
            updateView();
        }

        public void UpdateCurrentlySelectedDate(DateTime newSelectedDate)
        {
            currentlySelectedDate = newSelectedDate;
            updateView();
        }

        public void UpdateDays(ImmutableList<CalendarWeeklyViewDayViewModel> weekSection)
        {
            currentWeekSection = weekSection;
            updateView();
        }

        private void updateView()
        {
            var weekSection = currentWeekSection;
            var foundCurrentDay = false;
            var constraintSet = new ConstraintSet();
            var transition = new AutoTransition();
            transition.SetDuration(animationDuration);
            var textAnimations = new List<ValueAnimator>();
            constraintSet.Clone(rootView);

            for (var i = 0; i < weekSection.Count; i++)
            {
                var dayViewModel = weekSection[i];
                var dayTextView = dayTextViews[i];
                var weekDayType = getCalendarWeekDayType(dayViewModel);
                dayTextView.Text = dayViewModel.Date.Day.ToString();
                dayTextView.Typeface = typeFaces[weekDayType];
                var startingTextColor = dayTextView.CurrentTextColor;
                var newTextColor = selectTextColorFor(rootView.Context, weekDayType);
                if (startingTextColor != newTextColor)
                {
                    var valueAnimator = createTextColorAnimator(startingTextColor, newTextColor, dayTextView, animationDuration);
                    textAnimations.Add(valueAnimator);
                }

                if (dayViewModel.Date != currentlySelectedDate)
                    continue;

                var dayIndicatorColor = weekDayType == CalendarWeekDayType.TodaySelected ? Resource.Color.accent : Resource.Color.darkBackground;
                currentDayIndicator.BackgroundTintList = ColorStateList.ValueOf(rootView.Context.SafeGetColor(dayIndicatorColor));
                
                constraintSet.Connect(currentDayIndicator.Id, ConstraintSet.Top, dayTextView.Id, ConstraintSet.Top);
                constraintSet.Connect(currentDayIndicator.Id, ConstraintSet.Right, dayTextView.Id, ConstraintSet.Right);
                constraintSet.Connect(currentDayIndicator.Id, ConstraintSet.Bottom, dayTextView.Id, ConstraintSet.Bottom);
                constraintSet.Connect(currentDayIndicator.Id, ConstraintSet.Left, dayTextView.Id, ConstraintSet.Left);
                foundCurrentDay = true;
            }

            constraintSet.SetVisibility(currentDayIndicator.Id, (int) foundCurrentDay.ToVisibility());
            transition.AddListener(new TransitionListener(textAnimations.ToImmutableList()));
            TransitionManager.BeginDelayedTransition(rootView, transition);
            textAnimations.ForEach(animator => animator.Start());
            
            constraintSet.ApplyTo(rootView);
        }

        private static ValueAnimator createTextColorAnimator(int startingTextColor, Color newTextColor, TextView dayTextView, long transitionDuration)
        {
            var valueAnimator = ValueAnimator.OfArgb(startingTextColor, newTextColor);

            valueAnimator.SetDuration(transitionDuration);

            void onValueAnimatorOnUpdate(object sender, ValueAnimator.AnimatorUpdateEventArgs args)
            {
                dayTextView.SetTextColor(new Color((int) valueAnimator.AnimatedValue));
            }

            void onValueAnimatorOnAnimationCancel(object sender, EventArgs args)
            {
                dayTextView.SetTextColor(new Color(startingTextColor));
                clearListeners();
            }

            void onValueAnimatorOnAnimationEnd(object sender, EventArgs args)
            {
                dayTextView.SetTextColor(new Color(newTextColor));
                clearListeners();
            }

            void clearListeners()
            {
                valueAnimator.Update -= onValueAnimatorOnUpdate;
                valueAnimator.AnimationCancel -= onValueAnimatorOnAnimationCancel;
                valueAnimator.AnimationEnd -= onValueAnimatorOnAnimationEnd;
            }

            valueAnimator.Update += onValueAnimatorOnUpdate;
            valueAnimator.AnimationEnd += onValueAnimatorOnAnimationEnd;
            valueAnimator.AnimationCancel += onValueAnimatorOnAnimationCancel;
            return valueAnimator;
        }

        private class TransitionListener : TransitionListenerAdapter
        {
            private readonly ImmutableList<ValueAnimator> animators;

            public TransitionListener(ImmutableList<ValueAnimator> animators)
            {
                this.animators = animators;
            }
            
            public override void OnTransitionCancel(Transition transition)
            {
                base.OnTransitionCancel(transition);
                animators.ForEach(animator => animator.Cancel());
                animators.Clear();
            }

            public override void OnTransitionEnd(Transition transition)
            {
                base.OnTransitionEnd(transition);
                animators.ForEach(animator => animator.End());
                animators.Clear();
            }
        }

        private Color selectTextColorFor(Context context, CalendarWeekDayType calendarWeekDayType)
        {
            switch (calendarWeekDayType)
            {
                case CalendarWeekDayType.Disabled:
                    return context.SafeGetColor(Resource.Color.weekStripeDisabledDayColor);
                case CalendarWeekDayType.Selected:
                    return context.SafeGetColor(Resource.Color.primaryTextOnDarkBackground);
                case CalendarWeekDayType.TodaySelected:
                    return Color.White;
                case CalendarWeekDayType.Today:
                    return context.SafeGetColor(Resource.Color.accent);
                case CalendarWeekDayType.Enabled:
                    return context.SafeGetColor(Resource.Color.primaryText);
                default:
                    throw new ArgumentOutOfRangeException(nameof(calendarWeekDayType), calendarWeekDayType, null);
            }
        }

        private CalendarWeekDayType getCalendarWeekDayType(CalendarWeeklyViewDayViewModel calendarWeeklyViewDayViewModel)
        {
            if (!calendarWeeklyViewDayViewModel.Enabled)
                return CalendarWeekDayType.Disabled;

            if (calendarWeeklyViewDayViewModel.Date == currentlySelectedDate)
            {
                return calendarWeeklyViewDayViewModel.IsToday 
                    ? CalendarWeekDayType.TodaySelected
                    : CalendarWeekDayType.Selected;   
            }

            return calendarWeeklyViewDayViewModel.IsToday
                ? CalendarWeekDayType.Today
                : CalendarWeekDayType.Enabled;
        }

        public void Destroy()
        {
            disposableBag.Dispose();
            disposableBag = new CompositeDisposable();
        }
    }
}