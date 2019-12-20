using Microsoft.Reactive.Testing;
using System;

namespace Toggl.Core.Tests.TestExtensions
{
    public static class TestSchedulerExtensions
    {
        public static ITestableObserver<T> CreateObserverFor<T>(this TestScheduler scheduler, IObservable<T> observable)
        {
            var observer = scheduler.CreateObserver<T>();
            observable.Subscribe(observer);
            return observer;
        }
    }
}
