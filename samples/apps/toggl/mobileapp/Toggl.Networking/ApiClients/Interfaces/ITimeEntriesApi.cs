using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Networking.ApiClients.Interfaces;
using Toggl.Shared.Models;

namespace Toggl.Networking.ApiClients
{
    public interface ITimeEntriesApi
        : IDeletingApiClient<ITimeEntry>,
          ICreatingApiClient<ITimeEntry>,
          IUpdatingApiClient<ITimeEntry>,
          IPullingApiClient<ITimeEntry>,
          IPullingChangedApiClient<ITimeEntry>
    {
        Task<List<ITimeEntry>> GetAll(DateTimeOffset start, DateTimeOffset end);
    }
}
