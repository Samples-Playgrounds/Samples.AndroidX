using System.Collections.Generic;
using System.Linq;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Core.Reports
{
    public sealed class ReportsMemoryCache
    {
        private readonly object listLock = new object();
        private readonly IList<IProject> projectsInMemory = new List<IProject>();

        public IEnumerable<Either<long, IProject>> TryGetProjects(long[] projectIds)
        {
            lock (listLock)
            {
                return projectIds.Select(tryGetProjectFromMemoryCache);
            }
        }

        private Either<long, IProject> tryGetProjectFromMemoryCache(long id)
        {
            var project = projectsInMemory.FirstOrDefault(p => p.Id == id);
            return project == null
                ? Either<long, IProject>.WithLeft(id)
                : Either<long, IProject>.WithRight(project);
        }

        internal void PersistInCache(IEnumerable<IProject> projects)
        {
            lock (listLock)
            {
                foreach (var project in projects)
                {
                    projectsInMemory.Add(project);
                }
            }
        }
    }
}
