using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using System;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Views
{
    [Register("toggl.droid.views.ReportsCalendarDayView")]
    public sealed class ReportsCalendarDayView : TextView
    {
        private readonly int cornerRadius;
        private readonly Paint circlePaint;
        private readonly Paint selectedPaint;
        private readonly int verticalPadding;

        private bool isToday;

        public bool IsToday
        {
            get => isToday;
            set
            {
                isToday = value;
                PostInvalidate();
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                PostInvalidate();
            }
        }

        private bool roundLeft;

        public bool RoundLeft
        {
            get => roundLeft;
            set
            {
                roundLeft = value;
                PostInvalidate();
            }
        }

        private bool roundRight;

        public bool RoundRight
        {
            get => roundRight;
            set
            {
                roundRight = value;
                PostInvalidate();
            }
        }

        private bool isSingleDaySelection;

        public bool IsSingleDaySelection
        {
            get => isSingleDaySelection;
            set
            {
                isSingleDaySelection = value;
                PostInvalidate();
            }
        }

        public ReportsCalendarDayView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ReportsCalendarDayView(Context context)
            : this(context, null)
        {
        }

        public ReportsCalendarDayView(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {
        }

        public ReportsCalendarDayView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            cornerRadius = 23.DpToPixels(context);
            verticalPadding = 6.DpToPixels(context);
            selectedPaint = new Paint
            {
                Flags = PaintFlags.AntiAlias,
                Color = context.SafeGetColor(Resource.Color.calendarSelected)
            };

            circlePaint = new Paint
            {
                Flags = PaintFlags.AntiAlias,
                Color = context.SafeGetColor(Resource.Color.placeholderText)
            };
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
        }

        public override void Draw(Canvas canvas)
        {
            var width = Width;
            var height = Height - verticalPadding * 2;

            if (IsSelected)
            {
                drawCircle(canvas, width, height, selectedPaint);

                if (!IsSingleDaySelection)
                {
                    var squareRectLeft = RoundLeft ? cornerRadius : 0;
                    var squareRectRight = width - (RoundRight ? cornerRadius : 0);
                    var squareRect = new RectF(squareRectLeft, verticalPadding, squareRectRight, height + verticalPadding);
                    canvas.DrawRect(squareRect, selectedPaint);
                }
            }
            else if (IsToday)
            {
                drawCircle(canvas, width, height, circlePaint);
            }

            base.Draw(canvas);
        }

        private void drawCircle(Canvas canvas, float width, float height, Paint paint)
        {
            var centerX = width / 2;
            var centerY = height / 2 + verticalPadding;
            var radius = Math.Min(width, height) / 2;

            canvas.DrawCircle(centerX, centerY, radius, paint);
        }
    }
}
