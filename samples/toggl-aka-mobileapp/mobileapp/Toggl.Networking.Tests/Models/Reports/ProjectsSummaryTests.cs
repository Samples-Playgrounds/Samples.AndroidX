using System.Collections.Generic;
using Toggl.Networking.Models.Reports;
using Xunit;

namespace Toggl.Networking.Tests.Models.Reports
{
    public sealed class ProjectsSummaryTests
    {
        public sealed class TheProjectsSummaryModel
        {
            private string validJson
                => "[{\"user_id\":23741667,\"project_id\":null,\"tracked_seconds\":9876,\"billable_seconds\":6543},"
                    + "{\"user_id\":23741667,\"project_id\":1427273,\"tracked_seconds\":5678,\"billable_seconds\":null},"
                    + "{\"user_id\":23741667,\"project_id\":1427273,\"tracked_seconds\":4598,\"billable_seconds\":56}]";

            private List<ProjectSummary> validSummary => new List<ProjectSummary>
            {
                new ProjectSummary
                {
                    UserId = 23741667,
                    ProjectId = null,
                    TrackedSeconds = 9876,
                    BillableSeconds = 6543
                },
                new ProjectSummary
                {
                    UserId = 23741667,
                    ProjectId = 1427273,
                    TrackedSeconds = 5678,
                    BillableSeconds = null
                },
                new ProjectSummary
                {
                    UserId = 23741667,
                    ProjectId = 1427273,
                    TrackedSeconds = 4598,
                    BillableSeconds = 56
                }
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
