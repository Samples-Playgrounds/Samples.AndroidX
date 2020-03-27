using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.Tests.Sync.States
{
    public interface ITestModel : IIdentifiable, ILastChangedDatable, IDeletable, IDatabaseSyncable
    {
    }

    public interface IDatabaseTestModel : IDatabaseSyncable, ITestModel
    {
    }

    public interface IThreadSafeTestModel : IThreadSafeModel, IDatabaseTestModel
    {
    }

    public interface IThreadSafeWorkspaceTestModel : IThreadSafeTestModel, IThreadSafeWorkspace
    {
    }

    public interface IThreadSafeUserTestModel : IThreadSafeTestModel, IThreadSafeUser
    {
    }

    public interface IThreadSafeWorkspaceFeatureTestModel : IThreadSafeTestModel, IThreadSafeWorkspaceFeature
    {
    }

    public interface IThreadSafePreferencesTestModel : IThreadSafeTestModel, IThreadSafePreferences
    {
    }

    public interface IThreadSafeTagTestModel : IThreadSafeTestModel, IThreadSafeTag
    {
    }

    public interface IThreadSafeClientTestModel : IThreadSafeTestModel, IThreadSafeClient
    {
    }

    public interface IThreadSafeProjectTestModel : IThreadSafeTestModel, IThreadSafeProject
    {
    }

    public interface IThreadSafeTaskTestModel : IThreadSafeTestModel, IThreadSafeTask
    {
    }

    public interface IThreadSafeTimeEntryTestModel : IThreadSafeTestModel, IThreadSafeTimeEntry
    {
    }

    public sealed class TestModel : IThreadSafeTestModel
    {
        public long Id { get; set; }

        public DateTimeOffset At { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string LastSyncErrorMessage { get; set; }

        public bool IsDeleted { get; set; }

        public TestModel()
        {
        }

        public TestModel(long id, SyncStatus status, bool deleted = false)
        {
            Id = id;
            SyncStatus = status;
            IsDeleted = deleted;
        }

        public static TestModel From(ITestModel model)
            => new TestModel
            {
                Id = model.Id,
                At = model.At,
                IsDeleted = model.IsDeleted,
                ServerDeletedAt = model.ServerDeletedAt,
                SyncStatus = model.SyncStatus,
                LastSyncErrorMessage = model.LastSyncErrorMessage
            };

        public static TestModel Clean(ITestModel testModel)
            => new TestModel(testModel.Id, SyncStatus.InSync) { At = testModel.At };

        public static TestModel Dirty(long id)
            => new TestModel(id, SyncStatus.SyncNeeded);

        public static TestModel DirtyDeleted(long id)
            => new TestModel(id, SyncStatus.SyncNeeded, true);

        public static TestModel Unsyncable(ITestModel model, string message)
            => new TestModel { Id = model.Id, LastSyncErrorMessage = message, SyncStatus = SyncStatus.SyncFailed };
    }
}
