using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Exceptions;
using Toggl.Core.Tests.Sync.Exceptions;
using Toggl.Core.Tests.Sync.Extensions;
using Toggl.Core.Tests.Sync.State;
using Toggl.Networking;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Helpers;
using Toggl.Networking.Network;
using Toggl.Networking.Tests.Integration.Helper;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;

namespace Toggl.Core.Tests.Sync.Helpers
{
    public sealed class Server
    {
        public ITogglApi Api { get; }
        public ServerState InitialServerState { get; }

        private Server(ITogglApi api, ServerState initialServerState)
        {
            Api = api;
            InitialServerState = initialServerState;
        }

        public async Task<ServerState> PullCurrentState()
        {
            var user = await Api.User.Get();
            var clients = await Api.Clients.GetAll();
            var projects = await Api.Projects.GetAll();
            var preferences = await Api.Preferences.Get();
            var tags = await Api.Tags.GetAll();
            var tasks = await Api.Tasks.GetAll();
            var timeEntries = await Api.TimeEntries.GetAll();
            var workspaces = await Api.Workspaces.GetAll();
            var tokens = await Api.PushServices.GetAll();

            return new ServerState(
                user, clients, projects, preferences, tags, tasks, timeEntries, workspaces, pushNotificationsTokens: tokens);
        }

        public async Task Push(ServerState state)
        {
            var updateIds = new object();

            var user = state.User;
            var clients = (IEnumerable<IClient>)state.Clients;
            var projects = (IEnumerable<IProject>)state.Projects;
            var preferences = state.Preferences;
            var tags = (IEnumerable<ITag>)state.Tags;
            var tasks = (IEnumerable<ITask>)state.Tasks;
            var timeEntries = (IEnumerable<ITimeEntry>)state.TimeEntries;
            var pricingPlans = state.PricingPlans;

            // do not push the default workspace twice
            var defaultWorkspace = state.DefaultWorkspace;
            var workspaces = state.Workspaces.Where(ws => ws.Id != InitialServerState.DefaultWorkspace.Id).ToList();

            if (defaultWorkspace != null)
            {
                // update the default workspace with the data
                // there is no need to update IDs - the default WS already has an ID
                await WorkspaceHelper.Update(user, defaultWorkspace);
            }

            if (workspaces.Any())
            {
                await Task.WhenAll(
                    workspaces.Select(async workspace =>
                    {
                        var serverWorkspace = await Api.Workspaces.Create(workspace);

                        lock (updateIds)
                        {
                            user = workspace.Id == user.DefaultWorkspaceId
                                ? user.With(defaultWorkspaceId: serverWorkspace.Id)
                                : user;
                            clients = clients.Select(client => client.WorkspaceId == workspace.Id
                                ? client.With(workspaceId: serverWorkspace.Id)
                                : client);
                            projects = projects.Select(project => project.WorkspaceId == workspace.Id
                                ? project.With(workspaceId: serverWorkspace.Id)
                                : project);
                            tags = tags.Select(tag => tag.WorkspaceId == workspace.Id
                                ? tag.With(workspaceId: serverWorkspace.Id)
                                : tag);
                            tasks = tasks.Select(task => task.WorkspaceId == workspace.Id
                                ? task.With(workspaceId: serverWorkspace.Id)
                                : task);
                            timeEntries = timeEntries.Select(timeEntry => timeEntry.WorkspaceId == workspace.Id
                                ? timeEntry.With(workspaceId: serverWorkspace.Id)
                                : timeEntry);
                            pricingPlans = pricingPlans.ToDictionary(
                                keyValuePair => keyValuePair.Key == workspace.Id
                                    ? serverWorkspace.Id
                                    : keyValuePair.Key,
                                keyValuePair => keyValuePair.Value);
                        }
                    }).ToArray());
            }

            // the user does not want the default workspace on the server
            if (defaultWorkspace == null)
            {
                try
                {
                    await WorkspaceHelper.Delete(user, InitialServerState.DefaultWorkspace.Id);
                }
                catch (ApiException exception)
                {
                    throw new CannotDeleteDefaultWorkspaceException(exception);
                }
            }

            // activate pricing plans
            if (pricingPlans.Any())
            {
                var pricingPlanActivator = new SubscriptionPlanActivator();

                await pricingPlans
                    .Where(keyValuePair => keyValuePair.Value != PricingPlans.Free)
                    .Select(keyValuePair =>
                        pricingPlanActivator.EnsureWorkspaceIsOnPlan(user, keyValuePair.Key, keyValuePair.Value))
                    .ToArray()
                    .Apply(Task.WhenAll);
            }

            var serverUser = await Api.User.Update(user);
            lock (updateIds)
            {
                tasks = tasks.Select(task => task.UserId == user.Id
                    ? task.With(userId: serverUser.Id)
                    : task);
            }

            await Api.Preferences.Update(preferences);

            if (tags.Any())
            {
                await Task.WhenAll(tags.Select(async tag =>
                {
                    var serverTag = await Api.Tags.Create(tag);

                    lock (updateIds)
                    {
                        timeEntries = timeEntries.Select(timeEntry => timeEntry.TagIds.Contains(tag.Id)
                            ? timeEntry.With(tagIds:
                                New<IEnumerable<long>>.Value(
                                    timeEntry.TagIds.Select(id => id == tag.Id ? serverTag.Id : id)))
                            : timeEntry);
                    }
                }));
            }

            if (clients.Any())
            {
                await Task.WhenAll(clients.Select(async client =>
                {
                    var serverClient = await Api.Clients.Create(client);
                    lock (updateIds)
                    {
                        projects = projects.Select(project => project.ClientId == client.Id
                            ? project.With(clientId: serverClient.Id)
                            : project);
                    }
                }));
            }


            if (projects.Any())
            {
                await Task.WhenAll(projects.Select(async project =>
                {
                    var serverProject = await Api.Projects.Create(project);
                    lock (updateIds)
                    {
                        tasks = tasks.Select(task => task.ProjectId == project.Id
                            ? task.With(projectId: serverProject.Id)
                            : task);
                        timeEntries = timeEntries.Select(timeEntry => timeEntry.ProjectId == project.Id
                            ? timeEntry.With(projectId: serverProject.Id)
                            : timeEntry);
                    }
                }));
            }

            if (tasks.Any())
            {
                await Task.WhenAll(tasks.Select(async task =>
                {
                    var serverTask = await Api.Tasks.Create(task);
                    lock (updateIds)
                    {
                        timeEntries = timeEntries.Select(timeEntry => timeEntry.TaskId == task.Id
                            ? timeEntry.With(taskId: serverTask.Id)
                            : timeEntry);
                    }
                }));
            }

            await Task.WhenAll(timeEntries.Select(Api.TimeEntries.Create));

            if (state.PushNotificationsTokens.Any())
            {
                await state.PushNotificationsTokens
                    .Select(Api.PushServices.Subscribe)
                    .Apply(Task.WhenAll);
            }
        }

