using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.Reactive.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using Xunit;

namespace Toggl.Core.Tests
{
    public sealed class TimeServiceTests
    {
        public sealed class TheCurrentTimeObservableProperty
        {
            private readonly TimeService timeService;
            private readonly TestScheduler testScheduler = new TestScheduler();

            public TheCurrentTimeObservableProperty()
            {
                timeService = new TimeService(testScheduler);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTheSameObjectsRegardlessOfTheTimeTheObserversSubscribed()
            {
                var firstStep = 500;
                var secondStep = 3500;
                var expectedNumberOfMessages = (firstStep + secondStep) / 1000;
                var firstObserver = testScheduler.CreateObserver<DateTimeOffset>();
                var secondObserver = testScheduler.CreateObserver<DateTimeOffset>();

                timeService.CurrentDateTimeObservable.Subscribe(firstObserver);
                testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(firstStep).Ticks);
                timeService.CurrentDateTimeObservable.Subscribe(secondObserver);
                testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(secondStep).Ticks);

                secondObserver.Messages
                             .Should().HaveCount(expectedNumberOfMessages)
                             .And.BeEquivalentTo(firstObserver.Messages);
            }

            [Fact, LogIfTooSlow]
            public void PublishesCurrentTimeFlooredToTheCurrentSecond()
            {
                DateTimeOffset roundedNow = default(DateTimeOffset);
                timeService.CurrentDateTimeObservable.Subscribe(time => roundedNow = time);

                testScheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

                roundedNow.Should().NotBe(default(DateTimeOffset));
                roundedNow.Millisecond.Should().Be(0);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsCurrentTimeFlooredToTheCurrentSecond()
            {
                DateTimeOffset roundedNow = timeService.CurrentDateTime;

                roundedNow.Millisecond.Should().Be(0);
            }
        }

        public sealed class TheMidnightObservableProperty
        {
            private TimeService timeService;
            private TestScheduler testScheduler;

            private void reset(DateTimeOffset? now = null)
            {
                testScheduler = new TestScheduler();
                testScheduler.AdvanceTo((now ?? DateTimeOffset.UtcNow).Ticks);
                timeService = new TimeService(testScheduler);
            }

            [Fact, LogIfTooSlow]
            public void EmitsFirstValueAtTheNearestMidnightInTheLocalTimeZone()
            {
                reset();
                int numberOfEmitedValues = 0;
                DateTimeOffset? lastEmitted = null;
                var now = timeService.CurrentDateTime;

                timeService.MidnightObservable.Subscribe(midnight =>
                {
                    numberOfEmitedValues++;
                    lastEmitted = midnight;
                });
                testScheduler.AdvanceBy(TimeSpan.FromHours(24).Ticks);

                numberOfEmitedValues.Should().Be(1);
                lastEmitted.Should().NotBeNull();
                lastEmitted.Should().Be(nextLocalMidnight(now));
            }

            [Fact, LogIfTooSlow]
            public void EmitsSecondValueExcatly24HoursAfterTheNearestMidnight()
            {
                reset();
                int numberOfEmitedValues = 0;
                DateTimeOffset? lastEmitted = null;
                var now = timeService.CurrentDateTime;

                timeService.MidnightObservable.Subscribe(midnight =>
                {
                    numberOfEmitedValues++;
                    lastEmitted = midnight;
                });
                testScheduler.AdvanceBy(TimeSpan.FromDays(2).Ticks);

                numberOfEmitedValues.Should().Be(2);
                lastEmitted.Should().NotBeNull();
                lastEmitted.Should().Be(nextLocalMidnight(now).AddHours(24));
            }

            [Fact, LogIfTooSlow]
            public void SubscriberDoesNotReceiveAnyValueEmittedBeforeItSubscribed()
            {
                reset();
                int numberOfEmitedValues = 0;
                DateTimeOffset? firstEmitted = null;

                testScheduler.AdvanceBy(TimeSpan.FromDays(2).Ticks);
                var timeBeforeSubscription = testScheduler.Now.ToLocalTime();
                timeService.MidnightObservable.Subscribe(midnight =>
                {
                    numberOfEmitedValues++;
                    firstEmitted = firstEmitted ?? midnight;
                });
                testScheduler.AdvanceBy(TimeSpan.FromDays(2).Ticks);

                numberOfEmitedValues.Should().Be(2);
                firstEmitted.Should().BeAfter(timeBeforeSubscription);
            }

