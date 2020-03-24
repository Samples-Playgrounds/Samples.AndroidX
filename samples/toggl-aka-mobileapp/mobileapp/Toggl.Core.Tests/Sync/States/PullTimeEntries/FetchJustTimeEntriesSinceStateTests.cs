using FluentAssertions;
using FsCheck.Xunit;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Sync;
using Toggl.Core.Sync.States;
using Toggl.Core.Sync.States.PullTimeEntries;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Networking;
using Toggl.Shared.Models;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.Sync.States.PullTimeEntries
{
    public sealed class FetchJustTimeEntriesSinceStateTests
    {
        public sealed class TheStartMethod
        {
            private readonly ISinceParameterRepository sinceParameters;
            private readonly ITogglApi api;
            private readonly ITimeService timeService;
            private readonly ILeakyBucket leakyBucket;
            private readonly IRateLimiter rateLimiter;
            private readonly FetchJustTimeEntriesSinceState state;
            private readonly DateTimeOffset now = new DateTimeOffset(2017, 02, 15, 13, 50, 00, TimeSpan.Zero);

            public TheStartMethod()
            {
                sinceParameters = Substitute.For<ISinceParameterRepository>();
                api = Substitute.For<ITogglApi>();
                timeService = Substitute.For<ITimeService>();
                leakyBucket = Substitute.For<ILeakyBucket>();
                leakyBucket.TryClaimFreeSlots(Arg.Any<int>(), out _).Returns(true);
                rateLimiter = Substitute.For<IRateLimiter>();
                rateLimiter.WaitForFreeSlot().Returns(Observable.Return(Unit.Default));
                timeService.CurrentDateTime.Returns(now);
                state = new FetchJustTimeEntriesSinceState(api, sinceParameters, timeService, leakyBucket, rateLimiter);
            }

            [Fact, LogIfTooSlow]
            public void EmitsTransitionToFetchStartedResult()
            {
                var transition = state.Start().SingleAsync().Wait();

                transition.Result.Should().Be(state.Done);
            }

            [Fact, LogIfTooSlow]
            public void MakesNoApiCallsBeforeSubscription()
            {
                state.Start();

                var t = api.DidNotReceive().TimeEntries;
            }

            [Fact, LogIfTooSlow]
            public void SendsRequestToFetchTimeEntriesWhenRateLimiterAllocatesASlotAfterSubscription()
            {
                var scheduler = new TestScheduler();
                var delay = TimeSpan.FromSeconds(1);
                rateLimiter.WaitForFreeSlot().Returns(Observable.Return(Unit.Default).Delay(delay, scheduler));

                api.TimeEntries
                    .GetAll(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                    .ReturnsTaskOf(null);

                state.Start().Subscribe();

                api.TimeEntries.DidNotReceive().GetAll(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>());
                api.TimeEntries.DidNotReceive().GetAllSince(Arg.Any<DateTimeOffset>());

                scheduler.AdvanceBy(delay.Ticks);

                api.TimeEntries.Received().GetAll(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>());
            }

            [Property]
            public void MakesCorrectCallWithSinceThresholdsWhenSinceIsLessThanTwoMonthsInThePast(int seed)
            {
                var rnd = new Random(seed);
                var percent = rnd.NextDouble();

                var now = timeService.CurrentDateTime;
                var twoMonths = (now.AddMonths(2) - now);
                var seconds = twoMonths.TotalSeconds * percent;
                var since = now.AddSeconds(-seconds);

                sinceParameters.Get<ITimeEntry>().Returns(since);

                state.Start().Wait();

                api.TimeEntries.Received().GetAllSince(since);
            }

            [Fact, LogIfTooSlow]
            public void MakesApiCallsWithoutTheSinceParameterWhenTheThresholdIsMoreThanTwoMonthsInThePast()
            {
                var now = timeService.CurrentDateTime;

                sinceParameters.Get<ITimeEntry>().Returns(now.AddMonths(-8));

                state.Start().Wait();

                api.TimeEntries.Received().GetAll(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>());
            }

            [Fact, LogIfTooSlow]
            public void MakesCorrectCallWithoutSinceThreshold()
            {
                state.Start().Wait();

                api.TimeEntries.Received().GetAll(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>());
            }

            [Fact, LogIfTooSlow]
            public void ReturnsReplayingApiCallObservables()
            {
                api.TimeEntries
                    .GetAll(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                    .ReturnsTaskOf(null);

                var transition = (Transition<IFetchObservables>)state.Start().SingleAsync().Wait();

                var observables = transition.Parameter;
                observables.GetList<ITimeEntry>().SingleAsync().Wait().Should().BeNull();
            }

            [Fact, LogIfTooSlow]
            public async Task FetchesTwoMonthsOfTimeEntriesDataIncludingTwoDaysAfterNow()
            {
                sinceParameters.Get<ITimeEntry>().Returns(now.AddMonths(-8));

                await state.Start().SingleAsync();

                var max = TimeSpan.FromDays(62);
                var min = TimeSpan.FromDays(59);

                await api.TimeEntries.Received().GetAll(
                    Arg.Is<DateTimeOffset>(start => min <= now - start && now - start <= max), Arg.Is(now.AddDays(2)));
            }

            [Property]
            public void ReturnsPreventServerOverloadWithCorrectDelayWhenTheLeakyBucketIsFull(TimeSpan delay)
            {
                leakyBucket.TryClaimFreeSlots(Arg.Any<int>(), out _)
                    .Returns(x =>
                    {
                        x[1] = delay;
                        return false;
                    });

                var transition = state.Start().SingleAsync().Wait();

                transition.Result.Should().Be(state.PreventOverloadingServer);
                var parameter = ((Transition<TimeSpan>)transition).Parameter;
            }

            [Fact]
            public void AllTheOtherObservablesThrow()
            {
                var fetchObservables = ((Transition<IFetchObservables>)state.Start().SingleAsync().Wait()).Parameter;

                void throwsForList<T>()
                {
                    Action subscribe = () => fetchObservables.GetList<T>().Subscribe();

                    subscribe.Should().Throw<InvalidOperationException>();
                }

                void throwsForSingle<T>()
                {
                    Action subscribe = () => fetchObservables.GetSingle<T>().Subscribe();

                    subscribe.Should().Throw<InvalidOperationException>();
                }

                throwsForList<IWorkspace>();
                throwsForList<IProject>();
                throwsForList<ITask>();
                throwsForList<IClient>();
                throwsForList<ITag>();
                throwsForList<IWorkspaceFeatureCollection>();
                throwsForSingle<IUser>();
                throwsForSingle<IPreferences>();
            }
        }
    }
}
