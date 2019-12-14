using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Tests.Mocks
{
    public sealed class MockUser : IThreadSafeUser
    {
        public MockUser() { }

        public MockUser(IDatabaseUser entity)
        {
            Id = entity.Id;
            ApiToken = entity.ApiToken;
            DefaultWorkspaceId = entity.DefaultWorkspaceId;
            Email = entity.Email;
            Fullname = entity.Fullname;
            BeginningOfWeek = entity.BeginningOfWeek;
            Language = entity.Language;
            ImageUrl = entity.ImageUrl;
            At = entity.At;
            Timezone = entity.Timezone;
        }

        public string ApiToken { get; set; }

        public long? DefaultWorkspaceId { get; set; }

        public Email Email { get; set; }

        public string Fullname { get; set; }

        public BeginningOfWeek BeginningOfWeek { get; set; }

        public string Language { get; set; }

        public string ImageUrl { get; set; }

        public DateTimeOffset At { get; set; }

        public long Id { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string LastSyncErrorMessage { get; set; }

        public bool IsDeleted { get; set; }

        public string Timezone { get; set; }
    }
}
