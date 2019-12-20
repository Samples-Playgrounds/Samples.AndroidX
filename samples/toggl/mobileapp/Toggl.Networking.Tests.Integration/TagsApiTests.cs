using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Models;
using Toggl.Networking.Tests.Integration.BaseTests;
using Toggl.Shared.Models;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.Integration
{
    public sealed class TagsApiTests
    {
        public sealed class TheGetAllMethod : AuthenticatedGetAllEndpointBaseTests<ITag>
        {
            protected override Task<List<ITag>> CallEndpointWith(ITogglApi togglApi)
                => togglApi.Tags.GetAll();

            private readonly string[] tags1 =
            {
                "tag1",
                "tag2",
                "tag3"
            };

            private readonly string[] tags2 =
            {
                "tag3",
                "tag4",
                "tag5"
            };

            [Fact, LogTestInfo]
            public async Task ReturnsTagsForAllWorkspaces()
            {
                var (togglApi, user) = await SetupTestUser();
                var otherWorkspace = await togglApi.Workspaces.Create(new Workspace { Name = Guid.NewGuid().ToString() });

                await pushTags(togglApi, tags1, user.DefaultWorkspaceId.Value);
                await pushTags(togglApi, tags2, otherWorkspace.Id);

                var returnedTags = await togglApi.Tags.GetAll();

                returnedTags.Should().HaveCount(tags1.Length + tags2.Length);
                assertTags(returnedTags, tags1, user.DefaultWorkspaceId.Value);
                assertTags(returnedTags, tags2, otherWorkspace.Id);
            }

            private void assertTags(List<ITag> returnedTags, string[] expectedTags, long expectedWorkspaceId)
            {
                foreach (var expectedTag in expectedTags)
                {
                    returnedTags.Should().Contain(t => t.Name == expectedTag && t.WorkspaceId == expectedWorkspaceId);
                }
            }

            private async Task pushTags(ITogglApi togglApi, string[] tags, long workspaceId)
            {
                foreach (var tagName in tags)
                {
                    var tag = new Tag { Name = tagName, WorkspaceId = workspaceId };
                    await togglApi.Tags.Create(tag);
                }
            }
        }


        public sealed class TheGetAllSinceMethod : AuthenticatedGetSinceEndpointBaseTests<ITag>
        {
            protected override Task<List<ITag>> CallEndpointWith(ITogglApi togglApi, DateTimeOffset threshold)
                => togglApi.Tags.GetAllSince(threshold);

            protected override DateTimeOffset AtDateOf(ITag model)
                => model.At;

            protected override ITag MakeUniqueModel(ITogglApi api, IUser user)
                => new Tag { Name = Guid.NewGuid().ToString(), WorkspaceId = user.DefaultWorkspaceId.Value };

            protected override Task<ITag> PostModelToApi(ITogglApi api, ITag model)
                => api.Tags.Create(model);

            protected override Expression<Func<ITag, bool>> ModelWithSameAttributesAs(ITag model)
                => t => isTheSameAs(model, t);
        }

        public sealed class TheCreateMethod : AuthenticatedPostEndpointBaseTests<ITag>
        {
            protected override async Task<ITag> CallEndpointWith(ITogglApi togglApi)
            {
                var user = await togglApi.User.Get();
                var tag = createNewTag(user.DefaultWorkspaceId.Value);
                return await togglApi.Tags.Create(tag);
            }

            [Fact, LogTestInfo]
            public async Task CreatesNewTag()
            {
                var (togglClient, user) = await SetupTestUser();

                var tag = createNewTag(user.DefaultWorkspaceId.Value);
                var persistedTag = await togglClient.Tags.Create(tag);

                persistedTag.Name.Should().Be(tag.Name);
                persistedTag.WorkspaceId.Should().Be(user.DefaultWorkspaceId);
            }
        }

        private static Tag createNewTag(long workspaceID)
            => new Tag { Name = Guid.NewGuid().ToString(), WorkspaceId = workspaceID };

        private static bool isTheSameAs(ITag a, ITag b)
            => a.Id == b.Id
            && a.Name == b.Name
            && a.WorkspaceId == b.WorkspaceId;
    }
}
