using FluentAssertions;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Storage.Exceptions;
using Xunit;

namespace Toggl.Storage.Tests
{
    public abstract class RepositoryTests<TTestModel> : BaseStorageTests<TTestModel>
        where TTestModel : class, ITestModel, new()
    {
        protected sealed override IObservable<TTestModel> Create(TTestModel testModel)
            => Repository.Create(testModel);

        protected sealed override IObservable<TTestModel> Update(long id, TTestModel testModel)
            => Repository.Update(id, testModel);

        protected sealed override IObservable<Unit> Delete(long id)
            => Repository.Delete(id);

        protected abstract IRepository<TTestModel> Repository { get; }

        [Fact, LogIfTooSlow]
        public void TheGetByIdMethodThrowsIfThereIsNoEntityWithThatIdInTheRepository()
        {
            Func<Task> callingGetByIdInAnEmptyRepository =
                async () => await Repository.GetById(-1);

            callingGetByIdInAnEmptyRepository
                .Should().Throw<DatabaseOperationException<TTestModel>>();
        }

        [Fact, LogIfTooSlow]
        public async Task TheGetByIdMethodAlwaysReturnsASingleElement()
        {
            var testEntity = new TTestModel();
            await Repository.Create(testEntity);

            var element = await Repository.GetById(testEntity.Id).SingleAsync();
            element.Should().Be(testEntity);
        }

        [Fact, LogIfTooSlow]
        public async Task TheGetAllMethodReturnsAnEmptyListIfThereIsNothingOnTheRepository()
        {
            var entities = await Repository.GetAll(_ => true);
            entities.Count().Should().Be(0);
        }

        [Fact, LogIfTooSlow]
        public async Task TheGetAllMethodReturnsAllItemsThatMatchTheQuery()
        {
            const int numberOfItems = 5;

            for (int i = 0; i < numberOfItems; ++i)
            {
                var model = GetModelWith(i);
                await Repository.Create(model);
            }

            var entities = await Repository.GetAll(_ => true);
            entities.Count().Should().Be(numberOfItems);
        }

        [Fact, LogIfTooSlow]
        public async Task TheUpdateCanChangeId()
        {
            var oldTestEntity = GetModelWith(123);
            var nextTestEntity = GetModelWith(456);
            await Create(oldTestEntity);

            await Update(oldTestEntity.Id, nextTestEntity).SingleAsync();
            Func<Task> gettingTheEntityByOldId =
                async () => await Repository.GetById(oldTestEntity.Id);

            gettingTheEntityByOldId
                .Should().Throw<DatabaseOperationException<TTestModel>>();
        }

        [Fact, LogIfTooSlow]
        public async Task TheUpdateFailsIfCalledWithTheOldIdForTheSecondTimeWhenIdChanges()
        {
            var oldTestEntity = GetModelWith(123);
            var nextTestEntity = GetModelWith(456);
            await Create(oldTestEntity);

            await Update(oldTestEntity.Id, nextTestEntity).SingleAsync();
            Func<Task> tringToChangeTheOldEntity =
                async () => await Repository.Update(oldTestEntity.Id, nextTestEntity);

            tringToChangeTheOldEntity
                .Should().Throw<DatabaseOperationException<TTestModel>>();
        }
    }
}
