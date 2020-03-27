﻿using Plugin.Fingerprint.Abstractions;
using SMS.Fingerprint.Sample;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sample
{
    public partial class MainPage : ContentPage
    {
        private CancellationTokenSource _cancel;
        private bool _initialized;

        public MainPage()
        {
            InitializeComponent();
        }       

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_initialized)
            {
                _initialized = true;
                lblAuthenticationType.Text = "Auth Type: " + await Plugin.Fingerprint.CrossFingerprint.Current.GetAuthenticationTypeAsync();
            }
        }

        private async void OnAuthenticate(object sender, EventArgs e)
        {
            await AuthenticateAsync("Prove you have fingers!");
        }

        private async void OnAuthenticateLocalized(object sender, EventArgs e)
        {
            await AuthenticateAsync("Beweise, dass du Finger hast!", "Abbrechen", "Anders!", "Viel zu schnell!");
        }

        private async Task AuthenticateAsync(string reason, string cancel = null, string fallback = null, string tooFast = null)
        {
            _cancel = swAutoCancel.IsToggled ? new CancellationTokenSource(TimeSpan.FromSeconds(10)) : new CancellationTokenSource();
            lblStatus.Text = "";

            var dialogConfig = new AuthenticationRequestConfiguration("My App", reason)
            { // all optional
                CancelTitle = cancel,
                FallbackTitle = fallback,
                AllowAlternativeAuthentication = swAllowAlternative.IsToggled
            };

            // optional
            dialogConfig.HelpTexts.MovedTooFast = tooFast;

            var result = await Plugin.Fingerprint.CrossFingerprint.Current.AuthenticateAsync(dialogConfig, _cancel.Token);

            await SetResultAsync(result);
        }

        private async Task SetResultAsync(FingerprintAuthenticationResult result)
        {
            if (result.Authenticated)
            {
                await Navigation.PushAsync(new SecretView());
            }
            else
            {
                lblStatus.Text = $"{result.Status}: {result.ErrorMessage}";
            }
        }
    }
}
