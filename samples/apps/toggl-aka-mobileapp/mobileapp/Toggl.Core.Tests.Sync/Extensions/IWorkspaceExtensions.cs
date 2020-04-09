using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Core.Tests.Sync.Extensions
{
    public static class IWorkspaceExtensions
    {
        public static IThreadSafeWorkspace ToSyncable(
            this IWorkspace workspace)
            => new MockWorkspace
            {
                Admin = workspace.Admin,
                At = workspace.At,
                DefaultCurrency = workspace.DefaultCurrency,
                DefaultHourlyRate = workspace.DefaultHourlyRate,
                Id = workspace.Id,
                IsDeleted = false,
                IsInaccessible = false,
                LastSyncErrorMessage = null,
                LogoUrl = workspace.LogoUrl,
                Name = workspace.Name,
            };

        public static IEnumerable<IThreadSafeWorkspace> ToSyncable(this IEnumerable<IWorkspace> workspaces)
            => workspaces.Select(workspace => workspace.ToSyncable());

        public static IWorkspace With(
            this IWorkspace workspace,
            New<string> name = default(New<string>))
            => new Networking.Models.Workspace
            {
                Admin = workspace.Admin,
                At = workspace.At,
                DefaultCurrency = workspace.DefaultCurrency,
                DefaultHourlyRate = workspace.DefaultHourlyRate,
                Id = workspace.Id,
                LogoUrl = workspace.LogoUrl,
                Name = name.ValueOr(workspace.Name)
            };
    }
}
