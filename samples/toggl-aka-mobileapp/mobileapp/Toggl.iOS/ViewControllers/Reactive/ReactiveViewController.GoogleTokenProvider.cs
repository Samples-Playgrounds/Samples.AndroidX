using Foundation;
using Google.SignIn;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Exceptions;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public abstract partial class ReactiveViewController<TViewModel> : ISignInDelegate
    {
        private const int cancelErrorCode = -5;

        private bool loggingIn;
        private Subject<string> tokenSubject = new Subject<string>();

        public IObservable<string> GetGoogleToken()
        {
            if (loggingIn)
                return tokenSubject.AsObservable();

            if (SignIn.SharedInstance.CurrentUser != null)
                SignIn.SharedInstance.SignOutUser();

            SignIn.SharedInstance.Delegate = this;
            SignIn.SharedInstance.PresentingViewController = this;
            SignIn.SharedInstance.SignInUser();
            loggingIn = true;

            return tokenSubject.AsObservable();
        }

        public void DidSignIn(SignIn signIn, GoogleUser user, NSError error)
        {
            if (error != null)
            {
                tokenSubject.OnError(new GoogleLoginException(error.Code == cancelErrorCode));
            }
            else
            {
                var token = user.Authentication.AccessToken;
                signIn.DisconnectUser();
                tokenSubject.CompleteWith(token);
            }

            tokenSubject = new Subject<string>();
            loggingIn = false;
        }

        [Export("signIn:presentViewController:")]
        public void PresentViewController(SignIn signIn, UIViewController viewController)
        {
            PresentViewController(viewController, true, null);
        }

        [Export("signIn:dismissViewController:")]
        public void DismissViewController(SignIn signIn, UIViewController viewController)
        {
            DismissViewController(true, null);
        }
    }
}
