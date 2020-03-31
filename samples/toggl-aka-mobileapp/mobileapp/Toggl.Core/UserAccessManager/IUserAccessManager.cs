using System;
using System.Reactive;
using Toggl.Networking;
using Toggl.Shared;

namespace Toggl.Core.Login
{
    public interface IUserAccessManager
    {
        IObservable<ITogglApi> UserLoggedIn { get; }
        IObservable<Unit> UserLoggedOut { get; }

        void OnUserLoggedOut();

        bool CheckIfLoggedIn();

        string GetSavedApiToken();

        void LoginWithSavedCredentials();

        IObservable<Unit> LoginWithGoogle(string googleToken);
        IObservable<Unit> Login(Email email, Password password);

        IObservable<Unit> SignUpWithGoogle(string googleToken, bool termsAccepted, int countryId, string timezone);
        IObservable<Unit> SignUp(Email email, Password password, bool termsAccepted, int countryId, string timezone);

        IObservable<Unit> RefreshToken(Password password);

        IObservable<string> ResetPassword(Email email);
    }
}
