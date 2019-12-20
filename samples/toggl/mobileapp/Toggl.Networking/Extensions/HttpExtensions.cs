using System.Collections.Generic;
using System.Net.Http.Headers;
using Toggl.Networking.Network;
using static Toggl.Networking.Network.HttpHeader.HeaderType;

namespace Toggl.Networking.Extensions
{
    internal static class HttpExtensions
    {
        public static void AddRange(this HttpRequestHeaders self, IEnumerable<HttpHeader> headers)
        {
            foreach (var header in headers)
            {
                switch (header.Type)
                {
                    case None:
                        break;
                    case Auth:
                        self.Authorization = new AuthenticationHeaderValue("Basic", header.Value);
                        break;
                    default:
                        self.Add(header.Name, header.Value);
                        break;
                }
            }
        }
    }
}
