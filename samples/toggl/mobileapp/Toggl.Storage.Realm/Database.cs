using Realms;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Shared;
using Toggl.Storage.Models;
using Toggl.Storage.Realm.Models;

namespace Toggl.Storage.Realm
{
    public sealed class Database : ITogglDatabase
    {
        private readonly RealmConfiguration realmConfiguration;

        public Database(RealmConfigurator realmConfigurator)
        {
            Ensure.Argument.IsNotNull(realmConfigurator, nameof(realmConfigurator));

            realmConfiguration = realmConfigurator.Configuration;
            IdProvider = new IdProvider(getRealmInstance);
            SinceParameters = createSinceParameterRepository();
            Tags = Repository<IDatabaseTag>.For(getRealmInstance, (tag, realm) => new RealmTag(tag, realm));
            Tasks = Repository<IDatabaseTask>.For(getRealmInstance, (task, realm) => new RealmTask(task, realm));
            User = SingleObjectStorage<IDatabaseUser>.For(getRealmInstance, (user, realm) => new RealmUser(user, realm));
            Clients = Repository<IDatabaseClient>.For(getRealmInstance, (client, realm) => new RealmClient(client, realm));
            Preferences = SingleObjectStorage<IDatabasePreferences>.For(getRealmInstance, (preferences, realm) => new RealmPreferences(preferences, realm));
            Projects = Repository<IDatabaseProject>.For(getRealmInstance, (project, realm) => new RealmProject(project, realm));
            TimeEntries = Repository<IDatabaseTimeEntry>.For(getRealmInstance, (timeEntry, realm) => new RealmTimeEntry(timeEntry, realm));
            Workspaces = Repository<IDatabaseWorkspace>.For(getRealmInstance, (workspace, realm) => new RealmWorkspace(workspace, realm));
            WorkspaceFeatures = Repository<IDatabaseWorkspaceFeatureCollection>.For(
                getRealmInstance,
                (collection, realm) => new RealmWorkspaceFeatureCollection(collection, realm),
                id => x => x.WorkspaceId == id,
                ids => x => ids.Contains(x.WorkspaceId),
                features => features.WorkspaceId);
        }

        public IIdProvider IdProvider { get; }
        public ISinceParameterRepository SinceParameters { get; }
        public IRepository<IDatabaseTag> Tags { get; }
        public IRepository<IDatabaseTask> Tasks { get; }
        public IRepository<IDatabaseClient> Clients { get; }
        public ISingleObjectStorage<IDatabasePreferences> Preferences { get; }
        public IRepository<IDatabaseProject> Projects { get; }
        public ISingleObjectStorage<IDatabaseUser> User { get; }
        public IRepository<IDatabaseTimeEntry> TimeEntries { get; }
        public IRepository<IDatabaseWorkspace> Workspaces { get; }
        public IRepository<IDatabaseWorkspaceFeatureCollection> WorkspaceFeatures { get; }

        public IObservable<Unit> Clear() =>
            Observable.Start(() =>
            {
                var realm = getRealmInstance();

                using (var transaction = realm.BeginWrite())
                {
                    realm.RemoveAll();
                    transaction.Commit();
                }
            });

        private Realms.Realm getRealmInstance()
            => Realms.Realm.GetInstance(realmConfiguration);

        private ISinceParameterRepository createSinceParameterRepository()
        {
            var sinceParametersRealmAdapter =
                new RealmAdapter<RealmSinceParameter, IDatabaseSinceParameter>(
                    getRealmInstance,
                    (parameter, realm) => new RealmSinceParameter(parameter),
                    id => entity => entity.Id == id,
                    ids => entity => ids.Contains(entity.Id),
                    parameter => parameter.Id);

            return new SinceParameterStorage(sinceParametersRealmAdapter);
        }
    }
}
