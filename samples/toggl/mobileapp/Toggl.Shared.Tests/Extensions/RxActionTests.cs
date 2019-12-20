using FluentAssertions;
using Microsoft.Reactive.Testing;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class RxActionTests
    {
        public sealed class TheConstructor : ReactiveTest
        {
            [Fact, LogIfTooSlow]
            public void CompletesWhenCreatedFromAction()
            {
                var testScheduler = new TestScheduler();

                void actionFunction(int n)
                {
                    return;
                }

                var action = InputAction<int>.FromAction(actionFunction, testScheduler);

                var observer = testScheduler.CreateObserver<Unit>();
                action.ExecuteWithCompletion(2).Subscribe(observer);

                testScheduler.Start();
                observer.Messages.Count.Should().Be(2);
                observer.Messages.Last().Value.Kind.Should().Be(NotificationKind.OnCompleted);
            }

            [Fact, LogIfTooSlow]
            public void CompletesWhenCreatedFromObservable()
            {
                var testScheduler = new TestScheduler();

                IObservable<Unit> observableFunction(int n)
                {
                    return Observable.Return(default(Unit));
                }

                var action = InputAction<int>.FromObservable(observableFunction, testScheduler);

                var observer = testScheduler.CreateObserver<Unit>();
                action.ExecuteWithCompletion(2).Subscribe(observer);

                testScheduler.Start();
                observer.Messages.Count.Should().Be(2);
                observer.Messages.Last().Value.Kind.Should().Be(NotificationKind.OnCompleted);
            }

            [Fact, LogIfTooSlow]
            public void CompletesWhenCreatedFromTask()
            {
                var testScheduler = new TestScheduler();

                Task asyncFunction(int n)
                {
                    return Task.FromResult(default(Unit));
                }

                var action = InputAction<int>.FromAsync(asyncFunction, testScheduler);

                var observer = testScheduler.CreateObserver<Unit>();

                action.ExecuteWithCompletion(2).Subscribe(observer);
                testScheduler.Start();

                observer.Messages.Count.Should().Be(2);
                observer.Messages.Last().Value.Kind.Should().Be(NotificationKind.OnCompleted);
            }

            [Fact, LogIfTooSlow]
            public void CompletesWhenCreatedFromAsyncTask()
            {
                var testScheduler = new TestScheduler();

                async Task asyncFunction(int n)
                {
                    await Task.FromResult(default(Unit));
                }

                var action = InputAction<int>.FromAsync(asyncFunction, testScheduler);

                var observer = testScheduler.CreateObserver<Unit>();

                action.ExecuteWithCompletion(2).Subscribe(observer);
                testScheduler.Start();

                observer.Messages.Count.Should().Be(2);
                observer.Messages.Last().Value.Kind.Should().Be(NotificationKind.OnCompleted);
            }
        }

        public sealed class TheErrorsProperty : ReactiveTest
        {
            [Fact, LogIfTooSlow]
            public void ForwardsErrorsInTheExecution()
            {
                var testScheduler = new TestScheduler();
                var exception = new Exception("This is an error");

                var observable = testScheduler.CreateColdObservable(
                    OnError<string>(10, exception)
                );

                var action = new RxAction<Unit, string>(_ => observable, testScheduler);

                var observer = testScheduler.CreateObserver<Exception>();

                action.Errors.Subscribe(observer);

                testScheduler.Schedule(TimeSpan.FromTicks(300), () => action.Execute(Unit.Default));
                testScheduler.Start();

                observer.Messages.AssertEqual(
                    OnNext(311, exception)
                );
            }
        }

        public sealed class TheExecutingProperty : ReactiveTest
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueWhileExecuting()
            {
                var testScheduler = new TestScheduler();
                var observer = testScheduler.CreateObserver<bool>();
                var observable = testScheduler.CreateColdObservable(
                    OnNext(10, "0"),
                    OnNext(20, "1"),
                    OnCompleted<string>(30)
                );

                var action = new RxAction<Unit, string>(_ => observable, testScheduler);

                action.Executing.Subscribe(observer);

                testScheduler.Schedule(TimeSpan.FromTicks(300), () =>
                {
                    action.Execute(Unit.Default);
                });
                testScheduler.Start();

                observer.Messages.AssertEqual(
                    OnNext(0, false),
                    OnNext(300, true),
                    OnNext(331, false)
                );
            }
        }

        public sealed class TheEnabledProperty : ReactiveTest
        {
            [Fact, LogIfTooSlow]
            public void ReturnsFalseWhileExecuting()
            {
                var testScheduler = new TestScheduler();
                var observer = testScheduler.CreateObserver<bool>();
                var observable = testScheduler.CreateColdObservable(
                    OnNext(10, "1"),
                    OnNext(20, "2"),
                    OnCompleted<string>(30)
                );

                var action = new RxAction<Unit, string>(_ => observable, testScheduler);

                action.Enabled.Subscribe(observer);

                testScheduler.Schedule(TimeSpan.FromTicks(300), () =>
                {
                    action.Execute(Unit.Default);
                });
                testScheduler.Start();

                observer.Messages.AssertEqual(
                    OnNext(0, true),
                    OnNext(300, false),
                    OnNext(331, true)
                );
            }
        }
    }
}