            [Theory, LogIfTooSlow]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            public void EmitsSeveralDaysInARowExactlyAtMidnightEvenWhenTheTimeChangesDueToDaylightSavingTime(int numberOfDays)
            {
                reset(new DateTimeOffset(2017, 10, 26, 12, 34, 56, TimeSpan.Zero));
                var emittedDateTimes = new List<DateTimeOffset>();

                timeService.MidnightObservable.Subscribe(emittedDateTimes.Add);
                testScheduler.AdvanceBy(numberOfDays * ticksPerDay);

                emittedDateTimes.Count.Should().Be(numberOfDays);

                emittedDateTimes
                    .Select(midnight => midnight.ToLocalTime())
                    .Should()
                    .OnlyContain(midnight => midnight.Hour == 0 && midnight.Minute == 0 && midnight.Second == 0);
            }

            [Fact, LogIfTooSlow]
            public void SchedulerNowReturnsUtcTime()
            {
                var utcNow = DateTimeOffset.UtcNow;

                var currentThreadSchedulerNow = Scheduler.CurrentThread.Now;
                var defaultSchedulerNow = Scheduler.Default.Now;
                var immediateSchedulerNow = Scheduler.Immediate.Now;
                var schedulerNow = Scheduler.Now;

                currentThreadSchedulerNow.Should().BeCloseTo(utcNow, 1000);
                defaultSchedulerNow.Should().BeCloseTo(utcNow, 1000);
                immediateSchedulerNow.Should().BeCloseTo(utcNow, 1000);
                schedulerNow.Should().BeCloseTo(utcNow, 1000);
            }

            private DateTimeOffset nextLocalMidnight(DateTimeOffset now)
            {
                var dayFromNow = now.ToLocalTime().AddDays(1);
                var date = new DateTime(dayFromNow.Year, dayFromNow.Month, dayFromNow.Day);
                return new DateTimeOffset(date, TimeZoneInfo.Local.GetUtcOffset(date)).ToUniversalTime();
            }

            private long ticksPerDay => TimeSpan.FromDays(1).Ticks;
        }

        public sealed class TheSignificantTimeChangeProperty
        {
            private readonly TimeService timeService;
            private readonly TestScheduler scheduler;

            public TheSignificantTimeChangeProperty()
            {
                scheduler = new TestScheduler();
                timeService = new TimeService(scheduler);
            }

            [Fact, LogIfTooSlow]
            public void EmitsEventWhenTimeZoneChangedInvoked()
            {
                var observer = scheduler.CreateObserver<Unit>();
                timeService.SignificantTimeChangeObservable.Subscribe(observer);

                timeService.SignificantTimeChanged();

                observer.Messages.Count.Should().Be(1);
            }
        }

        public sealed class TheRunAfterDelayMethod
        {
            private readonly TimeService timeService;
            private readonly TestScheduler scheduler;

            public TheRunAfterDelayMethod()
            {
                scheduler = new TestScheduler();
                timeService = new TimeService(scheduler);
            }

            [Property]
            public void RunsTheActionAfterSpecifiedDelay(PositiveInt delaySeconds)
            {
                var delay = TimeSpan.FromSeconds(delaySeconds.Get);
                var actionWasInvoked = false;
                Action action = () => actionWasInvoked = true;

                timeService.RunAfterDelay(delay, action);
                scheduler.AdvanceBy(delay.Ticks);

                actionWasInvoked.Should().BeTrue();
            }

            [Property]
            public void DoesNotRunTheActionAfterSpecifiedDelay(PositiveInt delaySeconds)
            {
                var delay = TimeSpan.FromSeconds(delaySeconds.Get);
                var actionWasInvoked = false;
                Action action = () => actionWasInvoked = true;

                timeService.RunAfterDelay(delay, action);
                scheduler.AdvanceBy(delay.Ticks - 1);

                actionWasInvoked.Should().BeFalse();
            }
        }
    }
}
