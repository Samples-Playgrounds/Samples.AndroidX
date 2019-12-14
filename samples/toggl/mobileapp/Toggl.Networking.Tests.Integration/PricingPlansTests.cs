using FluentAssertions;
using System;
using System.Linq;
using Toggl.Networking.Helpers;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Networking.Tests.Integration.Helper;
using Xunit;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class PricingPlansTests : EndpointTestBase
    {
        [Fact, LogTestInfo]
        public async void ThePricingPlansEnumsContainsAllAndOnlyTheAvailablePricingPlans()
        {
            var (_, user) = await SetupTestUser();

            var availablePlans = await WorkspaceHelper.GetAllAvailablePricingPlans(user);
            var convertedPlans = availablePlans.Select(plan => (PricingPlans)plan);

            Enum.GetNames(typeof(PricingPlans)).Length.Should().Be(availablePlans.Count);
            Enum.GetValues(typeof(PricingPlans)).Should().Contain(convertedPlans);
        }
    }
}
