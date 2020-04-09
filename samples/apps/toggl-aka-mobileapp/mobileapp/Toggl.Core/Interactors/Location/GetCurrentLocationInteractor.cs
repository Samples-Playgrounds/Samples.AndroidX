using System;
using System.Reactive.Threading.Tasks;
using Toggl.Networking;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Core.Interactors.Location
{
    public sealed class GetCurrentLocationInteractor : IInteractor<IObservable<ILocation>>
    {
        private readonly ITogglApi api;

        public GetCurrentLocationInteractor(ITogglApi api)
        {
            Ensure.Argument.IsNotNull(api, nameof(api));

            this.api = api;
        }

        public IObservable<ILocation> Execute()
            => api.Location.Get().ToObservable();
    }
}
