using System;
using Android.Graphics;
using Toggl.Core.UI.Calendar;

namespace Toggl.Droid.ViewHelpers
{
    public sealed class CalendarItemRectAttributes
    {
        public CalendarItemLayoutAttributes Attrs { get; }
        
        public float Left { get; }
        
        public float Right { get; }

        private readonly float totalHours;
        
        public CalendarItemRectAttributes(CalendarItemLayoutAttributes attributes, float left, float right)
        {
            Attrs = attributes;
            Left = left;
            Right = right;
            totalHours = attributes.StartTime.Hour + attributes.StartTime.Minute / 60f;
        }

        public float CalculateTop(float hourHeight)
        {
            return totalHours * hourHeight;
        }

        public float CalculateHeight(float hourHeight, float minHourHeight)
        {
            return (float) Math.Max(minHourHeight, hourHeight * (Attrs.Duration.TotalMinutes / 60f));
        }

        public void CalculateRect(float hourHeight, float minHourHeight, RectF rect)
        {
            var calendarItemTop = CalculateTop(hourHeight);
            var calendarItemHeight = CalculateHeight(hourHeight, minHourHeight);
            var calendarItemBottom = calendarItemTop + calendarItemHeight;
            rect.Set(Left, calendarItemTop, Right, calendarItemBottom);
        } 
    }
}