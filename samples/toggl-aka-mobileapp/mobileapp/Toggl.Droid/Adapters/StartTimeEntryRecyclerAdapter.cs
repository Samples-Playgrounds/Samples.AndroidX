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
    public sealed class StartTimeEntryRecyclerAdapter : BaseSectionedRecyclerAdapter<string, AutocompleteSuggestion>
    {
        public const int WorkspaceHeader = 0;
        public const int NoEntityFound = 1;
        public const int TimeEntrySuggestion = 2;
        public const int ProjectSuggestion = 3;
        public const int TagSuggestion = 4;
        public const int TaskSuggestion = 5;
        public const int CreateEntity = 6;
        public const int TimeEntrySuggestionWithPartialContent = 7;
        public const int QuerySymbolSuggestion = 8;

        private readonly ISubject<ProjectSuggestion> toggleTasksSubject = new Subject<ProjectSuggestion>();

        protected override HashSet<int> HeaderViewTypes { get; } = new HashSet<int> { WorkspaceHeader };

        protected override HashSet<int> ItemViewTypes { get; } = new HashSet<int>
        {
            NoEntityFound,
            TimeEntrySuggestion,
            ProjectSuggestion,
            TagSuggestion,
            TaskSuggestion,
            CreateEntity,
            TimeEntrySuggestionWithPartialContent,
            QuerySymbolSuggestion
        };

        public IObservable<ProjectSuggestion> ToggleTasks => toggleTasksSubject.AsObservable();

        public StartTimeEntryRecyclerAdapter()
        {
        }

        public StartTimeEntryRecyclerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override int SelectHeaderViewType(string headerItem) => WorkspaceHeader;

        protected override int SelectItemViewType(AutocompleteSuggestion item)
        {
            switch (item)
            {
                case TagSuggestion _:
                    return TagSuggestion;
                case QuerySymbolSuggestion _:
                    return QuerySymbolSuggestion;
                case TaskSuggestion _:
                    return TaskSuggestion;
                case NoEntityInfoMessage _:
                    return NoEntityFound;
                case ProjectSuggestion _:
                    return ProjectSuggestion;
                case TimeEntrySuggestion timeEntrySuggestion when timeEntrySuggestionHasPartialContent(timeEntrySuggestion):
                    return TimeEntrySuggestionWithPartialContent;
                case TimeEntrySuggestion _:
                    return TimeEntrySuggestion;
                default:
                    return CreateEntity;
            }

            bool timeEntrySuggestionHasPartialContent(TimeEntrySuggestion timeEntrySuggestion)
                => string.IsNullOrEmpty(timeEntrySuggestion.Description) || !timeEntrySuggestion.HasProject;
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
                case QuerySymbolSuggestion:
                    return new SimpleTextViewHolder<AutocompleteSuggestion>(
                        inflater.Inflate(Resource.Layout.StartTimeEntryActivityQuerySymbolCell, parent, false),
                        Resource.Id.HintLabel,
                        suggestion => (suggestion as QuerySymbolSuggestion).FormattedDescription()
                    );

                case NoEntityFound:
                    // This view type of suggestion is ignored on Droid
                    return new SimpleTextViewHolder<AutocompleteSuggestion>(
                        inflater.Inflate(Resource.Layout.StartTimeEntryActivityNoEntityCell, parent, false),
                        Resource.Id.TextView,
                        suggestion => ""
                    );

                case TimeEntrySuggestion:
                    return new TimeEntrySuggestionViewHolder(
                        inflater.Inflate(Resource.Layout.StartTimeEntryActivityTimeEntryCell, parent, false)
                    );

                case ProjectSuggestion:
                    return new ProjectSuggestionViewHolder(
                        inflater.Inflate(Resource.Layout.StartTimeEntryActivityProjectCell, parent, false),
                        toggleTasksSubject
                    );

                case TagSuggestion:
                    return new SimpleTextViewHolder<AutocompleteSuggestion>(
                        inflater.Inflate(Resource.Layout.StartTimeEntryActivityTagCell, parent, false),
                        Resource.Id.TagLabel,
                        suggestion => (suggestion as TagSuggestion).Name
                    );

                case TaskSuggestion:
                    return new SimpleTextViewHolder<AutocompleteSuggestion>(
                        inflater.Inflate(Resource.Layout.StartTimeEntryActivityTaskCell, parent, false),
                        Resource.Id.TaskLabel,
                        suggestion => (suggestion as TaskSuggestion).Name
                    );

                case CreateEntity:
                    return new SimpleTextViewHolder<AutocompleteSuggestion>(
                        inflater.Inflate(Resource.Layout.AbcCreateEntityCell, parent, false),
                        Resource.Id.CreationLabel,
                        suggestion => (suggestion as CreateEntitySuggestion).CreateEntityMessage
                    );

                case TimeEntrySuggestionWithPartialContent:
                    return new TimeEntrySuggestionViewHolder(
                        inflater.Inflate(Resource.Layout.StartTimeEntryActivityTimeEntryWithPartialContentCell, parent, false)
                    );

                default:
                    throw new InvalidOperationException($"Invalid view type {viewType}");
            }
        }
    }
}
