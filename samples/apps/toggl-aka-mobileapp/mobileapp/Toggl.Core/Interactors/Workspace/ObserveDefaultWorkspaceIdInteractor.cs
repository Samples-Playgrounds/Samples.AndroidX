using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Interactors
{
    public class ObserveDefaultWorkspaceIdInteractor : IInteractor<IObservable<long>>
    {
        private readonly ITogglDataSource dataSource;

        public ObserveDefaultWorkspaceIdInteractor(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<long> Execute()
            => dataSource.User.Current
                .SelectMany(user => user.DefaultWorkspaceId.HasValue
                    ? Observable.Return(user.DefaultWorkspaceId.Value)
                    : chooseWorkspace())
                .Where(workspaceId => workspaceId != null)
                .DistinctUntilChanged();

        private IObservable<long> chooseWorkspace()
            => dataSource.Workspaces.GetAll(workspace => !workspace.IsDeleted)
                .Select(workspaces => workspaces.OrderBy(workspace => workspace.Id))
                .SelectMany(workspaces =>
                    workspaces.None()
                        ? Observable.Never<long>()
                        : Observable.Return(workspaces.First().Id));
    }
}
