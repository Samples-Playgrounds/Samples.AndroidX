using System.Threading.Tasks;
using Toggl.Networking.Network;
using Toggl.Shared.Models;

namespace Toggl.Networking.Tests.Integration.BaseTests
{
    public abstract class EndpointTestBase
    {
        protected async Task<(ITogglApi togglClient, IUser user)> SetupTestUser()
        {
            var user = await User.Create();
            var credentials = Credentials.WithApiToken(user.ApiToken);
            var togglApi = TogglApiWith(credentials);

            return (togglApi, user);
        }

        protected ITogglApi TogglApiWith(Credentials credentials)
            => Helper.TogglApiFactory.TogglApiWith(credentials);
    }
}
