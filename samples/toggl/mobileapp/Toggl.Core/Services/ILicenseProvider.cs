using System.Collections.Generic;

namespace Toggl.Core.Services
{
    public interface ILicenseProvider
    {
        Dictionary<string, string> GetAppLicenses();
    }
}
