﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.Pull
{
    internal sealed class PersistListState<TInterface, TDatabaseInterface, TThreadsafeInterface>
        : IPersistState
        where TDatabaseInterface : TInterface, IDatabaseModel
        where TThreadsafeInterface : TDatabaseInterface, IThreadSafeModel
    {
        private readonly IDataSource<TThreadsafeInterface, TDatabaseInterface> dataSource;

        private readonly Func<TInterface, TThreadsafeInterface> convertToThreadsafeEntity;

        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        public PersistListState(
            IDataSource<TThreadsafeInterface, TDatabaseInterface> dataSource,
            Func<TInterface, TThreadsafeInterface> convertToThreadsafeEntity)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(convertToThreadsafeEntity, nameof(convertToThreadsafeEntity));

            this.dataSource = dataSource;
            this.convertToThreadsafeEntity = convertToThreadsafeEntity;
        }

        public IObservable<ITransition> Start(IFetchObservables fetch)
            => fetch.GetList<TInterface>()
                .SingleAsync()
                .Select(toThreadsafeList)
                .SelectMany(dataSource.BatchUpdate)
                .Select(_ => Done.Transition(fetch));

        private IList<TThreadsafeInterface> toThreadsafeList(IEnumerable<TInterface> entities)
            => entities.Select(convertToThreadsafeEntity).ToList();
    }
}
