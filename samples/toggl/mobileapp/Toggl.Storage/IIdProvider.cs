namespace Toggl.Storage
{
    public interface IIdProvider
    {
        long GetNextIdentifier();
    }
}
