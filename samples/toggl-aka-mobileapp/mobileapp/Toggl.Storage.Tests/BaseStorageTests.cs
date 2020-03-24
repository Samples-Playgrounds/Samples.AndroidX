using FluentAssertions;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Storage.Exceptions;
using Xunit;

namespace Toggl.Storage.Tests
{
    public abstract class BaseStorageTests<TTestModel>
        where TTestModel : class, ITestModel, IDatabaseSyncable, new()
    {
        protected abstract IObservable<TTestModel> Create(TTestModel testModel);
        protected abstract IObservable<TTestModel> Update(long id, TTestModel testModel);
        protected abstract IObservable<Unit> Delete(long id);

        protected abstract TTestModel GetModelWith(int id);

        [Fact, LogIfTooSlow]
        public void TheUpdateMethodThrowsIfThereIsNoEntityWithThatIdInTheRepository()
        {
            Func<Task> callingUpdateInAnEmptyRepository =
                async () => await Update(12345, new TTestModel());

            callingUpdateInAnEmptyRepository
                .Should().Throw<DatabaseOperationException<TTestModel>>();
        }

        [Fact, LogIfTooSlow]
        public async Task TheUpdateMethodAlwaysReturnsASingleElement()
        {
            var testEntity = GetModelWith(12345);
            await Create(testEntity);

            var element = await Update(testEntity.Id, testEntity).SingleAsync();
            element.Should().Be(testEntity);
        }

        [Fact, LogIfTooSlow]
        public void TheDeleteMethodThrowsIfThereIsNoEntityWithThatIdInTheRepository()
        {
            Func<Task> callingDeleteInAnEmptyRepository =
                async () => await Delete(12345);

            callingDeleteInAnEmptyRepository
                .Should().Throw<DatabaseOperationException<TTestModel>>();
        }
    }
}
