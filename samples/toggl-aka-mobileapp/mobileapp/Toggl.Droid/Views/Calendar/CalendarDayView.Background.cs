using System;
using System.Collections.Immutable;
using Android.Graphics;
using Toggl.Shared;
using System.Globalization;
using System.Linq;
using Toggl.Core.UI.Helper;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Views.Calendar
{
    public partial class CalendarDayView
    {
        private float hoursX;
        private float timeSliceStartX;
        private float timeSlicesTopPadding;
        private float verticalLineLeftMargin;
        private float hoursDistanceFromTimeLine;
        private TimeFormat timeOfDayFormat = TimeFormat.TwelveHoursFormat;
        private ImmutableArray<string> hours = ImmutableArray<string>.Empty;
        private ImmutableArray<float> timeLinesYs = ImmutableArray<float>.Empty;
        private ImmutableArray<float> hoursYs = ImmutableArray<float>.Empty;
        private Paint hoursLabelPaint;
        private Paint linesPaint;

        partial void initBackgroundBackingFields()
        {
            timeSliceStartX = 60.DpToPixels(Context);
            timeSlicesTopPadding = 0;
            verticalLineLeftMargin = 68.DpToPixels(Context);
            hoursDistanceFromTimeLine = 12.DpToPixels(Context);
            hoursX = timeSliceStartX - hoursDistanceFromTimeLine;
            hours = createHours();

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

        partial void processBackgroundOnLayout(bool changed, int left, int top, int right, int bottom)
        {
            if (!changed) 
                return;
            
            timeLinesYs = createTimeLinesYPositions();
            hoursYs = timeLinesYs.Select(lineY => lineY + hoursLabelPaint.Descent()).ToImmutableArray();
        }

        partial void drawHourLines(Canvas canvas)
        {
            canvas.DrawLine(verticalLineLeftMargin, 0f, verticalLineLeftMargin, maxHeight, linesPaint);

            var timeLinesYsToDraw = timeLinesYs;
            var hourLabelYsToDraw = hoursYs;
            var hoursToDraw = hours;
            for (var hour = 1; hour < timeLinesYsToDraw.Length; hour++)
            {
                var hourTop = hourLabelYsToDraw[hour] + linesPaint.Ascent();
                var hourBottom = hourLabelYsToDraw[hour] + linesPaint.Descent();
                if (!(hourBottom > scrollOffset) || !(hourTop - scrollOffset < Height)) continue;

                canvas.DrawLine(timeSliceStartX, timeLinesYsToDraw[hour], Width, timeLinesYsToDraw[hour], linesPaint);
                canvas.DrawText(hoursToDraw[hour], hoursX, hourLabelYsToDraw[hour], hoursLabelPaint);
            }
        }

        private ImmutableArray<float> createTimeLinesYPositions()
            => Enumerable.Range(0, hoursPerDay)
                .Select(line => line * hourHeight + timeSlicesTopPadding)
                .ToImmutableArray();

        private ImmutableArray<string> createHours()
        {
            var date = new DateTime();
            return Enumerable.Range(0, hoursPerDay)
                .Select(hour => date.AddHours(hour))
                .Select(formatHour)
                .ToImmutableArray();
        }

        private string formatHour(DateTime hour)
            => hour.ToString(fixedHoursFormat(), DateFormatCultureInfo.CurrentCulture);

        private string fixedHoursFormat()
            => timeOfDayFormat.IsTwentyFourHoursFormat
                ? Shared.Resources.TwentyFourHoursFormat
                : Shared.Resources.TwelveHoursFormat;
    }
}
