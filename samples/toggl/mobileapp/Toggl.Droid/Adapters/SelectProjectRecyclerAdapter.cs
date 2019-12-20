using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Droid.ViewHolders;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Adapters
{
    public sealed class SelectProjectRecyclerAdapter : BaseSectionedRecyclerAdapter<string, AutocompleteSuggestion>
    {
        private const int workspaceHeader = 0;
        private const int projectSuggestionViewType = 1;
        private const int taskSuggestionViewType = 2;
        private const int createEntitySuggestionViewType = 3;

        private readonly ISubject<ProjectSuggestion> toggleTasksSubject = new Subject<ProjectSuggestion>();

        protected override HashSet<int> HeaderViewTypes { get; } = new HashSet<int> { workspaceHeader };

        protected override HashSet<int> ItemViewTypes { get; } = new HashSet<int>
        {
            projectSuggestionViewType,
            taskSuggestionViewType,
            createEntitySuggestionViewType
        };

        public IObservable<ProjectSuggestion> ToggleTasks => toggleTasksSubject.AsObservable();

        public SelectProjectRecyclerAdapter()
        {
        }

        public SelectProjectRecyclerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override int SelectHeaderViewType(string headerItem) => workspaceHeader;

        protected override int SelectItemViewType(AutocompleteSuggestion item)
        {
            switch (item)
            {
                case ProjectSuggestion _:
                    return projectSuggestionViewType;
                case TaskSuggestion _:
                    return taskSuggestionViewType;
                case CreateEntitySuggestion _:
                    return createEntitySuggestionViewType;
                default:
                    throw new Exception("Invalid item type");
            }
        }

        protected override BaseRecyclerViewHolder<string> CreateHeaderViewHolder(LayoutInflater inflater, ViewGroup parent, int viewType)
        {
            var inflatedView = inflater.Inflate(Resource.Layout.StartTimeEntryActivityWorkspaceHeader, parent, false);
            return new SimpleTextViewHolder<string>(inflatedView, Resource.Id.WorkspaceHeaderTextView, CommonFunctions.Identity);
        }

        protected override BaseRecyclerViewHolder<AutocompleteSuggestion> CreateItemViewHolder(LayoutInflater inflater, ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case projectSuggestionViewType:
                    var inflatedView = inflater.Inflate(Resource.Layout.SelectProjectActivityProjectCell, parent, false);
                    return new ProjectSuggestionViewHolder(inflatedView, toggleTasksSubject);
                case taskSuggestionViewType:
                    return new SimpleTextViewHolder<AutocompleteSuggestion>(
                        inflater.Inflate(Resource.Layout.SelectProjectActivityTaskCell, parent, false),
                        Resource.Id.TaskNameLabel,
                        suggestion => (suggestion as TaskSuggestion).Name
                    );
                case createEntitySuggestionViewType:
                    return new SimpleTextViewHolder<AutocompleteSuggestion>(
                        inflater.Inflate(Resource.Layout.AbcCreateEntityCell, parent, false),
                        Resource.Id.CreationLabel,
                        suggestion => (suggestion as CreateEntitySuggestion).CreateEntityMessage
                    );
                default:
                    throw new Exception("Unsupported view type");
            }
        }
    }
}
