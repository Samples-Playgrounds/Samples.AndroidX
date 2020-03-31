using System;
using System.Collections.Generic;
using Toggl.Core.Sync;

namespace SyncDiagramGenerator
{
    internal sealed class Configurator : ITransitionConfigurator
    {
        public List<object> AllDistinctStatesInOrder { get; } = new List<object>();

        public Dictionary<IStateResult, (object State, Type ParameterType)> Transitions { get; }
            = new Dictionary<IStateResult, (object, Type)>();

        public void ConfigureTransition(IStateResult result, ISyncState state)
        {
            addToListIfNew(state);
            Transitions.Add(result, (state, null));
        }

        public void ConfigureTransition<T>(StateResult<T> result, ISyncState<T> state)
        {
            addToListIfNew(state);
            Transitions.Add(result, (state, typeof(T)));
        }

        private void addToListIfNew(object state)
        {
            if (AllDistinctStatesInOrder.Contains(state))
                return;

            AllDistinctStatesInOrder.Add(state);
        }
    }
}
