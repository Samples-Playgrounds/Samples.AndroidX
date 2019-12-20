using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using TogglTask = Toggl.Networking.Models.Task;

namespace Toggl.Networking.ApiClients
{
    internal sealed class TasksApi : BaseApi, ITasksApi
    {
        private readonly TaskEndpoints endPoints;

        public TasksApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credentials)
            : base(apiClient, serializer, credentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.Tasks;
        }

        public Task<List<ITask>> GetAll()
            => SendRequest<TogglTask, ITask>(endPoints.Get, AuthHeader);

        public Task<List<ITask>> GetAllSince(DateTimeOffset threshold)
            => SendRequest<TogglTask, ITask>(endPoints.GetSince(threshold), AuthHeader);

        public async Task<ITask> Create(ITask task)
        {
            var endPoint = endPoints.Post(task.WorkspaceId, task.ProjectId);
            var taskCopy = task as TogglTask ?? new TogglTask(task);
            return await SendRequest(endPoint, AuthHeader, taskCopy, SerializationReason.Post)
                .ConfigureAwait(false);
        }
    }
}
