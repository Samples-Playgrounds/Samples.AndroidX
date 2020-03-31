using System;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.Transformations
{
    public class DurationAndFormatToString
    {
        public static string Convert(TimeSpan duration, DurationFormat format)
        {
            return duration.ToFormattedString(format);
        }
    }
}
