using Android.OS;
using Android.App;
using Firebase.Analytics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using Toggl.Core.Analytics;
using AppCenterAnalytics = Microsoft.AppCenter.Analytics.Analytics;

namespace Toggl.Droid.Services
{
    public sealed class AnalyticsServiceAndroid : BaseAnalyticsService
    {
        private const int maxAppCenterStringLength = 64;

        private FirebaseAnalytics firebaseAnalytics { get; }

        public AnalyticsServiceAndroid()
        {
#if USE_ANALYTICS
            firebaseAnalytics = Firebase.Analytics.FirebaseAnalytics.GetInstance(Android.App.Application.Context);
#endif
        }

        public override void Track(string eventName, Dictionary<string, string> parameters)
        {
#if USE_ANALYTICS
            var bundle = bundleFromParameters(parameters);
            firebaseAnalytics.LogEvent(eventName, bundle);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(eventName, trimLongParameters(parameters));
#endif
        }

        protected override void TrackException(Exception exception)
        {
            Crashes.TrackError(exception);
        }

        public override void Track(Exception exception, IDictionary<string, string> properties)
        {
            Crashes.TrackError(exception, properties);
        }

        public override void Track(Exception exception, string message)
        {
            var dict = new Dictionary<string, string>
            {
                [nameof(message)] = message,
            };

            Crashes.TrackError(exception, dict);
        }

        private Bundle bundleFromParameters(Dictionary<string, string> parameters)
        {
            var bundle = new Bundle();

            foreach (var entry in parameters)
            {
                bundle.PutString(entry.Key, entry.Value);
            }

            return bundle;
        }

        private Dictionary<string, string> trimLongParameters(Dictionary<string, string> parameters)
        {
            var validParameters = new Dictionary<string, string>();
            foreach (var (key, value) in parameters)
            {
                validParameters.Add(trimForAppCenter(key), trimForAppCenter(value));
            }

            return validParameters;
        }

        private string trimForAppCenter(string text)
            => text.Length > maxAppCenterStringLength ? text.Substring(0, maxAppCenterStringLength) : text;

        public override void SetAppCenterUserId(long id)
        {
            setAppCenterUserId(id.ToString());
        }

        public override void ResetAppCenterUserId()
        {
            setAppCenterUserId("");
        }

        private void setAppCenterUserId(string id)
        {
            try
            {
                AppCenter.SetUserId(id);
            }
            catch
            {
                /* This catch intentionally left blank.
                 * Failure to set the user id for analytics is not a reason to crash the app.
                 * And there are no reasonable ways to gracefuly recover from this.
                 */
            }
        }
    }
}
