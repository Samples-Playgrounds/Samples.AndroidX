namespace Toggl.Storage.Models
{
    public interface IPotentiallyInaccessible
    {
        bool IsInaccessible { get; }
    }
}
