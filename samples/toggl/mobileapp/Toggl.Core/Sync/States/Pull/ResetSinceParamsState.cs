using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.Sync.States.Pull
{
    public class ResetSinceParamsState : ISyncState<IEnumerable<IWorkspace>>
    {
        private readonly ISinceParameterRepository sinceParameterRepository;

        public StateResult<IEnumerable<IWorkspace>> Done { get; } = new StateResult<IEnumerable<IWorkspace>>();

        public ResetSinceParamsState(ISinceParameterRepository sinceParameterRepository)
        {
            Ensure.Argument.IsNotNull(sinceParameterRepository, nameof(sinceParameterRepository));
            this.sinceParameterRepository = sinceParameterRepository;
        }

        public IObservable<ITransition> Start(IEnumerable<IWorkspace> workspaces)
        {
            sinceParameterRepository.Reset();
            return Observable.Return(Done.Transition(workspaces));
        }
    }
}
