namespace Toggl.Shared.Extensions
{
    public static class EmailExtensions
    {
        public static Email ToEmail(this string self)
            => Email.From(self);
    }
}
