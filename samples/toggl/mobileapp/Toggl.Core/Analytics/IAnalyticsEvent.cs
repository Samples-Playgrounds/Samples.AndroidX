namespace Toggl.Core.Analytics
{
    public interface IAnalyticsEvent
    {
        void Track();
    }

    public interface IAnalyticsEvent<T>
    {
        void Track(T param);
    }

    public interface IAnalyticsEvent<T1, T2>
    {
        void Track(T1 param1, T2 param2);
    }

    public interface IAnalyticsEvent<T1, T2, T3>
    {
        void Track(T1 param1, T2 param2, T3 param3);
    }

    public interface IAnalyticsEvent<T1, T2, T3, T4>
    {
        void Track(T1 param1, T2 param2, T3 param3, T4 param4);
    }
}
