using FluentAssertions;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Tests.Integration.BaseTests;
using Xunit;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class ResetApiTokenDetectionTests : EndpointTestBase
    {
        [Fact, LogTestInfo]
        public async Task DoesNotInterpretForbiddenErrorAsResetApiToken()
        {
            var (api, user) = await SetupTestUser();

            Func<Task> shouldJustThrowForbiddenError = async () => await api.Workspaces.GetById(user.DefaultWorkspaceId - 1 ?? 0);

            shouldJustThrowForbiddenError.Should().Throw<ForbiddenException>();
        }

        [Fact, LogTestInfo]
        public async Task DetectsWhenTokenWasReset()
        {
            var (api, user) = await SetupTestUser();

            await User.ResetApiToken(user);
            Func<Task> shouldDetectResetApiToken = async () => await api.User.Get();

            shouldDetectResetApiToken.Should().Throw<UnauthorizedException>();
        }
    }
}
