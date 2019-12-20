using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AndroidX.RecyclerView.Widget;
using Toggl.Core;
using Toggl.Core.Helper;
using Toggl.Core.UI.Helper;
using Toggl.Droid.Extensions;
using Toggl.Shared;

namespace Toggl.Droid.Views
{
    [Register("toggl.droid.views.CalendarRecyclerView")]
    public class CalendarRecyclerView : RecyclerView
    {
        private const int hoursPerDay = Constants.HoursPerDay;
        private const float secondsInOneHour = 60f * 60f;

        private static readonly string twelveHoursFormat = Shared.Resources.TwelveHoursFormat;
        private static readonly string twentyFourHoursFormat = Shared.Resources.TwentyFourHoursFormat;

        private readonly ISubject<PointF> emptySpaceTouchedSubject = new Subject<PointF>();

        private TimeFormat timeOfDayFormat = TimeFormat.TwelveHoursFormat;
        private ITimeService timeService;
        private bool hasTwoColumns;

        private float hoursX;
        private float hourHeight;
        private float timeSliceStartX;
        private float timeSlicesTopPadding;
        private float verticalLineLeftMargin;
        private float middleLineX;
        private float hoursDistanceFromTimeLine;
        private ImmutableArray<string> hours = ImmutableArray<string>.Empty;
        private ImmutableArray<float> timeLinesYs = ImmutableArray<float>.Empty;
        private ImmutableArray<float> hoursYs = ImmutableArray<float>.Empty;

        private PointF lastTouch;
        private Paint hoursLabelPaint;
        private Paint linesPaint;

        private int maxDistanceBetweenDownAndUpTouches;

        public IObservable<DateTimeOffset> EmptySpansTouchedObservable
            => emptySpaceTouchedSubject.Select(pointToDateTimeOffset);

        #region Constructors
        protected CalendarRecyclerView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public CalendarRecyclerView(Context context)
            : base(context)
        {
            init();
        }

        public CalendarRecyclerView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            init();
        }

        public CalendarRecyclerView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            init();
        }

        private void init()
        {
            hourHeight = 56.DpToPixels(Context);
            timeSliceStartX = 60.DpToPixels(Context);
            timeSlicesTopPadding = 0;
            verticalLineLeftMargin = 68.DpToPixels(Context);
            hoursDistanceFromTimeLine = 12.DpToPixels(Context);
            hoursX = timeSliceStartX - hoursDistanceFromTimeLine;
            maxDistanceBetweenDownAndUpTouches = 6.DpToPixels(Context);

            linesPaint = new Paint(PaintFlags.AntiAlias)
            {
                Color = Context.SafeGetColor(Resource.Color.separator),
                StrokeWidth = 1.DpToPixels(Context)
            };

            hoursLabelPaint = new Paint(PaintFlags.AntiAlias)
            {
                Color = Context.SafeGetColor(Resource.Color.reportsLabel),
                TextAlign = Paint.Align.Right,
                TextSize = 12.SpToPixels(Context)
            };
        }

        #endregion

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            timeLinesYs = createTimeLinesYPositions();
            hours = createHours();
            hoursYs = timeLinesYs.Select(lineY => lineY + hoursLabelPaint.Descent()).ToImmutableArray();
            middleLineX = verticalLineLeftMargin + (Width - verticalLineLeftMargin) / 2f;
        }

        public override bool OnTouchEvent(MotionEvent motionEvent)
        {
            switch (motionEvent.Action)
            {
                case MotionEventActions.Down:
                    var touchDown = motionEvent.ToPointF();
                    if (pointIsInActionableArea(touchDown))
                    {
                        lastTouch = touchDown;
                    }
                    return base.OnTouchEvent(motionEvent);

                case MotionEventActions.Up:
                    var touchUp = motionEvent.ToPointF();
                    if (pointIsInActionableArea(touchUp)
                        && touchUpIsCloseEnoughToLastTouch(touchUp)
                        && !lastTouchInterceptsAnyChild(touchUp))
                    {
                        emptySpaceTouchedSubject.OnNext(touchUp);
                    }

                    lastTouch = null;
                    return base.OnTouchEvent(motionEvent);

                case MotionEventActions.Cancel:
                case MotionEventActions.Outside:
                    lastTouch = null;
                    return base.OnTouchEvent(motionEvent);

                default:
                    return base.OnTouchEvent(motionEvent);
            }
        }

        public void SetTimeService(ITimeService timeService)
        {
            this.timeService = timeService;
        }

        private DateTimeOffset pointToDateTimeOffset(PointF point)
        {
            var seconds = (point.Y + ComputeVerticalScrollOffset()) / hourHeight * secondsInOneHour;
            var timeSpan = TimeSpan.FromSeconds(seconds);
            return todayDate() + timeSpan;
        }

        private DateTime todayDate()
        {
            return timeService?.CurrentDateTime.ToLocalTime().Date ?? DateTimeOffset.Now.Date;
        }

        private bool pointIsInActionableArea(PointF point)
        {
            var minX = hasTwoColumns ? middleLineX : verticalLineLeftMargin;
            return point.X > minX;
        }

        private bool lastTouchInterceptsAnyChild(PointF touchUp)
            => FindChildViewUnder(touchUp.X, touchUp.Y) != null;

        private bool touchUpIsCloseEnoughToLastTouch(PointF touchUp)
        {
            if (lastTouch == null)
                return false;

            var distance = Shared.Math.DistanceSq(touchUp.ToPoint(), lastTouch.ToPoint());

            return distance < maxDistanceBetweenDownAndUpTouches;
        }

        private ImmutableArray<string> createHours()
        {
            DateTime date = new DateTime();
            return Enumerable.Range(0, hoursPerDay)
                .Select(hour => date.AddHours(hour))
                .Select(formatHour)
                .ToImmutableArray();
        }

        private ImmutableArray<float> createTimeLinesYPositions()
         => Enumerable.Range(0, hoursPerDay)
             .Select(line => line * hourHeight + timeSlicesTopPadding)
             .ToImmutableArray();

        public override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            var offset = ComputeVerticalScrollOffset();

            canvas.DrawLine(verticalLineLeftMargin, 0f, verticalLineLeftMargin, Height, linesPaint);
            if (hasTwoColumns)
            {
                canvas.DrawLine(middleLineX, 0f, middleLineX, Height, linesPaint);
            }

            for (var hour = 1; hour < timeLinesYs.Length; hour++)
            {
                canvas.DrawLine(timeSliceStartX, timeLinesYs[hour] - offset, Width, timeLinesYs[hour] - offset, linesPaint);
                canvas.DrawText(hours[hour], hoursX, hoursYs[hour] - offset, hoursLabelPaint);
            }
        }

        public void SetHasTwoColumns(bool shouldHaveTwoColumns)
        {
            if (hasTwoColumns == shouldHaveTwoColumns) return;
            hasTwoColumns = shouldHaveTwoColumns;
            Invalidate();
        }

        public void SetHourFormat(TimeFormat timeFormat)
        {
            timeOfDayFormat = timeFormat;
            Invalidate();
        }

        private string formatHour(DateTime hour)
            => hour.ToString(fixedHoursFormat(), DateFormatCultureInfo.CurrentCulture);

        private string fixedHoursFormat()
            => timeOfDayFormat.IsTwentyFourHoursFormat ? twentyFourHoursFormat : twelveHoursFormat;
    }
}
