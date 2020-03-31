using System;
using Toggl.Core.UI.Parameters;

namespace Toggl.Core.UI.Services
{
    public interface IDeeplinkParser
    {
        DeeplinkParameters Parse(Uri uri);
    }
}
