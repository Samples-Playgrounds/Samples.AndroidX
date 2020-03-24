using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Analytics;
using Toggl.Core.Exceptions;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using Toggl.iOS.Views.EditDuration.Shapes;
using Toggl.iOS.Views.EditDuration.Shapes.Caps;
using Toggl.Shared.Extensions;
using UIKit;
using static Toggl.Shared.Math;

namespace Toggl.iOS.Views.EditDuration
{
    [Register(nameof(WheelForegroundView))]
    public sealed class WheelForegroundView : BaseWheelView
    {
        public event EventHandler StartTimeChanged;

        public event EventHandler EndTimeChanged;

        public IObservable<EditTimeSource> TimeEdited
            => timeEditedSubject.AsObservable();

        public DateTimeOffset MinimumStartTime { get; set; }

        public DateTimeOffset MaximumStartTime { get; set; }

        public DateTimeOffset MinimumEndTime { get; set; }

        public DateTimeOffset MaximumEndTime { get; set; }

        public DateTimeOffset StartTime
        {
            get => startTime;
            set
            {
                if (startTime == value) return;
                startTime = value.Clamp(MinimumStartTime, MaximumStartTime);
                StartTimeChanged?.Invoke(this, new EventArgs());
                SetNeedsLayout();
            }
        }

        public DateTimeOffset EndTime
        {
            get => endTime < startTime ? startTime : endTime;
            set
            {
                if (endTime == value) return;
                endTime = value.Clamp(MinimumEndTime, MaximumEndTime);
                EndTimeChanged?.Invoke(this, new EventArgs());
                SetNeedsLayout();
            }
        }

        public bool IsRunning
        {
            get => isRunning;
            set
            {
                if (isRunning == value) return;
                isRunning = value;
                SetNeedsLayout();
            }
        }

        private CGColor backgroundColor
            => Colors.EditDuration.Wheel.Rainbow.GetPingPongIndexedItem(numberOfFullLoops).ToNativeColor().CGColor;

        private CGColor foregroundColor
            => Colors.EditDuration.Wheel.Rainbow.GetPingPongIndexedItem(numberOfFullLoops + 1).ToNativeColor().CGColor;

        private bool isRunning;

        private DateTimeOffset startTime;
        private DateTimeOffset endTime;

        private CGPoint startTimePosition;
        private CGPoint endTimePosition;

        private double startTimeAngle => startTime.LocalDateTime.TimeOfDay.ToAngleOnTheDial().ToPositiveAngle();
        private double endTimeAngle => endTime.LocalDateTime.TimeOfDay.ToAngleOnTheDial().ToPositiveAngle();

        private readonly nfloat extendedRadiusMultiplier = 1.5f;
        private double endPointsRadius => (Radius + SmallRadius) / 2;
        private bool isDragging;
        private UISelectionFeedbackGenerator feedbackGenerator;
        private WheelUpdateType updateType;
        private double editBothAtOnceStartTimeAngleOffset;

        private int numberOfFullLoops => (int)((EndTime - StartTime).TotalMinutes / MinutesInAnHour);
        private bool isFullCircle => numberOfFullLoops >= 1;

        private readonly Subject<EditTimeSource> timeEditedSubject = new Subject<EditTimeSource>();

        private CAShapeLayer fullWheel;
        private Arc arc;
        private EndCap endCap;
        private StartCap startCap;

        private CGRect currentFrame;

        public WheelForegroundView(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            var gestureRecognizer = new UIPanGestureRecognizer(handleTouch);
            AddGestureRecognizer(gestureRecognizer);

            SetNeedsLayout();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            calculateEndPointPositions();

            if (currentFrame != Frame || arc == null)
            {
                createSubLayers();
                currentFrame = Frame;
            }

            CATransaction.Begin();
            CATransaction.DisableActions = true;

            updateUIElements();

            CATransaction.Commit();
        }

        private void updateUIElements()
        {
            startCap.Position = startTimePosition;
            startCap.Color = foregroundColor;
            startCap.Angle = startTimeAngle;
            endCap.Position = endTimePosition;
            endCap.Color = foregroundColor;
            endCap.Angle = endTimeAngle;
            endCap.ShowOnlyBackground = IsRunning;

            fullWheel.FillColor = backgroundColor;
            fullWheel.Hidden = !isFullCircle;

            arc.Color = foregroundColor;
            arc.Update((nfloat)startTimeAngle, (nfloat)endTimeAngle);
        }

        private void createSubLayers()
        {
            RemoveSublayers();

            fullWheel = new Wheel(Center, Radius, SmallRadius, backgroundColor);
            Layer.AddSublayer(fullWheel);

            arc = new Arc(Bounds, Center, (Radius + SmallRadius) / 2, Thickness);
            Layer.AddSublayer(arc);

            endCap = new EndCap(Resize);
            Layer.AddSublayer(endCap);

            startCap = new StartCap(Resize);
            Layer.AddSublayer(startCap);
        }

        private void calculateEndPointPositions()
        {
            var center = Center.ToTogglPoint();

            startTimePosition = PointOnCircumference(center, startTimeAngle, endPointsRadius).ToCGPoint();
            endTimePosition = PointOnCircumference(center, endTimeAngle, endPointsRadius).ToCGPoint();
        }

        #region Touch interaction

