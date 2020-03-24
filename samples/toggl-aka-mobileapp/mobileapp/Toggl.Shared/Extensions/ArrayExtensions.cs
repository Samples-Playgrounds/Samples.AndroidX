namespace Toggl.Shared.Extensions
{
    public static class ArrayExtensions
    {
        public static T GetPingPongIndexedItem<T>(this T[] array, int index)
            => array[index.PingPongClamp(array.Length)];
    }
}
