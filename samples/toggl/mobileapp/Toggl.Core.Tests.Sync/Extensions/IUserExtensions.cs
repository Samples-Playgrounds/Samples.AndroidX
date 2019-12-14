using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Networking.Models;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.Tests.Sync.Extensions
{
    public static class IUserExtensions
    {
        public static IUser With(
            this IUser user,
            New<long?> defaultWorkspaceId = default(New<long?>),
            New<Email> email = default(New<Email>),
            New<DateTimeOffset> at = default(New<DateTimeOffset>))
            => new User
            {
                Id = user.Id,
                At = at.ValueOr(user.At),
                ApiToken = user.ApiToken,
                DefaultWorkspaceId = defaultWorkspaceId.ValueOr(user.DefaultWorkspaceId),
                BeginningOfWeek = user.BeginningOfWeek,
                Email = email.ValueOr(user.Email),
                Fullname = user.Fullname,
                ImageUrl = user.ImageUrl,
                Language = user.Language,
                Timezone = user.Timezone
            };

        public static IThreadSafeUser ToSyncable(
            this IUser user,
            SyncStatus syncStatus = SyncStatus.InSync,
            bool isDeleted = false,
            string lastSyncErrorMessage = null,
            New<DateTimeOffset> at = default(New<DateTimeOffset>),
            New<Email> email = default(New<Email>))
            => new MockUser
            {
                Id = user.Id,
                At = at.ValueOr(user.At),
                ApiToken = user.ApiToken,
                DefaultWorkspaceId = user.DefaultWorkspaceId,
                BeginningOfWeek = user.BeginningOfWeek,
                Email = email.ValueOr(user.Email),
                Fullname = user.Fullname,
                ImageUrl = user.ImageUrl,
                Language = user.Language,
                SyncStatus = syncStatus,
                IsDeleted = isDeleted,
                LastSyncErrorMessage = lastSyncErrorMessage,
                Timezone = user.Timezone
            };
    }
}
