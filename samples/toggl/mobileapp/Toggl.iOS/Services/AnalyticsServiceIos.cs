using Foundation;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Analytics;
using AppCenterAnalytics = Microsoft.AppCenter.Analytics.Analytics;
using AppCenter = Microsoft.AppCenter.AppCenter;
using FirebaseAnalytics = Firebase.Analytics.Analytics;

namespace Toggl.iOS.Services
{
    public sealed class AnalyticsServiceIos : BaseAnalyticsService
    {
        private const int maxAppCenterStringLength = 64;

        public override void Track(string eventName, Dictionary<string, string> parameters = null)
        {
            parameters = parameters ?? new Dictionary<string, string>();

            AppCenterAnalytics.TrackEvent(eventName, trimLongParameters(parameters));

            var convertedDictionary = convertDictionary(parameters);
            FirebaseAnalytics.LogEvent(new NSString(eventName), convertedDictionary);
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

        private NSDictionary<NSString, NSObject> convertDictionary(Dictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
                return new NSDictionary<NSString, NSObject>();

            return NSDictionary<NSString, NSObject>.FromObjectsAndKeys(
                parameters.Values.ToArray(),
                parameters.Keys.ToArray()
            );
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

        private string trimForAppCenter(string text)
            => text.Length > maxAppCenterStringLength ? text.Substring(0, maxAppCenterStringLength) : text;
    }
}
