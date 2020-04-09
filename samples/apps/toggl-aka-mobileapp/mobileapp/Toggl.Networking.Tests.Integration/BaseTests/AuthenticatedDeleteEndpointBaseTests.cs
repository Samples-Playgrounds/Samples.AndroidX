using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Toggl.Networking.Tests.Integration.BaseTests
{
    public abstract class AuthenticatedDeleteEndpointBaseTests<T> : AuthenticatedEndpointBaseTests<T>
    {
        protected sealed override async Task<T> CallEndpointWith(ITogglApi togglApi)
        {
            var entity = await Initialize(ValidApi);
            await Delete(togglApi, entity);
            return default;
        }

        protected abstract Task<T> Initialize(ITogglApi api);
        protected abstract Task Delete(ITogglApi api, T entity);
    }
}
