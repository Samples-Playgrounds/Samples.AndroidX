using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Toggl.Shared;

namespace Toggl.Core.UI.Collections
{
    [Preserve(AllMembers = true)]
    [Obsolete("We are moving into using CollectionSection and per platform diffing")]
    public class WorkspaceGroupedCollection<T> : ObservableCollection<T>
    {
        public long WorkspaceId { get; }
        public string WorkspaceName { get; }

        public WorkspaceGroupedCollection()
        {
        }

        public WorkspaceGroupedCollection(string workspaceName, long workspaceId, IEnumerable<T> items)
        {
            Ensure.Argument.IsNotNull(items, nameof(items));
            Ensure.Argument.IsNotNull(workspaceName, nameof(workspaceName));

            WorkspaceName = workspaceName;
            WorkspaceId = workspaceId;

            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
}
