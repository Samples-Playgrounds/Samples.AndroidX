namespace Toggl.Shared.Extensions
{
    public static class PasswordExtensions
    {
        public static Password ToPassword(this string self)
            => Password.From(self);
    }
}
