using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DTOs;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.UI.Parameters;

namespace Toggl.Core.UI.Extensions
{
    public static class DeeplinkParametersExtensions
    {
        public static async Task Start(this DeeplinkStartTimeEntryParameters parameters, IInteractorFactory interactorFactory, ITimeService timeService)
        {
            var workspaceId = parameters.WorkspaceId;
            if (!workspaceId.HasValue)
            {
                var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();
                workspaceId = defaultWorkspace.Id;
            }

            var description = parameters.Description ?? string.Empty;
            var startTime = parameters.StartTime ?? timeService.CurrentDateTime;
            var projectId = parameters.ProjectId;
            var taskId = parameters.TaskId;
            var tagIds = parameters.TagIds;
            var isBillable = parameters.IsBillable ?? false;
            var source = parameters.Source ?? TimeEntryStartOrigin.Deeplink;

            var prototype = description.AsTimeEntryPrototype(startTime, workspaceId.Value, null, projectId, taskId, tagIds, isBillable);
            await interactorFactory.CreateTimeEntry(prototype, source).Execute();
        }

        public static async Task Continue(this DeeplinkContinueTimeEntryParameters parameters, IInteractorFactory interactorFactory)
        {
            await interactorFactory.ContinueMostRecentTimeEntry().Execute();
        }

        public static async Task Stop(this DeeplinkStopTimeEntryParameters parameters, IInteractorFactory interactorFactory, ITimeService timeService)
        {
            var stopTime = parameters.StopTime ?? timeService.CurrentDateTime;
            var source = parameters.Source ?? TimeEntryStopOrigin.Deeplink;

            await interactorFactory.StopTimeEntry(stopTime, source).Execute();
        }

        public static async Task Create(this DeeplinkCreateTimeEntryParameters parameters, IInteractorFactory interactorFactory)
        {
            var workspaceId = parameters.WorkspaceId;
            if (!workspaceId.HasValue)
            {
                var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();
                workspaceId = defaultWorkspace.Id;
            }

            var startTime = parameters.StartTime;

            if (!startTime.HasValue)
                return;

            var stopTime = parameters.StopTime;
            var duration = parameters.Duration;

            if (!duration.HasValue && !stopTime.HasValue)
                return;

            if (!duration.HasValue)
            {
                duration = stopTime.Value - startTime.Value;
            }

            var description = parameters.Description ?? string.Empty;
            var projectId = parameters.ProjectId;
            var taskId = parameters.TaskId;
            var tagIds = parameters.TagIds;
            var isBillable = parameters.IsBillable ?? false;
            var source = parameters.StartSource ?? TimeEntryStartOrigin.Deeplink;

            var prototype = description.AsTimeEntryPrototype(startTime.Value, workspaceId.Value, duration, projectId, taskId, tagIds, isBillable);
            await interactorFactory.CreateTimeEntry(prototype, source).Execute();
        }

        public static async Task Update(this DeeplinkUpdateTimeEntryParameters parameters, IInteractorFactory interactorFactory, ITimeService timeService)
        {
            var timeEntryId = parameters.TimeEntryId;
            var workspaceId = parameters.WorkspaceId;
            var startTime = parameters.StartTime ?? timeService.CurrentDateTime;
            var stopTime = parameters.StopTime;
            var description = parameters.Description;
            var projectId = parameters.ProjectId;
            var taskId = parameters.TaskId;
            var tagIds = parameters.TagIds;
            var isBillable = parameters.IsBillable ?? false;

            var timeEntryToUpdate = await interactorFactory.GetTimeEntryById(timeEntryId).Execute();

            if (!workspaceId.HasValue)
            {
                var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();
                workspaceId = defaultWorkspace.Id;
            }

            var newWorkspaceId = parameters.HasWorkspaceId ? workspaceId.Value : timeEntryToUpdate.WorkspaceId;
            var newStartTime = parameters.HasStartTime ? startTime : timeEntryToUpdate.Start;
            var newStopTime = parameters.HasStopTime ? stopTime : timeEntryToUpdate.StopTime();
            var newDescription = parameters.HasDescription ? description : timeEntryToUpdate.Description;
            var newProjectId = parameters.HasProjectId ? projectId : timeEntryToUpdate.ProjectId;
            var newTaskId = parameters.HasTaskId ? taskId : timeEntryToUpdate.TaskId;
            var newTagIds = parameters.HasTagIds ? tagIds : timeEntryToUpdate.TagIds;
            var newIsBillable = parameters.HasIsBillable ? isBillable : timeEntryToUpdate.Billable;

            var editTimeEntryDto = new EditTimeEntryDto
            {
                Id = timeEntryId,
                WorkspaceId = newWorkspaceId,
                StartTime = newStartTime,
                StopTime = newStopTime,
                Description = newDescription,
                ProjectId = newProjectId,
                TaskId = newTaskId,
                TagIds = newTagIds,
                Billable = newIsBillable,
            };

            await interactorFactory.UpdateTimeEntry(editTimeEntryDto).Execute();
        }

        public static StartTimeEntryParameters ToStartTimeEntryParameters(this DeeplinkNewTimeEntryParameters parameters, ITimeService timeService)
        {
            var workspaceId = parameters.WorkspaceId;
            var startTime = parameters.StartTime ?? timeService.CurrentDateTime;
            var duration = parameters.Duration;
            var description = parameters.Description;
            var projectId = parameters.ProjectId;
            var tags = parameters.TagIds;

            return new StartTimeEntryParameters(
                startTime,
                string.Empty,
                duration,
                workspaceId,
                description,
                projectId,
                tags
            );
        }
    }
}
