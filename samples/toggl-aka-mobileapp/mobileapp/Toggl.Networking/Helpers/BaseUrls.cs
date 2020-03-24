using System;

namespace Toggl.Networking.Helpers
{
    internal static class BaseUrls
    {
        private static Uri productionBaseUrl { get; }
            = new Uri("https://mobile.toggl.com");

        private static Uri stagingBaseUrl { get; }
            = new Uri("https://mobile.toggl.space");

        private const string apiPrefix = "/api/v9/";

        private const string reportsPrefix = "/reports/api/v3/";

        public static Uri ForApi(ApiEnvironment environment)
            => forEnvironment(environment, apiPrefix);

        public static Uri ForReports(ApiEnvironment environment)
            => forEnvironment(environment, reportsPrefix);

        private static Uri forEnvironment(ApiEnvironment environment, string prefix)
        {
            switch (environment)
            {
                case ApiEnvironment.Staging:
                    return new Uri(stagingBaseUrl, prefix);
                case ApiEnvironment.Production:
                    return new Uri(productionBaseUrl, prefix);
                default:
                    throw new ArgumentOutOfRangeException(nameof(environment), environment, "Unknown api environment.");
            }
        }
    }
}
