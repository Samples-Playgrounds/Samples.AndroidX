using System.Collections.Generic;
using Toggl.Core.Analytics;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Sync
{
    public sealed class TransitionHandlerProvider : ITransitionHandlerProvider, ITransitionConfigurator
    {
        private readonly IAnalyticsService analyticsService;

        private readonly Dictionary<IStateResult, (string stateName, TransitionHandler handler)> transitionHandlers
            = new Dictionary<IStateResult, (string stateName, TransitionHandler handler)>();

        public TransitionHandlerProvider(IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            this.analyticsService = analyticsService;
        }

        public void ConfigureTransition(IStateResult result, ISyncState state)
        {
            Ensure.Argument.IsNotNull(result, nameof(result));
            Ensure.Argument.IsNotNull(state, nameof(state));

            var stateName = state.GetType().GetFriendlyName();
            transitionHandlers.Add(result, (stateName, _ => state.Start()));
        }

        public void ConfigureTransition<T>(StateResult<T> result, ISyncState<T> state)
        {
            Ensure.Argument.IsNotNull(result, nameof(result));
            Ensure.Argument.IsNotNull(state, nameof(state));

            var stateName = state.GetType().GetFriendlyName();
            transitionHandlers.Add(
                result,
                (stateName, t => state.Start(((Transition<T>)t).Parameter))
            );
        }

        public TransitionHandler GetTransitionHandler(IStateResult result)
        {
            Ensure.Argument.IsNotNull(result, nameof(result));

            transitionHandlers.TryGetValue(result, out var transitionHandler);
            if (transitionHandler.stateName != null)
            {
                analyticsService.SyncStateTransition.Track(transitionHandler.stateName);
            }

            return transitionHandler.handler;
        }
    }
}
