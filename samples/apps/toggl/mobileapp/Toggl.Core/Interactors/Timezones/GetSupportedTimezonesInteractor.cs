using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Shared;

namespace Toggl.Core.Interactors.Timezones
{
    public sealed class GetSupportedTimezonesInteractor : IInteractor<IObservable<List<string>>>
    {
        public GetSupportedTimezonesInteractor()
        {
        }

        public IObservable<List<string>> Execute()
        {
            string json = Resources.TimezonesJson;

            var timezones = JsonConvert.DeserializeObject<List<string>>(json);

            return Observable.Return(timezones);
        }
    }
}
