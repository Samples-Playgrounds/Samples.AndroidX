using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Networking.Exceptions;
using Toggl.Shared.Models;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Networking.Tests.Integration.BaseTests
{
    public abstract class AuthenticatedGetSinceEndpointBaseTests<T>
        : AuthenticatedEndpointBaseTests<List<T>>
    {
        protected override Task<List<T>> CallEndpointWith(ITogglApi togglApi)
            => CallEndpointWith(togglApi, DateTimeOffset.Now);

        protected abstract Task<List<T>> CallEndpointWith(ITogglApi togglApi, DateTimeOffset threshold);

        protected abstract DateTimeOffset AtDateOf(T model);

        protected abstract T MakeUniqueModel(ITogglApi api, IUser user);

        protected abstract Task<T> PostModelToApi(ITogglApi api, T model);

        protected abstract Expression<Func<T, bool>> ModelWithSameAttributesAs(T model);

        protected async Task<(T, T)> SetUpModels(ITogglApi api, IUser user)
        {
            var firstModel = MakeUniqueModel(api, user);
            var firstModelPosted = await PostModelToApi(api, firstModel);

            // make sure we get different `At` dates
            await Task.Delay(TimeSpan.FromSeconds(2));

            var secondModel = MakeUniqueModel(api, user);
            var secondModelPosted = await PostModelToApi(api, secondModel);

            return (firstModelPosted, secondModelPosted);
        }

        [Fact, LogTestInfo]
        public async Task ReturnsAllModelsIfThresholdIsOldestAtDate()
        {
            var (api, user) = await SetupTestUser();
            var (firstModel, secondModel) = await SetUpModels(api, user);

            var models = await CallEndpointWith(api, AtDateOf(firstModel));

            models.Should().HaveCount(2);
            models.Should().Contain(ModelWithSameAttributesAs(firstModel));
            models.Should().Contain(ModelWithSameAttributesAs(secondModel));
        }

        [Fact, LogTestInfo]
        public async void ReturnsOnlyModelsWithAtDateNewerOrSameAsThreshold()
        {
            var (api, user) = await SetupTestUser();
            var (_, secondModel) = await SetUpModels(api, user);

            var models = await CallEndpointWith(api, AtDateOf(secondModel));

            models.Should().HaveCount(1);
            models.Should().Contain(ModelWithSameAttributesAs(secondModel));
        }

        [Fact, LogTestInfo]
        public async void ReturnsNoModelsIfThresholdIsAfterNewestAtDate()
        {
            var (api, user) = await SetupTestUser();
            var (_, secondModel) = await SetUpModels(api, user);

            var models = await CallEndpointWith(api, AtDateOf(secondModel).AddSeconds(1));

            models.Should().NotBeNull();
            models.Should().BeEmpty();
        }

        [Fact, LogTestInfo]
        public async void SucceedsForThresholdAlmostThreeMonthsAgo()
        {
            var (api, _) = await SetupTestUser();

            await CallEndpointWith(api, DateTimeOffset.Now.AddDays(-85));
        }

        [Fact, LogTestInfo]
        public async void FailsForThresholdMoreThanThreeMonthsAgo()
        {
            var (api, _) = await SetupTestUser();

            Func<Task> callingEndpointWithBadThreshold = async () =>
                await CallEndpointWith(api, DateTimeOffset.Now.AddDays(-95));

            callingEndpointWithBadThreshold.Should().Throw<BadRequestException>();
        }
    }
}
