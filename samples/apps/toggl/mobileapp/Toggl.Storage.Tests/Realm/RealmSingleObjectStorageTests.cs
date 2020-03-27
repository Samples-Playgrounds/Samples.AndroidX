using Toggl.Storage.Realm;

namespace Toggl.Storage.Tests.Realm
{
    public sealed class RealmSingleObjectStorageTests : SingleObjectStorageTests<TestModel>
    {
        protected override ISingleObjectStorage<TestModel> Storage { get; }
            = new SingleObjectStorage<TestModel>(new TestAdapter(_ => __ => true, _ => __ => true));

        protected override TestModel GetModelWith(int id) => new TestModel { Id = id };
    }
}
