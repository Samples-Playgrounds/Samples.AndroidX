using Android.Views;
using System;
using AndroidX.RecyclerView.Widget;

namespace Toggl.Droid.Views.Calendar
{
    public partial class CalendarLayoutManager
    {
        private int computeScrollOffset(RecyclerView.State state, View startChild, View endChild)
        {
            if (cannotCalculateScrollVariables(state, startChild, endChild))
                return 0;

            var startChildPosition = GetPosition(startChild);
            var anchorHeight = (double)Math.Abs(orientationHelper.GetDecoratedEnd(startChild) - orientationHelper.GetDecoratedStart(startChild));

            return (int)Math.Round(startChildPosition * anchorHeight +
                                    (orientationHelper.StartAfterPadding - orientationHelper.GetDecoratedStart(startChild)));
        }

        private bool cannotCalculateScrollVariables(RecyclerView.State state, View startChild, View endChild)
            => ChildCount == 0 || state.ItemCount == 0 || startChild == null || endChild == null;
    }
}
