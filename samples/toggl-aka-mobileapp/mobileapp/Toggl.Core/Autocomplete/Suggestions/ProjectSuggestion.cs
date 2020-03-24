using System.Collections.Generic;
using System;
using System.Linq;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.Autocomplete.Suggestions
{
    [Preserve(AllMembers = true)]
    public sealed class ProjectSuggestion : AutocompleteSuggestion
    {
        public const long NoProjectId = 0;

        public static ProjectSuggestion NoProject(long workspaceId, string workspaceName)
            => new ProjectSuggestion(workspaceId, workspaceName);

        public static IEnumerable<ProjectSuggestion> FromProjects(
            IEnumerable<IThreadSafeProject> projects
        ) => projects.Select(project => new ProjectSuggestion(project));

        public long ProjectId { get; }

        public int NumberOfTasks { get; } = 0;

        public IList<TaskSuggestion> Tasks { get; }

        public bool HasTasks => Tasks?.Count > 0;

        public string ProjectName { get; } = "";

        public string ClientName { get; } = "";

        public string ProjectColor { get; }

        public bool TasksVisible { get; set; }

        public bool Selected { get; set; }

        private ProjectSuggestion(long workspaceId, string workspaceName)
        {
            ProjectId = NoProjectId;
            ClientName = "";
            ProjectColor = Helper.Colors.NoProject;
            ProjectName = Resources.NoProject;
            WorkspaceId = workspaceId;
            WorkspaceName = workspaceName;
        }

        public ProjectSuggestion(IThreadSafeProject project)
        {
            ProjectId = project.Id;
            ProjectName = project.Name;
            ProjectColor = project.Color;
            NumberOfTasks = project.Tasks?.Count() ?? 0;
            ClientName = project.Client?.Name ?? "";
            WorkspaceId = project.WorkspaceId;
            WorkspaceName = project.Workspace?.Name ?? "";
            Tasks = project.Tasks?.Select(task => new TaskSuggestion(Task.From(task))).ToList() ?? new List<TaskSuggestion>();
        }

        public override int GetHashCode()
            => HashCode.Combine(ProjectName, ProjectColor, ClientName);
    }

    public static class ProjectSuggestionExtensions
    {
        public static string FormattedNumberOfTasks(this ProjectSuggestion projectSuggestion)
        {
            switch (projectSuggestion.NumberOfTasks)
            {
                case 0:
                    return "";
                case 1:
                    return string.Format(Resources.NumberOfTasksSingular, projectSuggestion.NumberOfTasks);
                default:
                    return string.Format(Resources.NumberOfTasksPlural, projectSuggestion.NumberOfTasks);
            }
        }
    }
}
