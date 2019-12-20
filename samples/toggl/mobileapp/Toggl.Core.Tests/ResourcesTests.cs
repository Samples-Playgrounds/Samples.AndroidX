using FluentAssertions;
using System.Globalization;
using System.Resources;
using System.Text.RegularExpressions;
using Xunit;

namespace Toggl.Core.Tests
{
    public sealed class ResourcesTests
    {
        [Fact, LogIfTooSlow]
        public void TheTermsOfServiceDialogAlwaysContainsPlaceholders()
        {
            var placeholdersToCheck = new[] { 0, 1 };
            var resourceManager = new ResourceManager(
                "Toggl.Shared.Resources",
                typeof(Shared.Resources).Assembly);

            foreach (var code in Helper.Constants.SupportedTwoLettersLanguageCodes)
            {
                var culture = new CultureInfo(code);

                var dialogMessage = resourceManager.GetString("TermsOfServiceDialogMessage", culture);

                foreach (var placeholder in placeholdersToCheck)
                {
                    dialogMessage.Should().Contain($"{{{placeholder}}}");
                    Regex.Matches(dialogMessage, $"\\{{{placeholder}\\}}").Count.Should().Be(1);
                }
            }
        }
    }
}
