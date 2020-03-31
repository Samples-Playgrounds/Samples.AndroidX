using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Models.Interfaces;

namespace Toggl.Core.Tests.Interactors.AutocompleteSuggestions
{
    public abstract class BaseAutocompleteSuggestionsInteractorTest
    {
        protected IEnumerable<IThreadSafeClient> Clients { get; }

        protected IEnumerable<IThreadSafeTask> Tasks { get; }

        protected IEnumerable<IThreadSafeProject> Projects { get; }

        protected IEnumerable<IThreadSafeTag> Tags { get; }

        protected IEnumerable<IThreadSafeTimeEntry> TimeEntries { get; }

        protected BaseAutocompleteSuggestionsInteractorTest()
        {
            Clients = Enumerable.Range(10, 10).Select(id =>
            {
                var client = Substitute.For<IThreadSafeClient>();
                client.Id.Returns(id);
                client.Name.Returns(id.ToString());
                return client;
            });

            Tasks = Enumerable.Range(20, 10).Select(id =>
            {
                var task = Substitute.For<IThreadSafeTask>();
                task.Id.Returns(id);
                task.Name.Returns(id.ToString());
                return task;
            }).ToList();

            Projects = Enumerable.Range(30, 10).Select(id =>
            {
                var tasks = id % 2 == 0 ? Tasks.Where(t => (t.Id == id - 10 || t.Id == id - 11)).ToList() : null;
                var project = Substitute.For<IThreadSafeProject>();
                project.Id.Returns(id);
                project.Name.Returns(id.ToString());
                project.Color.Returns("#1e1e1e");
                project.Tasks.Returns(tasks);
                project.Active.Returns(id % 10 != 8);

                var client = id % 2 == 0 ? Clients.Single(c => c.Id == id - 20) : null;
                project.Client.Returns(client);

                if (tasks != null)
                {
                    foreach (var task in tasks)
                        task.ProjectId.Returns(id);
                }

                return project;
            });

            Tags = Enumerable.Range(50, 10).Select(id =>
            {
                var tag = Substitute.For<IThreadSafeTag>();
                tag.Id.Returns(id);
                tag.Name.Returns(id.ToString());
                return tag;
            });

            TimeEntries = Enumerable.Range(40, 10).Select(id =>
            {
                var timeEntry = Substitute.For<IThreadSafeTimeEntry>();
                timeEntry.Id.Returns(id);
                timeEntry.Description.Returns(id.ToString());

                var project = id % 2 == 0 ? Projects.Single(c => c.Id == id - 10) : null;
                timeEntry.Project.Returns(project);

                var task = id > 45 ? project?.Tasks?.First() : null;
                timeEntry.Task.Returns(task);

                timeEntry.IsDeleted.Returns(id % 10 == 9);

                return timeEntry;
            });
        }
    }
}
