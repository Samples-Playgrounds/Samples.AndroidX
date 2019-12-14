using FluentAssertions;
using Microsoft.Reactive.Testing;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Shared.Tests
{
    public class ObservableExtensionTests
    {
        public class TheConnectedReplayExtensionMethod
        {
            [Fact, LogIfTooSlow]
            public void Connects()
            {
                var connected = false;
                var observable = Observable.Create<object>(observer =>
                {
                    connected = true;
                    return () => { };
                });

                observable.ConnectedReplay();

                connected.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void Replays()
            {
                var items = new List<string>();
                var observable = new Subject<string>();

                var result = observable.ConnectedReplay();
                observable.OnNext("first");
                observable.OnNext("second");

                result.Subscribe(items.Add);
                items.Should().BeEquivalentTo(new[] { "first", "second" });
                result.Subscribe(items.Add);
                items.Should().BeEquivalentTo(new[] { "first", "second", "first", "second" });
            }
        }

        /**
         * Heads up: We removed our own implementation of the `RetryWhen` operator after updating
         * to Rx 4.1.2 which includes it. We keep the unit test to ensure the behavior is the same.
         */
        public class TheRetryWhenOperator : ReactiveTest
        {
            [Fact, LogIfTooSlow]
            public void ForwardsObservableEvents()
            {
                var scheduler = new TestScheduler();

                var xs = scheduler.CreateColdObservable(
                    OnNext(100, 1),
                    OnNext(150, 2),
                    OnNext(200, 3),
                    OnCompleted<int>(250)
                );

                var res = scheduler.Start(() =>
                    xs.RetryWhen(v => v)
                );

                res.Messages.AssertEqual(
                    OnNext(300, 1),
                    OnNext(350, 2),
                    OnNext(400, 3),
                    OnCompleted<int>(450)
                );

                xs.Subscriptions.AssertEqual(
                    Subscribe(200, 450)
                );
            }

            [Fact, LogIfTooSlow]
            public void CompletesWhenHandlerCompletes()
            {
                var scheduler = new TestScheduler();

                var ex = new Exception();

                var xs = scheduler.CreateColdObservable(
                    OnNext(100, 1),
                    OnNext(150, 2),
                    OnNext(200, 3),
                    OnError<int>(250, ex)
                );

                var res = scheduler.Start(() =>
                    xs.RetryWhen(v => v.Take(1).Skip(1))
                );

                res.Messages.AssertEqual(
                    OnNext(300, 1),
                    OnNext(350, 2),
                    OnNext(400, 3),
                    OnCompleted<int>(450)
                );

                xs.Subscriptions.AssertEqual(
                    Subscribe(200, 450)
                );
            }


            [Fact, LogIfTooSlow]
            public void ForwardsErrorsFromHandler()
            {
                var scheduler = new TestScheduler();

                var ex = new Exception();

                var res = scheduler.Start(() =>
                    Observable.Return(1).RetryWhen<int, int>(v => { throw ex; })
                );

                res.Messages.AssertEqual(
                    OnError<int>(200, ex)
                );
            }

            [Fact, LogIfTooSlow]
            public void ReplacesErrorWithHandlerError()
            {
                var scheduler = new TestScheduler();

                var ex = new Exception();
                var ex2 = new Exception();

                var xs = scheduler.CreateColdObservable(
                    OnNext(100, 1),
                    OnNext(150, 2),
                    OnNext(200, 3),
                    OnError<int>(250, ex)
                );

                var res = scheduler.Start(() =>
                    xs.RetryWhen(v => v.SelectMany(w => Observable.Throw<int>(ex2)))
                );

                res.Messages.AssertEqual(
                    OnNext(300, 1),
                    OnNext(350, 2),
                    OnNext(400, 3),
                    OnError<int>(450, ex2)
                );

                xs.Subscriptions.AssertEqual(
                    Subscribe(200, 450)
                );
            }

            [Fact, LogIfTooSlow]
            public void RetriesWhenHandlerSendsEvent()
            {
                var scheduler = new TestScheduler();

                var ex = new Exception();

                var xs = scheduler.CreateColdObservable(
                    OnNext(5, 1),
                    OnNext(10, 2),
                    OnNext(15, 3),
                    OnError<int>(20, ex)
                );

                var res = scheduler.Start(() =>
                    xs.RetryWhen(v =>
                    {
                        int[] count = { 0 };
                        return v.SelectMany(w =>
                        {
                            int c = ++count[0];
                            if (c == 3)
                            {
                                return Observable.Throw<int>(w);
                            }
                            return Observable.Return(1);
                        });
                    })
                );

                res.Messages.AssertEqual(
                    OnNext(205, 1),
                    OnNext(210, 2),
                    OnNext(215, 3),
                    OnNext(225, 1),
                    OnNext(230, 2),
                    OnNext(235, 3),
                    OnNext(245, 1),
                    OnNext(250, 2),
                    OnNext(255, 3),
                    OnError<int>(260, ex)
                );

                xs.Subscriptions.AssertEqual(
                    Subscribe(200, 220),
                    Subscribe(220, 240),
                    Subscribe(240, 260)
                );
            }

            [Fact, LogIfTooSlow]
            public void CanDelayRetries()
            {
                var scheduler = new TestScheduler();

                var ex = new Exception();

                var xs = scheduler.CreateColdObservable(
                    OnNext(5, 1),
                    OnNext(10, 2),
                    OnNext(15, 3),
                    OnError<int>(20, ex)
                );

                var res = scheduler.Start(() =>
                    xs.RetryWhen(v =>
                    {
                        int[] count = { 0 };
                        return v.SelectMany(w =>
                        {
                            int c = ++count[0];
                            if (c == 3)
                            {
                                return Observable.Throw<int>(w);
                            }
                            return Observable.Return(1).Delay(TimeSpan.FromTicks(c * 100), scheduler);
                        });
                    })
                );

                res.Messages.AssertEqual(
                    OnNext(205, 1),
                    OnNext(210, 2),
                    OnNext(215, 3),
                    OnNext(325, 1),
                    OnNext(330, 2),
                    OnNext(335, 3),
                    OnNext(545, 1),
                    OnNext(550, 2),
                    OnNext(555, 3),
                    OnError<int>(560, ex)
                );

                xs.Subscriptions.AssertEqual(
                    Subscribe(200, 220),
                    Subscribe(320, 340),
                    Subscribe(540, 560)
                );
            }
        }
    }
}
