using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Models;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Shared;
using Toggl.Shared.Models;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class PreferencesApiTests
    {
        private static string[] validDateFormats =
        {
            "YYYY-MM-DD",
            "DD.MM.YYYY",
            "DD-MM-YYYY",
            "MM/DD/YYYY",
            "DD/MM/YYYY",
            "MM-DD-YYYY"
        };

        private static string[] validTimeOfDayFormats = { "H:mm", "h:mm A" };

        public sealed class TheGetMethod : AuthenticatedEndpointBaseTests<IPreferences>
        {
            protected override Task<IPreferences> CallEndpointWith(ITogglApi togglApi)
                => togglApi.Preferences.Get();

            [Fact, LogTestInfo]
            public async Task ReturnsValidDateFormat()
            {
                var (togglClient, user) = await SetupTestUser();

                var preferences = await togglClient.Preferences.Get();

                preferences.DateFormat.Localized.Should().BeOneOf(validDateFormats);
            }

            [Fact, LogTestInfo]
            public async Task ReturnsValidTimeOfDayFormat()
            {
                var (togglClient, user) = await SetupTestUser();

                var preferences = await togglClient.Preferences.Get();

                preferences.TimeOfDayFormat.Localized.Should().BeOneOf(validTimeOfDayFormats);
            }

            [Fact, LogTestInfo]
            public async Task ReturnsValidDurationFormat()
            {
                var (togglClient, user) = await SetupTestUser();

                var preferences = await togglClient.Preferences.Get();

                Enum.IsDefined(typeof(DurationFormat), preferences.DurationFormat).Should().BeTrue();
            }
        }

        public sealed class TheUpdateMethod : AuthenticatedPutEndpointBaseTests<IPreferences>
        {
            protected override Task<IPreferences> PrepareForCallingUpdateEndpoint(ITogglApi api)
                => api.Preferences.Get();

            protected override async Task<IPreferences> CallUpdateEndpoint(ITogglApi api, IPreferences entityToUpdate)
            {
                var entityWithUpdates = new Preferences(entityToUpdate);
                entityWithUpdates.CollapseTimeEntries = !entityWithUpdates.CollapseTimeEntries;

                await api.Preferences.Update(entityWithUpdates);
                return await api.Preferences.Get();
            }

            [Theory, LogTestInfo]
            [MemberData(nameof(SupportedTimeFormatsTestData))]
            public async Task SetsTimeOfDayFormat(string timeFormat)
            {
                var (togglClient, user) = await SetupTestUser();

                var oldPreferences = await togglClient.Preferences.Get();
                var newPreferences = new Preferences(oldPreferences);
                newPreferences.TimeOfDayFormat = TimeFormat.FromLocalizedTimeFormat(timeFormat);
                await togglClient.Preferences.Update(newPreferences);
                var updatedPreferences = await togglClient.Preferences.Get();

                updatedPreferences.TimeOfDayFormat.Should().Be(newPreferences.TimeOfDayFormat);
            }

            [Fact, LogTestInfo]
            public async Task ChangingTimeOfDayFormatWithWrongFormatThrows()
            {
                var constructor = typeof(TimeFormat).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
                var invalidFormat = (TimeFormat)constructor.Invoke(new object[] { "h:mm:ss", "H:mm:ss" });
                var (togglClient, user) = await SetupTestUser();

                var oldPreferences = await togglClient.Preferences.Get();
                var newPreferences = new Preferences(oldPreferences);
                newPreferences.TimeOfDayFormat = invalidFormat;

                Action updatingWithWongFormat = () => togglClient.Preferences.Update(newPreferences).Wait();

                updatingWithWongFormat.Should().Throw<BadRequestException>();
            }

            [Theory, LogTestInfo]
            [MemberData(nameof(SupportedDateFormatsTestData))]
            public async Task SetsDateFormat(string dateFormat)
            {
                var (togglClient, user) = await SetupTestUser();

                var oldPreferences = await togglClient.Preferences.Get();
                var newPreferences = new Preferences(oldPreferences);
                newPreferences.DateFormat = DateFormat.FromLocalizedDateFormat(dateFormat);
                await togglClient.Preferences.Update(newPreferences);
                var updatedPreferences = await togglClient.Preferences.Get();

                updatedPreferences.DateFormat.Should().Be(newPreferences.DateFormat);
            }

            [Fact, LogTestInfo]
            public async Task ChangingDateFormatWithWrongFormatThrows()
            {
                var (togglClient, user) = await SetupTestUser();

                var oldPreferences = await togglClient.Preferences.Get();
                var newPreferences = new Preferences(oldPreferences);
                newPreferences.DateFormat = DateFormat.FromLocalizedDateFormat("dd.mm.yyyy");

                Action updatingWithWongFormat = () => togglClient.Preferences.Update(newPreferences).Wait();

                updatingWithWongFormat.Should().Throw<BadRequestException>();
            }

            [Theory, LogTestInfo]
            [MemberData(nameof(SupportedDurationFormatsTestData))]
            public async Task ChangesDurationFormat(DurationFormat format)
            {
                var (togglClient, user) = await SetupTestUser();

                var oldPreferences = await togglClient.Preferences.Get();
                var newPreferences = new Preferences(oldPreferences);
                newPreferences.DurationFormat = format;
                await togglClient.Preferences.Update(newPreferences);
                var updatedPreferences = await togglClient.Preferences.Get();

                updatedPreferences.DurationFormat.Should().Be(newPreferences.DurationFormat);
            }

            [Fact, LogTestInfo]
            public async Task ChangingDurationFormatWithWrongFormatThrows()
            {
                var (togglClient, user) = await SetupTestUser();

                var oldPreferences = await togglClient.Preferences.Get();
                var newPreferences = new Preferences(oldPreferences);
                newPreferences.DurationFormat = (DurationFormat)4;

                Action updatingWithWongFormat = () => togglClient.Preferences.Update(newPreferences).Wait();

                updatingWithWongFormat.Should().Throw<BadRequestException>();
            }

            [Theory, LogTestInfo]
            [InlineData(true)]
            [InlineData(false)]
            public async Task ChangesCollapseTimeEntries(bool collapseTimeEntries)
            {
                var (togglClient, user) = await SetupTestUser();

                var oldPreferences = await togglClient.Preferences.Get();
                var newPreferences = new Preferences(oldPreferences);
                newPreferences.CollapseTimeEntries = collapseTimeEntries;
                await togglClient.Preferences.Update(newPreferences);
                var updatedPreferences = await togglClient.Preferences.Get();

                updatedPreferences.CollapseTimeEntries.Should().Be(newPreferences.CollapseTimeEntries);
            }

            public static IEnumerable<object[]> SupportedDateFormatsTestData
                => validDateFormats.Select(format => new[] { format });

            public static IEnumerable<object[]> SupportedTimeFormatsTestData
                => validTimeOfDayFormats.Select(format => new[] { format });

            public static IEnumerable<object[]> SupportedDurationFormatsTestData
                => Enum.GetValues(typeof(DurationFormat)).Cast<object>().Select(format => new[] { format });
        }
    }
}
