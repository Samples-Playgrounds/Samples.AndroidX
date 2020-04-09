namespace Toggl.Shared
{
    public struct Point
    {
        public double X;

        public double Y;

        public static Point Zero { get; } = new Point { X = 0, Y = 0 };
    }
}
