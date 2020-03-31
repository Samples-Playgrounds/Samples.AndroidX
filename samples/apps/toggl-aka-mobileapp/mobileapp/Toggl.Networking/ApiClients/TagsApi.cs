using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Networking.Models;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    internal sealed class TagsApi : BaseApi, ITagsApi
    {
        private readonly TagEndpoints endPoints;

        public TagsApi(Endpoints endPoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credidentials)
            : base(apiClient, serializer, credidentials, endPoints.LoggedIn)
        {
            this.endPoints = endPoints.Tags;
        }

        public Task<List<ITag>> GetAll()
            => SendRequest<Tag, ITag>(endPoints.Get, AuthHeader);

        public Task<List<ITag>> GetAllSince(DateTimeOffset threshold)
            => SendRequest<Tag, ITag>(endPoints.GetSince(threshold), AuthHeader);

        public async Task<ITag> Create(ITag tag)
        {
            var endPoint = endPoints.Post(tag.WorkspaceId);
            var tagCopy = tag as Tag ?? new Tag(tag);
            return await SendRequest(endPoint, AuthHeader, tagCopy, SerializationReason.Post)
                .ConfigureAwait(false);
        }
    }
}
