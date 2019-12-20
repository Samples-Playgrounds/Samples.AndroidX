using System;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public interface IInteractor<out T>
    {
        T Execute();
    }

    public abstract class TrackableInteractor
    {
        internal IAnalyticsService AnalyticsService { get; }

        public TrackableInteractor(IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));

            AnalyticsService = analyticsService;
        }
    }

    public class TrackedInteractor<TException, TType> : IInteractor<IObservable<TType>>
    where TException : Exception
    {
        private readonly string message;
        private readonly IInteractor<IObservable<TType>> innerInteractor;

        public TrackedInteractor(IInteractor<IObservable<TType>> innerInteractor, string message)
        {
            this.innerInteractor = innerInteractor;
            this.message = message;
        }

        public IObservable<TType> Execute()
        {
            return innerInteractor.Execute()
                .Catch<TType, TException>(e =>
                {
                    ((TrackableInteractor)innerInteractor).AnalyticsService.Track(e, message);
                    return Observable.Throw<TType>(e);
                });
        }
    }

    public static class InteractorExtensions
    {
        public static TrackedInteractor<TException, TType> TrackException<TException, TType>(this IInteractor<IObservable<TType>> interactor, string message)
        where TException : Exception
        {
            return new TrackedInteractor<TException, TType>(interactor, message);
        }
    }
}
