using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Sync;
using Toggl.Core.Sync.States.Pull;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared.Models;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.Pull
{
    public sealed class ResetSinceParamsStateTests
    {
        private readonly ISinceParameterRepository sinceParameterRepository = Substitute.For<ISinceParameterRepository>();

        [Theory, LogIfTooSlow]
        [ConstructorData]
        public void ThrowsIfAnyOfTheArgumentsIsNull(bool useSinceParameterRepository)
        {
            var sinceParameterRepository =
                useSinceParameterRepository ? Substitute.For<ISinceParameterRepository>() : null;

            Action tryingToConstructWithNulls = () => new ResetSinceParamsState(sinceParameterRepository);

            tryingToConstructWithNulls.Should().Throw<ArgumentNullException>();
        }

        [Fact, LogIfTooSlow]
        public async Task ResetsSinceParameterRepositoryBeforePersisting()
        {
            var workspaces = new[]
            {
                new MockWorkspace { Id = 1 },
                new MockWorkspace { Id = 2 },
            };

            var state = new ResetSinceParamsState(sinceParameterRepository);
            await state.Start(workspaces);

            sinceParameterRepository.Received().Reset();
        }

        [Fact, LogIfTooSlow]
        public async Task ReturnsTheNewWorkspacesUntouched()
        {
            var workspaces = new[]
            {
                new MockWorkspace { Id = 1 },
                new MockWorkspace { Id = 2 },
            };
            var state = new ResetSinceParamsState(sinceParameterRepository);
            var transition = await state.Start(workspaces);
            var parameter = ((Transition<IEnumerable<IWorkspace>>)transition).Parameter;

            parameter.Should().BeSameAs(workspaces);
        }
    }
}
