using System;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.ConflictResolution
{
    internal static class Resolver
    {
        public static IConflictResolver<IDatabaseClient> ForClients { get; }
            = new PreferNewer<IDatabaseClient>();

        public static IConflictResolver<IDatabaseProject> ForProjects { get; }
            = new PreferNewer<IDatabaseProject>();

        internal static IConflictResolver<IDatabaseUser> ForUser { get; }
            = new PreferNewer<IDatabaseUser>();

        public static IConflictResolver<IDatabaseWorkspace> ForWorkspaces { get; }
            = new PreferNewer<IDatabaseWorkspace>();

        internal static IConflictResolver<IDatabasePreferences> ForPreferences { get; }
            = new OverwriteUnlessNeedsSync<IDatabasePreferences>();

        public static IConflictResolver<IDatabaseWorkspaceFeatureCollection> ForWorkspaceFeatures { get; }
            = new AlwaysOverwrite<IDatabaseWorkspaceFeatureCollection>();

        public static IConflictResolver<IDatabaseTask> ForTasks { get; }
            = new PreferNewer<IDatabaseTask>();

        public static IConflictResolver<IDatabaseTag> ForTags { get; }
            = new PreferNewer<IDatabaseTag>();

        public static IConflictResolver<IDatabaseTimeEntry> ForTimeEntries { get; }
            = new PreferNewer<IDatabaseTimeEntry>(TimeSpan.FromSeconds(5));
    }
}
