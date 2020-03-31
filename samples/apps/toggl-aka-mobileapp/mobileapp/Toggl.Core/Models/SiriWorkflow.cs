using Toggl.Shared;

namespace Toggl.Core.Models
{
    [Preserve(AllMembers = true)]
    public class SiriWorkflow
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public string FileName { get; set; }
    }
}
