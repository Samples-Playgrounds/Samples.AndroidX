namespace Toggl.Shared
{
    public static class ApplicationUrls
    {
        public const string Scheme = "toggl";

        public const string Host = "tracker";

        public static class Main
        {
            public const string Path = "/main";

            public static string Default
                => $"{Scheme}://{Host}{Path}";
        }

        public static class TimeEntry
        {
            public const string TimeEntryId = "timeEntryId";
            public const string WorkspaceId = "workspaceId";
            public const string StartTime = "startTime";
            public const string StopTime = "stopTime";
            public const string Duration = "duration";
            public const string Description = "description";
            public const string ProjectId = "projectId";
            public const string TaskId = "taskId";
            public const string Tags = "tags";
            public const string Billable = "billable";
            public const string Source = "source";

            public static class Start
            {
                public const string Path = "/timeEntry/start";

                public static string Default
                    => $"{Scheme}://{Host}{Path}";

                public static string WithDescription(string description)
                    => $"{Default}?description={description}";
            }

            public static class Stop
            {
                public const string Path = "/timeEntry/stop";

                public static string Default
                    => $"{Scheme}://{Host}{Path}";

                public static string FromSiri
                    => $"{Scheme}://{Host}{Path}?source=Siri";
            }

            public static class Create
            {
                public const string Path = "/timeEntry/create";

                public static string Default
                    => $"{Scheme}://{Host}{Path}";
            }

            public static class Update
            {
                public const string Path = "/timeEntry/update";

                public static string Default
                    => $"{Scheme}://{Host}{Path}";
            }

            public static class ContinueLast
            {
                public const string Path = "/timeEntry/continue";

                public static string Default
                    => $"{Scheme}://{Host}{Path}";
            }

            public static class New
            {
                public const string Path = "/timeEntry/new";

                public static string Default
                    => $"{Scheme}://{Host}{Path}";
            }

            public static class Edit
            {
                public const string Path = "/timeEntry/edit";

                public static string Default
                    => $"{Scheme}://{Host}{Path}";
            }
        }

        public static class Reports
        {
            public const string Path = "/reports";

            public const string WorkspaceId = "workspaceId";
            public const string StartDate = "startDate";
            public const string EndDate = "endDate";

            public static string Default
                => $"{Scheme}://{Host}{Path}";
        }

        public static class Calendar
        {
            public const string Path = "/calendar";

            public const string EventId = "eventId";

            public static string Default
                => $"{Scheme}://{Host}{Path}";

            public static string ForId(string eventId)
                => $"{Scheme}://{Host}{Path}?eventId={eventId}";
        }
    }
}
