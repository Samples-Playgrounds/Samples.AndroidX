namespace Toggl.Core.UI.Reactive
{
    public interface IReactive<out TBase>
    {
        TBase Base { get; }
    }
}
