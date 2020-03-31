using FluentAssertions;
using System;
using Toggl.Core.Analytics;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.Services;
using Xunit;

namespace Toggl.Core.Tests.UI.Services
{
    public sealed class DeeplinkParserTests
    {
        public abstract class DeeplinkParserTest : BaseTest
        {
            protected DeeplinkParser DeeplinkParser { get; }

            public DeeplinkParserTest()
            {
                DeeplinkParser = new DeeplinkParser();
            }
        }

        public sealed class TheStartTimeEntryUrl : DeeplinkParserTest
        {
            [Fact, LogIfTooSlow]
            public void ParseStartTimeEntry()
            {
                var url = new Uri("toggl://tracker/timeEntry/start");
                var parameters = DeeplinkParser.Parse(url) as DeeplinkStartTimeEntryParameters;
                parameters.Should().NotBeNull();
            }

            [Fact, LogIfTooSlow]
            public void ParseStartTimeEntryWithParameters()
            {
                var url = new Uri("toggl://tracker/timeEntry/start?description=Hello%20world&startTime=\"2019-05-14T14:30:00Z\"&workspaceId=42&source=Siri");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkStartTimeEntryParameters;
                parameters.Should().NotBeNull();
                parameters.Description.Should().Be("Hello world");
                parameters.StartTime.Should().Be(new DateTimeOffset(2019, 5, 14, 14, 30, 0, TimeSpan.Zero));
                parameters.WorkspaceId.Should().Be(42);
                parameters.Source.Should().Be(TimeEntryStartOrigin.Siri);
            }
        }

        public sealed class TheContinueTimeEntryUrl : DeeplinkParserTest
        {
            [Fact, LogIfTooSlow]
            public void ParseContinueTimeEntry()
            {
                var url = new Uri("toggl://tracker/timeEntry/continue");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkContinueTimeEntryParameters;
                parameters.Should().NotBeNull();
            }
        }

        public sealed class TheStopTimeEntryUrl : DeeplinkParserTest
        {
            [Fact, LogIfTooSlow]
            public void ParseStopTimeEntry()
            {
                var url = new Uri("toggl://tracker/timeEntry/stop");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkStopTimeEntryParameters;
                parameters.Should().NotBeNull();
            }

            [Fact, LogIfTooSlow]
            public void ParseStopTimeEntryWithParameters()
            {
                var url = new Uri("toggl://tracker/timeEntry/stop?stopTime=2019-05-14T14:45:00Z");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkStopTimeEntryParameters;
                parameters.Should().NotBeNull();
                parameters.StopTime.Should().Be(new DateTimeOffset(2019, 5, 14, 14, 45, 0, TimeSpan.Zero));
            }

            [Fact, LogIfTooSlow]
            public void ParseStopTimeEntryFromSiri()
            {
                var url = new Uri("toggl://tracker/timeEntry/stop?stopTime=2019-05-14T14:45:00Z&source=Siri");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkStopTimeEntryParameters;
                parameters.Should().NotBeNull();
                parameters.StopTime.Should().Be(new DateTimeOffset(2019, 5, 14, 14, 45, 0, TimeSpan.Zero));
                parameters.Source.Should().Be(TimeEntryStopOrigin.Siri);
            }
        }

        public sealed class TheNewTimeEntryUrl : DeeplinkParserTest
        {
            [Fact, LogIfTooSlow]
            public void ParseNewTimeEntry()
            {
                var url = new Uri("toggl://tracker/timeEntry/new");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkNewTimeEntryParameters;
                parameters.Should().NotBeNull();
            }
        }

        public sealed class TheEditTimeEntryUrl : DeeplinkParserTest
        {
            [Fact, LogIfTooSlow]
            public void ParseEditTimeEntry()
            {
                var url = new Uri("toggl://tracker/timeEntry/edit?timeEntryId=1");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkEditTimeEntryParameters;
                parameters.Should().NotBeNull();
                parameters.TimeEntryId.Should().Be(1);
            }
        }

        public sealed class TheReportsUrl : DeeplinkParserTest
        {
            [Fact, LogIfTooSlow]
            public void ParseTheReportsUrl()
            {
                var url = new Uri("toggl://tracker/reports");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkShowReportsParameters;
                parameters.Should().NotBeNull();
            }

            [Fact, LogIfTooSlow]
            public void ParseTheReportsUrlWithParameters()
            {
                var url = new Uri("toggl://tracker/reports?workspaceId=1&startDate=\"2019-05-01T00:00:00Z\"&endDate=\"2019-05-14T00:00:00Z\"");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkShowReportsParameters;
                parameters.Should().NotBeNull();
                parameters.WorkspaceId.Should().Be(1);
                parameters.StartDate.Should().Be(new DateTimeOffset(2019, 5, 1, 0, 0, 0, TimeSpan.Zero));
                parameters.EndDate.Should().Be(new DateTimeOffset(2019, 5, 14, 0, 0, 0, TimeSpan.Zero));
            }
        }

        public sealed class TheCalendarUrl : DeeplinkParserTest
        {
            [Fact, LogIfTooSlow]
            public void ParseTheCalendarUrl()
            {
                var url = new Uri("toggl://tracker/calendar");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkShowCalendarParameters;
                parameters.Should().NotBeNull();
            }

            [Fact, LogIfTooSlow]
            public void ParseTheCalendarUrlWithParameters()
            {
                var url = new Uri("toggl://tracker/calendar?eventId=1");

                var parameters = DeeplinkParser.Parse(url) as DeeplinkShowCalendarParameters;
                parameters.Should().NotBeNull();
                parameters.EventId.Should().Be("1");
            }
        }

        public sealed class UnknownUrl : DeeplinkParserTest
        {
            [Theory, LogIfTooSlow]
            [InlineData("toggl://tracker/foo")]
            [InlineData("toggl://tracker/foo?source=Siri")]
            [InlineData("toggl://not-toggl/foo?source=Siri")]
            public void ParseTheCalendarUrl(string urlString)
            {
                var url = new Uri(urlString);

                var parameters = DeeplinkParser.Parse(url);
                parameters.GetType().Should().Be(typeof(DeeplinkParameters));
            }
        }
    }
}
