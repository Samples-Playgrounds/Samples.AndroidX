using Foundation;
using System.Collections.Generic;
using Toggl.Core.UI.Calendar;

namespace Toggl.iOS.Views.Calendar
{
    public interface ICalendarCollectionViewLayoutDataSource
    {
        IEnumerable<NSIndexPath> IndexPathsOfCalendarItemsBetweenHours(int minHour, int maxHour);

        CalendarItemLayoutAttributes LayoutAttributesForItemAtIndexPath(NSIndexPath indexPath);

        NSIndexPath IndexPathForEditingItem();

        NSIndexPath IndexPathForRunningTimeEntry();
    }
}
