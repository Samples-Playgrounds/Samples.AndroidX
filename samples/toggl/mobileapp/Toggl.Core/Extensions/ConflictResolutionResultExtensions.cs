using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Extensions
{
    public static class ConflictResolutionResultExtensions
    {
        public static IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> ToThreadSafeResult<TThreadsafe, TDatabase>(
            this IObservable<IEnumerable<IConflictResolutionResult<TDatabase>>> resultsObservable, Func<TDatabase, TThreadsafe> from)
            where TThreadsafe : IThreadSafeModel, TDatabase
            where TDatabase : IDatabaseModel
            => resultsObservable.Select(results => results.Select(result => result.ToThreadSafeResult(from)));

        public static IConflictResolutionResult<TThreadsafe> ToThreadSafeResult<TThreadsafe, TDatabase>(this IConflictResolutionResult<TDatabase> result, Func<TDatabase, TThreadsafe> from)
            where TThreadsafe : IThreadSafeModel, TDatabase
            where TDatabase : IDatabaseModel
        {
            switch (result)
            {
                case CreateResult<TDatabase> createResult:
                    return new CreateResult<TThreadsafe>(from(createResult.Entity));
                case DeleteResult<TDatabase> deleteResult:
                    return new DeleteResult<TThreadsafe>(deleteResult.Id);
                case UpdateResult<TDatabase> updateResult:
                    return new UpdateResult<TThreadsafe>(updateResult.OriginalId, from(updateResult.Entity));
                case IgnoreResult<TDatabase> ignoreResult:
                    return new IgnoreResult<TThreadsafe>(ignoreResult.Id);
                default:
                    throw new ArgumentException($"Unknown conflict resolution result type {result.GetType().FullName}");
            }
        }

        public static IObservable<IEnumerable<TThreadsafe>> UnwrapUpdatedThreadSafeEntities<TThreadsafe>(
            this IObservable<IEnumerable<IConflictResolutionResult<TThreadsafe>>> resultsObservable)
            where TThreadsafe : IThreadSafeModel
            => resultsObservable.Select(results => results.Cast<UpdateResult<TThreadsafe>>().Select(result => result.Entity));


    }
}
