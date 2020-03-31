using System.Collections.Generic;
using System.Net;

namespace Toggl.Networking.Network
{
    internal interface IResponse
    {
        string RawData { get; }

        bool IsSuccess { get; }

        string ContentType { get; }

        HttpStatusCode StatusCode { get; }

        IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; }

        bool IsJson { get; }
    }
}
