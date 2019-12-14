using System.Collections.Generic;
using Toggl.Storage.Models;

namespace Toggl.Core.Models.Interfaces
{
    public partial interface IThreadSafeClient
        : IThreadSafeModel, IDatabaseClient
    {
        new IThreadSafeWorkspace Workspace { get; }
    }

    public partial interface IThreadSafePreferences
        : IThreadSafeModel, IDatabasePreferences
    {
    }

    public partial interface IThreadSafeProject
        : IThreadSafeModel, IDatabaseProject
    {
        new IThreadSafeClient Client { get; }

        new IThreadSafeWorkspace Workspace { get; }

        new IEnumerable<IThreadSafeTask> Tasks { get; }
    }

    public partial interface IThreadSafeTag
        : IThreadSafeModel, IDatabaseTag
    {
        new IThreadSafeWorkspace Workspace { get; }
    }

    public partial interface IThreadSafeTask
        : IThreadSafeModel, IDatabaseTask
    {
        new IThreadSafeUser User { get; }

        new IThreadSafeProject Project { get; }

        new IThreadSafeWorkspace Workspace { get; }
    }

    public partial interface IThreadSafeTimeEntry
        : IThreadSafeModel, IDatabaseTimeEntry
    {
        new IThreadSafeTask Task { get; }

        new IThreadSafeUser User { get; }

        new IThreadSafeProject Project { get; }

        new IThreadSafeWorkspace Workspace { get; }

        new IEnumerable<IThreadSafeTag> Tags { get; }
    }

    public partial interface IThreadSafeUser
        : IThreadSafeModel, IDatabaseUser
    {
    }

    public partial interface IThreadSafeWorkspace
        : IThreadSafeModel, IDatabaseWorkspace
    {
    }

    public partial interface IThreadSafeWorkspaceFeature
        : IThreadSafeModel, IDatabaseWorkspaceFeature
    {
    }

    public partial interface IThreadSafeWorkspaceFeatureCollection
        : IThreadSafeModel, IDatabaseWorkspaceFeatureCollection
    {
        new IThreadSafeWorkspace Workspace { get; }

        new IEnumerable<IThreadSafeWorkspaceFeature> DatabaseFeatures { get; }
    }
}
