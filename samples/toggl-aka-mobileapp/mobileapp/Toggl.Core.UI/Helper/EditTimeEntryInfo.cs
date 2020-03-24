using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Analytics;

namespace Toggl.Core.UI.Helper
{
    public struct EditTimeEntryInfo
    {
        public long[] Ids { get; }

        public EditTimeEntryOrigin Origin { get; }

        public EditTimeEntryInfo(EditTimeEntryOrigin origin, params long[] ids)
        {
            Ids = ids;
            Origin = origin;
        }
    }
}
