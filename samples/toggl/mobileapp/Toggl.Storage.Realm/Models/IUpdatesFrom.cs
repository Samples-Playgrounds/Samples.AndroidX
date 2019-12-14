namespace Toggl.Storage.Realm
{
    internal interface IUpdatesFrom<TEntity>
    {
        void SetPropertiesFrom(TEntity entity, Realms.Realm realm);
    }
}
