using System;
using System.Linq;
using System.Reactive.Linq;

namespace Toggl.Shared.Extensions
{
    public class ObservableAddons
    {
        public static IObservable<bool> CombineLatestAll(params IObservable<bool>[] observables)
        {
            return Observable
                .CombineLatest(observables, list => list.All(b => b))
                .DistinctUntilChanged();
        }
    }
}
