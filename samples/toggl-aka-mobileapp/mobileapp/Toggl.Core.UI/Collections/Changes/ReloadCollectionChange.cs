using System;

namespace Toggl.Core.UI.Collections.Changes
{
    [Obsolete("We are moving into using CollectionSection and per platform diffing")]
    public struct ReloadCollectionChange : ICollectionChange
    {
        public override string ToString() => $"Reload";
    }
}
