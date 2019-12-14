using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Toggl.Networking.Helpers;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared.Models;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.Integration.Helper
{
    internal static class WorkspaceHelper
    {
        public static async Task SetSubscription(IUser user, long workspaceId, PricingPlans plan)
        {
            var json = $"{{\"pricing_plan_id\":{(int)plan}}}";

            await makeRequest($"https://toggl.space/api/v9/workspaces/{workspaceId}/subscription", HttpMethod.Post, user, json);
        }

        public static async Task<List<int>> GetAllAvailablePricingPlans(IUser user)
        {
            var response = await makeRequest($"https://toggl.space/api/v9/workspaces/{user.DefaultWorkspaceId}/plans", HttpMethod.Get, user, null);
            var matches = Regex.Matches(response, "\\\"pricing_plan_id\\\":\\s*(?<id>\\d+),");
            return matches.Cast<Match>().SelectMany(match => match.Groups["id"].Captures.Cast<Capture>().Select(capture => int.Parse(capture.Value))).ToList();
        }

        public static async Task<IWorkspace> Update(IUser user, IWorkspace workspace)
        {
            var serializer = new JsonSerializer();
            var serializedJson = serializer.Serialize(
                workspace as Workspace ?? new Workspace(workspace), SerializationReason.Post);
            var responseJson = await makeRequest(
                $"https://toggl.space/api/v9/workspaces/{workspace.Id}", HttpMethod.Put, user, serializedJson);
            return serializer.Deserialize<Workspace>(responseJson);
        }

        public static async Task Delete(IUser user, long workspaceId)
        {
            await makeRequest(
                $"https://toggl.space/api/v8/workspaces/{workspaceId}/leave", HttpMethod.Delete, user, null);
        }

        private static async Task<string> makeRequest(string endpoint, HttpMethod method, IUser user, string json)
        {
            var requestMessage = AuthorizedRequestBuilder.CreateRequest(
                Credentials.WithApiToken(user.ApiToken), endpoint, method);

            if (json != null)
                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.SendAsync(requestMessage);
                var data = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return data;

                throw ApiExceptions.For(
                    new Request(json, requestMessage.RequestUri, new HttpHeader[0], requestMessage.Method),
                    new Response(data, false, response.Content?.Headers?.ContentType?.MediaType, response.Headers,
                        response.StatusCode));
            }
        }
    }
}
