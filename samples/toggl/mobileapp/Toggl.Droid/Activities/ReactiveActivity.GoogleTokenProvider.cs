using Android.App;
using Android.Content;
using Android.Gms.Auth;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Exceptions;
using Toggl.Shared.Extensions;
using Object = Java.Lang.Object;

namespace Toggl.Droid.Activities
{
    public abstract partial class ReactiveActivity<TViewModel>
    {
        private const int googleSignInResult = 123;
        private readonly object lockable = new object();
        private readonly string scope = $"oauth2:{Scopes.Profile}";

        private bool isLoggingIn;
        private GoogleApiClient googleApiClient;
        private Subject<string> loginSubject = new Subject<string>();

        public IObservable<string> GetGoogleToken()
        {
            ensureApiClientExists();

            return logOutIfNeeded().SelectMany(getGoogleToken);

            IObservable<Unit> logOutIfNeeded()
            {
                if (!googleApiClient.IsConnected)
                    return Observable.Return(Unit.Default);

                var logoutSubject = new Subject<Unit>();
                var logoutCallback = new LogOutCallback(() => logoutSubject.CompleteWith(Unit.Default));
                Auth.GoogleSignInApi
                    .SignOut(googleApiClient)
                    .SetResultCallback(logoutCallback);

                return logoutSubject.AsObservable();
            }

            IObservable<string> getGoogleToken(Unit _)
            {
                lock (lockable)
                {
                    if (isLoggingIn)
                        return loginSubject.AsObservable();

                    isLoggingIn = true;
                    loginSubject = new Subject<string>();

                    if (googleApiClient.IsConnected)
                    {
                        login();
                        return loginSubject.AsObservable();
                    }

                    googleApiClient.Connect();
                    return loginSubject.AsObservable();
                }
            }
        }

        private void onGoogleSignInResult(Intent data)
        {
            lock (lockable)
            {
                var signInData = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                if (!signInData.IsSuccess)
                {
                    loginSubject.OnError(new GoogleLoginException(signInData.Status.IsCanceled));
                    isLoggingIn = false;
                    return;
                }

                Task.Run(() =>
                {
                    try
                    {
                        var token = GoogleAuthUtil.GetToken(Application.Context, signInData.SignInAccount.Account, scope);
                        loginSubject.OnNext(token);
                        loginSubject.OnCompleted();
                    }
                    catch (Exception e)
                    {
                        loginSubject.OnError(e);
                    }
                    finally
                    {
                        isLoggingIn = false;
                    }
                });
            }
        }

        private void login()
        {
            if (!isLoggingIn) return;

            if (!googleApiClient.IsConnected)
            {
                throw new GoogleLoginException(false);
            }

            var intent = Auth.GoogleSignInApi.GetSignInIntent(googleApiClient);
            StartActivityForResult(intent, googleSignInResult);
        }

        private void onError(ConnectionResult result)
        {
            lock (lockable)
            {
                loginSubject.OnError(new GoogleLoginException(false));
                isLoggingIn = false;
            }
        }

        private void ensureApiClientExists()
        {
            if (googleApiClient != null)
                return;

            var signInOptions = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                .RequestIdToken("{TOGGL_DROID_GOOGLE_SERVICES_CLIENT_ID}")
                .RequestEmail()
                .Build();

            googleApiClient = new GoogleApiClient.Builder(Application.Context)
                .AddConnectionCallbacks(login)
                .AddOnConnectionFailedListener(onError)
                .AddApi(Auth.GOOGLE_SIGN_IN_API, signInOptions)
                .Build();
        }

        private class LogOutCallback : Object, IResultCallback
        {
            private Action callback;

            public LogOutCallback(Action callback)
            {
                this.callback = callback;
            }

            public void OnResult(Object result)
            {
                callback();
            }
        }
    }
}
