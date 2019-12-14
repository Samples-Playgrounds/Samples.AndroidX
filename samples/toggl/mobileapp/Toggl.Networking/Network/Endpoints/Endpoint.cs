using System;
using System.Net.Http;
using Toggl.Shared;

namespace Toggl.Networking.Network
{
    internal struct Endpoint
    {
        private static readonly HttpMethod httpMethodPatch = new HttpMethod("PATCH");

        public Uri Url { get; }
        public HttpMethod Method { get; }

        private Endpoint(Uri baseUrl, string uri, HttpMethod method)
            : this(new Uri(baseUrl, uri), method)
        {
        }

        private Endpoint(Uri url, HttpMethod method)
        {
            Ensure.Argument.IsNotNull(url, nameof(url));
            Ensure.Argument.IsAbsoluteUri(url, nameof(url));

            Url = url;
            Method = method;
        }

        public static Endpoint Get(Uri baseUrl, string relativeUri)
            => new Endpoint(baseUrl, relativeUri, HttpMethod.Get);

        public static Endpoint Put(Uri baseUrl, string relativeUri)
            => new Endpoint(baseUrl, relativeUri, HttpMethod.Put);

        public static Endpoint Post(Uri baseUrl, string relativeUri)
            => new Endpoint(baseUrl, relativeUri, HttpMethod.Post);

        public static Endpoint Patch(Uri baseUrl, string relativeUri)
            => new Endpoint(baseUrl, relativeUri, httpMethodPatch);

        public static Endpoint Delete(Uri baseUrl, string relativeUri)
            => new Endpoint(baseUrl, relativeUri, HttpMethod.Delete);
    }
}
