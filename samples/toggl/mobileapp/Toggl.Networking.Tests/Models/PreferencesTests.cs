using FluentAssertions;
using Toggl.Networking.Models;
using Toggl.Shared;
using Xunit;

namespace Toggl.Networking.Tests.Models
{
    public sealed class PreferencesTests
    {
        private string validJson
            => "{\"timeofday_format\":\"h:mm A\",\"date_format\":\"YYYY-MM-DD\",\"duration_format\":\"improved\",\"CollapseTimeEntries\":true}";

        private Preferences validPreferences => new Preferences
        {
            TimeOfDayFormat = TimeFormat.FromLocalizedTimeFormat("h:mm A"),
            DateFormat = DateFormat.FromLocalizedDateFormat("YYYY-MM-DD"),
            DurationFormat = DurationFormat.Improved,
            CollapseTimeEntries = true
        };

        [Fact, LogIfTooSlow]
        public void HasConstructorWhichCopiesValuesFromInterfaceToTheNewInstance()
        {
            var clonedObject = new Preferences(validPreferences);

            clonedObject.Should().NotBeSameAs(validPreferences);
            clonedObject.Should().BeEquivalentTo(validPreferences, options => options.IncludingProperties());
        }

        [Fact, LogIfTooSlow]
        public void CanBeDeserialized()
        {
            SerializationHelper.CanBeDeserialized(validJson, validPreferences);
        }

        [Fact, LogIfTooSlow]
        public void CanBeSerialized()
        {
            SerializationHelper.CanBeSerialized(validJson, validPreferences);
        }
    }
}
