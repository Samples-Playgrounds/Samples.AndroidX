using Toggl.Shared;

namespace Toggl.Core.UI.Parameters
{
    public sealed class CredentialsParameter
    {
        public static CredentialsParameter Empty { get; } = new CredentialsParameter { Email = Email.Empty, Password = Password.Empty };

        public CredentialsParameter()
        {
            Email = Email.Empty;
            Password = Password.Empty;
        }

        public Email Email { get; set; }

        public Password Password { get; set; }

        public static CredentialsParameter With(Email email, Password password)
            => new CredentialsParameter { Email = email, Password = password };
    }
}
