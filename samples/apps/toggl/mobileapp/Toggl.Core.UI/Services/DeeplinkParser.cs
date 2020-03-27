using System;
using System.Collections.Generic;
using Toggl.Core.Analytics;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.Services
{
    public sealed class DeeplinkParser : IDeeplinkParser
    {
        public DeeplinkParameters Parse(Uri uri)
        {
            var path = uri.AbsolutePath;
            var args = uri.GetQueryParams();

            switch (path)
            {
                case ApplicationUrls.TimeEntry.Start.Path:
                    return parseStartTimeEntry(args);
                case ApplicationUrls.TimeEntry.ContinueLast.Path:
                    return parseContinueLast();
                case ApplicationUrls.TimeEntry.Stop.Path:
                    return parseStopTimeEntry(args);
                case ApplicationUrls.TimeEntry.Create.Path:
                    return parseCreateTimeEntry(args);
                case ApplicationUrls.TimeEntry.Update.Path:
                    return parseUpdateTimeEntry(args);
                case ApplicationUrls.TimeEntry.New.Path:
                    return parseNewTimeEntry(args);
                case ApplicationUrls.TimeEntry.Edit.Path:
                    return parseEditTimeEntry(args);
                case ApplicationUrls.Reports.Path:
                    return parseShowReports(args);
                case ApplicationUrls.Calendar.Path:
                    return parseShowCalendar(args);
                default:
                    return DeeplinkParameters.Other;
            }
        }

        private DeeplinkParameters parseStartTimeEntry(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/start?workspaceId=1&startTime="2019-04-18T09:35:47Z"&description=Toggl&projectId=1&taskId=1&tags=[1,2,3]&billable=true&source=Siri
            var workspaceId = args.GetValueAsLong(ApplicationUrls.TimeEntry.WorkspaceId);
            var startTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StartTime);
            var description = args.GetValueAsString(ApplicationUrls.TimeEntry.Description);
            var projectId = args.GetValueAsLong(ApplicationUrls.TimeEntry.ProjectId);
            var taskId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TaskId);
            var tags = args.GetValueAsLongs(ApplicationUrls.TimeEntry.Tags);
            var isBillable = args.GetValueAsBool(ApplicationUrls.TimeEntry.Billable);
            var source = args.GetValueAsEnumCase(ApplicationUrls.TimeEntry.Source, TimeEntryStartOrigin.Deeplink);

            return DeeplinkParameters.WithStartTimeEntry(
                description,
                startTime,
                workspaceId,
                projectId,
                taskId,
                tags,
                isBillable,
                source);
        }

        private DeeplinkParameters parseContinueLast()
        {
            // e.g: toggl://tracker/timeEntry/continue
            return DeeplinkParameters.WithContinueLast();
        }

        private DeeplinkParameters parseStopTimeEntry(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/stop?stopTime="2019-04-18T09:45:47Z"&source=Siri
            var stopTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StopTime);
            var source = args.GetValueAsEnumCase(ApplicationUrls.TimeEntry.Source, TimeEntryStopOrigin.Deeplink);

            return DeeplinkParameters.WithStopTimeEntry(stopTime, source);
        }

        private DeeplinkParameters parseCreateTimeEntry(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/create?workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&duration=600&description="Toggl"&projectId=1&taskId=1&tags=[]&billable=true&source=Siri
            var workspaceId = args.GetValueAsLong(ApplicationUrls.TimeEntry.WorkspaceId);
            var startTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StartTime);
            var stopTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StopTime);
            var duration = args.GetValueAsTimeSpan(ApplicationUrls.TimeEntry.Duration);
            var description = args.GetValueAsString(ApplicationUrls.TimeEntry.Description);
            var projectId = args.GetValueAsLong(ApplicationUrls.TimeEntry.ProjectId);
            var taskId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TaskId);
            var tags = args.GetValueAsLongs(ApplicationUrls.TimeEntry.Tags);
            var isBillable = args.GetValueAsBool(ApplicationUrls.TimeEntry.Billable) ?? false;
            var source = args.GetValueAsEnumCase(ApplicationUrls.TimeEntry.Source, TimeEntryStartOrigin.Deeplink);

            return DeeplinkParameters.WithCreateTimeEntry(description, startTime, stopTime, duration, workspaceId, projectId, taskId, tags, isBillable, source);
        }

        private DeeplinkParameters parseUpdateTimeEntry(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/update?timeEntryId=1workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&description="Toggl"&projectId=1&taskId=1&tags=[]&billable=true&source=Siri
            var timeEntryId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TimeEntryId);
            var workspaceId = args.GetValueAsLong(ApplicationUrls.TimeEntry.WorkspaceId);
            var startTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StartTime);
            var stopTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StopTime);
            var description = args.GetValueAsString(ApplicationUrls.TimeEntry.Description);
            var projectId = args.GetValueAsLong(ApplicationUrls.TimeEntry.ProjectId);
            var taskId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TaskId);
            var tags = args.GetValueAsLongs(ApplicationUrls.TimeEntry.Tags);
            var isBillable = args.GetValueAsBool(ApplicationUrls.TimeEntry.Billable) ?? false;

            if (!timeEntryId.HasValue)
                return DeeplinkParameters.Other;

            return DeeplinkParameters.WithUpdateTimeEntry(
                timeEntryId.Value,
                args.ContainsKey(ApplicationUrls.TimeEntry.Description), description,
                args.ContainsKey(ApplicationUrls.TimeEntry.StartTime), startTime,
                args.ContainsKey(ApplicationUrls.TimeEntry.StopTime), stopTime,
                args.ContainsKey(ApplicationUrls.TimeEntry.WorkspaceId), workspaceId,
                args.ContainsKey(ApplicationUrls.TimeEntry.ProjectId), projectId,
                args.ContainsKey(ApplicationUrls.TimeEntry.TaskId), taskId,
                args.ContainsKey(ApplicationUrls.TimeEntry.Tags), tags,
                args.ContainsKey(ApplicationUrls.TimeEntry.Billable), isBillable);
        }

        private DeeplinkParameters parseNewTimeEntry(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/new?workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&duration=600&description="Toggl"&projectId=1&tags=[]
            var workspaceId = args.GetValueAsLong(ApplicationUrls.TimeEntry.WorkspaceId);
            var startTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StartTime);
            var stopTime = args.GetValueAsDateTimeOffset(ApplicationUrls.TimeEntry.StopTime);
            var duration = args.GetValueAsTimeSpan(ApplicationUrls.TimeEntry.Duration);
            var description = args.GetValueAsString(ApplicationUrls.TimeEntry.Description);
            var projectId = args.GetValueAsLong(ApplicationUrls.TimeEntry.ProjectId);
            var tags = args.GetValueAsLongs(ApplicationUrls.TimeEntry.Tags);

            return DeeplinkParameters.WithNewTimeEntry(description, startTime, stopTime, duration, workspaceId, projectId, tags);
        }

        private DeeplinkParameters parseEditTimeEntry(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/timeEntry/edit?timeEntryId=1
            var timeEntryId = args.GetValueAsLong(ApplicationUrls.TimeEntry.TimeEntryId);

            if (!timeEntryId.HasValue)
                return DeeplinkParameters.Other;

            return DeeplinkParameters.WithEditTimeEntry(timeEntryId.Value);
        }

        private DeeplinkParameters parseShowReports(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/reports?workspaceId=1&startDate="2019-04-18T09:35:47Z"&endDate="2019-04-18T09:45:47Z"
            var workspaceId = args.GetValueAsLong(ApplicationUrls.Reports.WorkspaceId);
            var startDate = args.GetValueAsDateTimeOffset(ApplicationUrls.Reports.StartDate);
            var endDate = args.GetValueAsDateTimeOffset(ApplicationUrls.Reports.EndDate);

            return DeeplinkParameters.WithReports(workspaceId, startDate, endDate);
        }

        private DeeplinkParameters parseShowCalendar(Dictionary<string, string> args)
        {
            // e.g: toggl://tracker/calendar?eventId=1
            var eventId = args.GetValueAsString(ApplicationUrls.Calendar.EventId);
            return DeeplinkParameters.WithCalendar(eventId);
        }
    }
}