        public static class Factory
        {
            private static readonly UserAgent userAgent = new UserAgent("TogglSyncingTests", "0.0.0");
            private static readonly ApiEnvironment environment = ApiEnvironment.Staging;

            public static async Task<Server> Create()
            {
                var user = await createUserAccountForTesting();
                var api = createApiFor(user);
                var initialServerState = await fetchInitialServerState(user, api);

                return new Server(api, initialServerState);
            }

            private static async Task<IUser> createUserAccountForTesting()
            {
                IUser user = null;
                int numberOfTries = 0;

                do
                {
                    if (user != null)
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    user = await Networking.Tests.Integration.User.Create();
                }
                while (user.DefaultWorkspaceId.HasValue == false && ++numberOfTries < 3);

                if (!user.DefaultWorkspaceId.HasValue)
                    throw new NoDefaultWorkspaceException($"Failed to create a new user account for testing even after {numberOfTries} tries.");

                return user;
            }

            private static ITogglApi createApiFor(IUser user)
            {
                var credentials = Credentials.WithApiToken(user.ApiToken);
                var apiConfiguration = new ApiConfiguration(environment, credentials, userAgent);
                var apiClient = createApiClient();

                return new TogglApi(apiConfiguration, apiClient);
            }

            private static IApiClient createApiClient()
            {
                var realApiClient = Networking.TogglApiFactory.CreateDefaultApiClient(userAgent);

                return new SlowApiClient(new RetryingApiClient(realApiClient));
            }

            private static async Task<ServerState> fetchInitialServerState(IUser user, ITogglApi api)
            {
                var defaultWorkspace = await api.Workspaces.GetById(user.DefaultWorkspaceId.Value);
                var preferences = await api.Preferences.Get();

                return new ServerState(
                    user,
                    workspaces: new[] { defaultWorkspace },
                    preferences: preferences);
            }
        }
    }
}
