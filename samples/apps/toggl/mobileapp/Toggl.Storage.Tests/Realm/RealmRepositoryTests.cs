using Toggl.Storage.Realm;

namespace Toggl.Storage.Tests.Realm
{
    public sealed class RealmRepositoryTests : RepositoryTests<TestModel>
    {
        protected override IRepository<TestModel> Repository { get; } = new Repository<TestModel>(new TestAdapter());

        protected override TestModel GetModelWith(int id) => new TestModel { Id = id };
    }
}
