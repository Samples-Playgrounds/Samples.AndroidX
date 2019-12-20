using System;
using System.Collections.Generic;
using System.Web;
using Toggl.Shared.Extensions.RxAction;

namespace Toggl.Shared.Extensions
{
    public static class UriExtensions
    {
        public static Dictionary<string, string> GetQueryParams(this Uri uri)
        {
            return HttpUtility
                .ParseQueryString(uri.Query)
                .ToDictionary(CommonFunctions.Identity, CommonFunctions.Identity);
        }
    }
}
