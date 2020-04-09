using Toggl.Shared;

namespace Toggl.Core.UI.Parameters
{
    public sealed class EmailParameter
    {
        public Email Email { get; set; }

        public static EmailParameter With(Email email)
            => new EmailParameter { Email = email };
    }
}
