using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.Sync.States.Pull
{
    public sealed class UpdateSinceDateState<T> : ISyncState<IFetchObservables>
        where T : ILastChangedDatable
    {
        private readonly ISinceParameterRepository sinceParameterRepository;

        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        public UpdateSinceDateState(ISinceParameterRepository sinceParameterRepository)
        {
            Ensure.Argument.IsNotNull(sinceParameterRepository, nameof(sinceParameterRepository));

            this.sinceParameterRepository = sinceParameterRepository;
        }

        public IObservable<ITransition> Start(IFetchObservables fetch)
            => fetch.GetList<T>()
                .Do(maybeUpdateSinceDates)
                .SelectValue(Done.Transition(fetch));

        private void maybeUpdateSinceDates(List<T> entities)
        {
            var since = entities.Max(entity => entity?.At);
            if (since.HasValue)
            {
                sinceParameterRepository.Set<T>(since);
            }
        }
    }
}
