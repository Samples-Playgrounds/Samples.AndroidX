using Android.Graphics;

namespace Toggl.Droid.Autocomplete
{
    public sealed class ProjectTokenSpan : TokenSpan
    {
        public long ProjectId { get; }

        public string ProjectName { get; }

        public string ProjectColor { get; }

        public long? TaskId { get; set; }

        public string TaskName { get; set; }

        public ProjectTokenSpan(long projectId, string projectName, string projectColor, long? taskId, string taskName)
            : base(Color.ParseColor(projectColor), Color.White, false)
        {
            ProjectId = projectId;
            ProjectName = projectName;
            ProjectColor = projectColor;
            TaskId = taskId;
            TaskName = taskName ?? string.Empty;
        }
    }
}
