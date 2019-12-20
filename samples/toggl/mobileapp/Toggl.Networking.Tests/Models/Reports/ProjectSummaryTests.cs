using Toggl.Networking.Models.Reports;
using Xunit;

namespace Toggl.Networking.Tests.Models.Reports
{
    public sealed class ProjectSummaryTests
    {
        public sealed class TheProjectSummaryModel
        {
            private string validJson
                => "{\"user_id\":23741667,\"project_id\":1427273,\"tracked_seconds\":9876,\"billable_seconds\":6543}";

            private ProjectSummary validSummary => new ProjectSummary
            {
                UserId = 23741667,
                ProjectId = 1427273,
                TrackedSeconds = 9876,
                BillableSeconds = 6543
            };

            [Fact, LogIfTooSlow]
            public void CanBeDeserialized()
            {
                SerializationHelper.CanBeDeserialized(validJson, validSummary);
            }

            [Fact, LogIfTooSlow]
            public void CanBeSerialized()
            {
                SerializationHelper.CanBeSerialized(validJson, validSummary);
            }
        }
    }
}
