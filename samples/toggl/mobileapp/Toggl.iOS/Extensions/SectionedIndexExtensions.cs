using Foundation;
using Toggl.Core.UI.Collections;

namespace Toggl.iOS.Extensions
{
    public static class SectionedIndexExtensions
    {
        public static NSIndexPath ToIndexPath(this SectionedIndex index)
            => NSIndexPath.FromRowSection(index.Row, index.Section);

        public static NSIndexPath[] ToIndexPaths(this SectionedIndex index)
            => new[] { index.ToIndexPath() };
    }
}
