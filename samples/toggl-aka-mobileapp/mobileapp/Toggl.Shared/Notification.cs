using System;

namespace Toggl.Shared
{
    public struct Notification
    {
        public string Id { get; }

        public string Title { get; }

        public string Description { get; }

        public DateTimeOffset ScheduledTime { get; }

        public Notification(string id, string title, string description, DateTimeOffset scheduledTime)
        {
            Ensure.Argument.IsNotNullOrEmpty(id, nameof(id));
            Ensure.Argument.IsNotNullOrEmpty(title, nameof(title));

            Id = id;
            Title = title;
            Description = description;
            ScheduledTime = scheduledTime;
        }
    }
}
