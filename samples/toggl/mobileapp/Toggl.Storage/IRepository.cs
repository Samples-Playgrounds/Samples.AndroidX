using System;
using System.Collections.Generic;

namespace Toggl.Storage
{
    public interface IRepository<TModel> : IBaseStorage<TModel>
    {
        IObservable<TModel> GetById(long id);
        IObservable<IEnumerable<TModel>> GetByIds(long[] ids);
        IObservable<TModel> ChangeId(long currentId, long newId);
    }
}
