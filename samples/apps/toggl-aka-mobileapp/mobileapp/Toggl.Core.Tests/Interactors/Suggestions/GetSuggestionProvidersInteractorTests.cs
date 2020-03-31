using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Interactors.Suggestions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Suggestions
{
    public sealed class GetSuggestionProvidersInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useTimeService,
                bool useCalendarService,
                bool useInteractorFactory)
            {
                Action createInstance = () => new GetSuggestionProvidersInteractor(
                    3,
                    useDataSource ? DataSource : null,
                    useTimeService ? TimeService : null,
                    useCalendarService ? CalendarService : null,
                    useInteractorFactory ? InteractorFactory : null);

                createInstance.Should().Throw<ArgumentNullException>();
            }

            [Theory, LogIfTooSlow]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(10)]
            [InlineData(-100)]
            [InlineData(256)]
            public void ThrowsIfTheCountIsOutOfRange(int count)
            {
                Action createInstance = () => new GetSuggestionProvidersInteractor(
                    count,
                    DataSource,
                    TimeService,
                    CalendarService,
                    InteractorFactory);

                createInstance.Should().Throw<ArgumentException>();
            }
        }
    }
}
