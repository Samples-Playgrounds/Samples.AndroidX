using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Networking.Models;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Core.Tests.Sync.Extensions
{
    public static class IProjectExtensions
    {
        public static IProject With(
            this IProject project,
            New<bool> active = default(New<bool>),
            New<long> workspaceId = default(New<long>),
            New<long?> clientId = default(New<long?>))
            => new Project
            {
                Id = project.Id,
                ServerDeletedAt = project.ServerDeletedAt,
                At = project.At,
                WorkspaceId = workspaceId.ValueOr(project.WorkspaceId),
                ClientId = clientId.ValueOr(project.ClientId),
                Name = project.Name,
                IsPrivate = project.IsPrivate,
                Active = active.ValueOr(project.Active),
                Color = project.Color,
                Billable = project.Billable,
                Template = project.Template,
                AutoEstimates = project.AutoEstimates,
                EstimatedHours = project.EstimatedHours,
                Rate = project.Rate,
                Currency = project.Currency,
                ActualHours = project.ActualHours
            };

        public static IThreadSafeProject ToSyncable(
            this IProject project)
            => new MockProject
            {
                Id = project.Id,
                ServerDeletedAt = project.ServerDeletedAt,
                At = project.At,
                WorkspaceId = project.WorkspaceId,
                ClientId = project.ClientId,
                Name = project.Name,
                IsPrivate = project.IsPrivate,
                Active = project.Active,
                Color = project.Color,
                Billable = project.Billable,
                Template = project.Template,
                AutoEstimates = project.AutoEstimates,
                EstimatedHours = project.EstimatedHours,
                Rate = project.Rate,
                Currency = project.Currency,
                ActualHours = project.ActualHours,
                IsDeleted = false,
                LastSyncErrorMessage = null
            };

        public static IEnumerable<IThreadSafeProject> ToSyncable(this IEnumerable<IProject> projects)
            => projects.Select(project => project.ToSyncable());
    }
}
