namespace Toggl.Storage.Queries
{
    public interface IQuery<T>
    {
        T Execute();
    }
}
