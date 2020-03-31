using System;
using Toggl.Shared.Models;

namespace Toggl.Storage.Models
{
    public interface IDatabaseSinceParameter : IIdentifiable
    {
        DateTimeOffset? Since { get; }
    }
}
