using System;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Models
{
    internal partial class User
    {
        public User(IDatabaseUser user, long workspaceId)
            : this(user, SyncStatus.SyncNeeded, null)
        {
            DefaultWorkspaceId = workspaceId;
        }

        public sealed class Builder
        {
            public static Builder FromExisting(IDatabaseUser user)
                => new Builder(user);

            public long Id { get; private set; }
            public string ApiToken { get; private set; }
            public long? DefaultWorkspaceId { get; private set; }
            public Email Email { get; private set; }
            public string Fullname { get; private set; }
            public BeginningOfWeek BeginningOfWeek { get; private set; }
            public string Language { get; private set; }
            public string ImageUrl { get; private set; }
            public DateTimeOffset At { get; private set; }
            public SyncStatus SyncStatus { get; private set; }
            public string LastSyncErrorMessage { get; private set; }
            public bool IsDeleted { get; private set; }
            public string Timezone { get; private set; }

            public Builder(IDatabaseUser user)
            {
                Id = user.Id;
                ApiToken = user.ApiToken;
                DefaultWorkspaceId = user.DefaultWorkspaceId;
                Email = user.Email;
                Fullname = user.Fullname;
                BeginningOfWeek = user.BeginningOfWeek;
                Language = user.Language;
                ImageUrl = user.ImageUrl;
                At = user.At;
                SyncStatus = user.SyncStatus;
                LastSyncErrorMessage = user.LastSyncErrorMessage;
                IsDeleted = user.IsDeleted;
                Timezone = user.Timezone;
            }

            public Builder SetBeginningOfWeek(BeginningOfWeek beginningOfWeek)
            {
                BeginningOfWeek = beginningOfWeek;
                return this;
            }

            public Builder SetSyncStatus(SyncStatus syncStatus)
            {
                SyncStatus = syncStatus;
                return this;
            }

            public Builder SetAt(DateTimeOffset dateTime)
            {
                At = dateTime;
                return this;
            }

            public Builder SetDefaultWorkspaceId(long defaultWorkspaceId)
            {
                DefaultWorkspaceId = defaultWorkspaceId;
                return this;
            }

            public User Build()
            {
                ensureValidity();
                return new User(this);
            }

            private void ensureValidity()
            {
                if (Id == 0)
                    throw new InvalidOperationException($"{nameof(Id)} must be specified before building user.");

                if (Enum.IsDefined(typeof(BeginningOfWeek), BeginningOfWeek) == false)
                    throw new InvalidOperationException($"You need to set a valid value to the {nameof(BeginningOfWeek)} property before building user.");

                if (!Email.IsValid)
                    throw new InvalidOperationException($"{nameof(Email)} must be valid before building user.");

                if (string.IsNullOrEmpty(Fullname))
                    throw new InvalidOperationException($"{nameof(Fullname)} must be specified before building user.");
            }
        }

        private User(Builder builder)
        {
            Id = builder.Id;
            ApiToken = builder.ApiToken;
            DefaultWorkspaceId = builder.DefaultWorkspaceId;
            Email = builder.Email;
            Fullname = builder.Fullname;
            BeginningOfWeek = builder.BeginningOfWeek;
            Language = builder.Language;
            ImageUrl = builder.ImageUrl;
            At = builder.At;
            SyncStatus = builder.SyncStatus;
            LastSyncErrorMessage = builder.LastSyncErrorMessage;
            IsDeleted = builder.IsDeleted;
            Timezone = builder.Timezone;
        }
    }

    internal static class UserExtensions
    {
        public static User With(this IDatabaseUser self, long workspaceId) => new User(self, workspaceId);
    }
}
