using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using System.Linq;
using Toggl.Core.Reports;
using Xunit;

namespace Toggl.Core.Tests.Reports
{
    public sealed class ProjectSummaryReportTests
    {
        public sealed class TheConstructor
        {
            [Property]
            public void CalculatesThePercentageOfBillable(NonEmptyArray<NonNegativeInt> durations)
            {
                var segments = durations.Get.Select(duration => duration.Get)
                    .Select((index, duration) => getSegmentFromDurationAndIndex(index, duration))
                    .ToArray();
                var projectsNotSyncedCount = 0;
                var totalTrackedSeconds = segments.Select(s => s.TrackedTime.TotalSeconds).Sum();
                var billableSeconds = segments.Select(s => s.BillableSeconds).Sum();
                float expectedBillablePercentage =
                    (float)(totalTrackedSeconds > 0 ? (100.0f / totalTrackedSeconds) * billableSeconds : 0);

                var report = new ProjectSummaryReport(segments, projectsNotSyncedCount);

                report.BillablePercentage.Should().Be(expectedBillablePercentage);
            }

            [Property]
            public void CalculatesTheTotalAmountOfSeconds(NonEmptyArray<NonNegativeInt> durations)
            {
                var actualDurations = durations.Get.Select(duration => duration.Get);
                var segments = actualDurations
                    .Select((index, duration) => getSegmentFromDurationAndIndex(index, duration))
                    .ToArray();
                var projectsNotSyncedCount = 0;
                var expectedDuration = (float)segments.Select(s => s.TrackedTime.TotalSeconds).Sum();

                var report = new ProjectSummaryReport(segments, projectsNotSyncedCount);

                report.TotalSeconds.Should().Be(expectedDuration);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsZerosForEmptyListOfProjects()
            {
                var projectsNotSyncedCount = 0;
                var report = new ProjectSummaryReport(new ChartSegment[0], projectsNotSyncedCount);

                report.TotalSeconds.Should().Be(0);
                report.BillablePercentage.Should().Be(0);
            }

            private ChartSegment getSegmentFromDurationAndIndex(int index, int trackedSeconds)
                => new ChartSegment("", "", 0, trackedSeconds, index % 2 == 0 ? trackedSeconds : 0, "#FFFFFF");
        }
    }
}