        private void handleTouch(UIPanGestureRecognizer recognizer)
        {
            var position = recognizer.LocationInView(this);
            switch (recognizer.State)
            {
                case UIGestureRecognizerState.Began:
                    touchesBegan(position);
                    break;
                case UIGestureRecognizerState.Changed:
                    touchesMoved(position);
                    break;
                case UIGestureRecognizerState.Ended:
                    touchesEnded();
                    break;
                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    touchesCancelled();
                    break;
            }
        }

        private void touchesBegan(CGPoint position)
        {
            if (isValid(position))
            {
                isDragging = true;
                feedbackGenerator = new UISelectionFeedbackGenerator();
                feedbackGenerator.Prepare();
            }
        }

        private void touchesMoved(CGPoint position)
        {
            if (isDragging == false) return;

            double previousAngle;
            switch (updateType)
            {
                case WheelUpdateType.EditStartTime:
                    previousAngle = startTimeAngle;
                    break;
                case WheelUpdateType.EditEndTime:
                    previousAngle = endTimeAngle;
                    break;
                default:
                    previousAngle = startTimeAngle + editBothAtOnceStartTimeAngleOffset;
                    break;
            }

            var currentAngle = AngleBetween(position.ToTogglPoint(), Center.ToTogglPoint());

            var angleChange = currentAngle - previousAngle;
            while (angleChange < -Math.PI) angleChange += FullCircle;
            while (angleChange > Math.PI) angleChange -= FullCircle;

            var timeChange = angleChange.AngleToTime();

            updateEditedTime(timeChange);
        }

        private void touchesCancelled()
        {
            finishTouchEditing();
        }

        private void touchesEnded()
        {
            finishTouchEditing();
            switch (updateType)
            {
                case WheelUpdateType.EditStartTime:
                    timeEditedSubject.OnNext(EditTimeSource.WheelStartTime);
                    break;
                case WheelUpdateType.EditEndTime:
                    timeEditedSubject.OnNext(EditTimeSource.WheelEndTime);
                    break;
                default:
                    timeEditedSubject.OnNext(EditTimeSource.WheelBothTimes);
                    break;
            }
        }

        private bool isValid(CGPoint position)
        {
            var intention = determineTapIntention(position);
            if (intention.HasValue)
            {
                updateType = intention.Value;
                if (updateType == WheelUpdateType.EditBothAtOnce)
                {
                    editBothAtOnceStartTimeAngleOffset =
                        AngleBetween(position.ToTogglPoint(), Center.ToTogglPoint()) - startTimeAngle;
                }

                return true;
            }

            return false;
        }

        private WheelUpdateType? determineTapIntention(CGPoint position)
        {
            if (touchesStartCap(position))
                return WheelUpdateType.EditStartTime;

            if (!IsRunning && touchesEndCap(position))
                return WheelUpdateType.EditEndTime;

            if (touchesStartCap(position, extendedRadius: true))
                return WheelUpdateType.EditStartTime;

            if (!IsRunning && touchesEndCap(position, extendedRadius: true))
                return WheelUpdateType.EditEndTime;

            if (!IsRunning && isOnTheWheelBetweenStartAndStop(position))
                return WheelUpdateType.EditBothAtOnce;

            return null;
        }

        private bool touchesStartCap(CGPoint position, bool extendedRadius = false)
            => isCloseEnough(position, startTimePosition, calculateCapRadius(extendedRadius));

        private bool touchesEndCap(CGPoint position, bool extendedRadius = false)
            => isCloseEnough(position, endTimePosition, calculateCapRadius(extendedRadius));

        private nfloat calculateCapRadius(bool extendedRadius)
            => (extendedRadius ? extendedRadiusMultiplier : 1) * (Thickness / 2);

        private static bool isCloseEnough(CGPoint tapPosition, CGPoint endPoint, nfloat radius)
            => DistanceSq(tapPosition.ToTogglPoint(), endPoint.ToTogglPoint()) <= radius * radius;

        private bool isOnTheWheelBetweenStartAndStop(CGPoint point)
        {
            var distanceFromCenterSq = DistanceSq(Center.ToTogglPoint(), point.ToTogglPoint());

            if (distanceFromCenterSq < SmallRadius * SmallRadius
                || distanceFromCenterSq > Radius * Radius)
            {
                return false;
            }

            var angle = AngleBetween(point.ToTogglPoint(), Center.ToTogglPoint());
            return isFullCircle || angle.IsBetween(startTimeAngle, endTimeAngle);
        }

        private void updateEditedTime(TimeSpan diff)
        {
            var giveFeedback = false;
            var duration = EndTime - StartTime;

            if (updateType == WheelUpdateType.EditStartTime
                || updateType == WheelUpdateType.EditBothAtOnce)
            {
                var nextStartTime = (StartTime + diff).RoundToClosestMinute();
                giveFeedback = nextStartTime != StartTime;
                StartTime = nextStartTime;
            }

            if (updateType == WheelUpdateType.EditEndTime)
            {
                var nextEndTime = (EndTime + diff).RoundToClosestMinute();
                giveFeedback = nextEndTime != EndTime;
                EndTime = nextEndTime;
            }

            if (updateType == WheelUpdateType.EditBothAtOnce)
            {
                EndTime = StartTime + duration;
            }

            if (giveFeedback)
            {
                feedbackGenerator.SelectionChanged();
                feedbackGenerator.Prepare();
            }
        }

        private void finishTouchEditing()
        {
            isDragging = false;
            feedbackGenerator = null;
        }

        #endregion

        private enum WheelUpdateType
        {
            EditStartTime,
            EditEndTime,
            EditBothAtOnce
        }
    }
}
