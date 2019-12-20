using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Toggl.iOS.Shared.Exceptions;
using Toggl.iOS.Shared.Models;
using Toggl.Networking;
using Toggl.Shared;
using Toggl.Shared.Models;
using System.Threading.Tasks;
using Toggl.iOS.Shared;
using Toggl.iOS.Shared.Analytics;

namespace Toggl.iOS.TimerWidgetExtension
{
    internal class NetworkingHandler
    {
        private readonly ITogglApi togglApi;

        public NetworkingHandler(ITogglApi togglApi)
        {
           Ensure.Argument.IsNotNull(togglApi, nameof(togglApi));

           this.togglApi = togglApi;
        }

        public async Task<ITimeEntry> StartTimeEntry(TimeEntry timeEntry)
        {
            try
            {
                var createdTimeEntry = await togglApi.TimeEntries
                    .Create(timeEntry)
                    .ConfigureAwait(false);

                SharedStorage.Instance.SetNeedsSync(true);
                SharedStorage.Instance.AddWidgetTrackingEvent(WidgetTrackingEvent.StartTimer());

                return createdTimeEntry;
            }
            catch (Exception e)
            {
                SharedStorage.Instance.AddWidgetTrackingEvent(WidgetTrackingEvent.Error(e.Message));
                throw;
            }
        }

        public async Task StopRunningTimeEntry()
        {
            try
            {
                await togglApi.TimeEntries
                    .GetAll()
                    .ToObservable()
                    .Select(getRunningTimeEntry)
                    .Select(stopTimeEntry)
                    .FirstAsync();

                SharedStorage.Instance.SetNeedsSync(true);
                SharedStorage.Instance.AddWidgetTrackingEvent(WidgetTrackingEvent.StopTimer());
            }
            catch (Exception e)
            {
                SharedStorage.Instance.AddWidgetTrackingEvent(WidgetTrackingEvent.Error(e.Message));
                throw;
            }
        }

        private async Task stopTimeEntry(ITimeEntry timeEntry)
        {
            var duration = (long)(DateTime.Now - timeEntry.Start).TotalSeconds;
            await togglApi.TimeEntries.Update(
                TimeEntry.From(timeEntry).With(duration)
            );
        }

        private ITimeEntry getRunningTimeEntry(IList<ITimeEntry> timeEntries)
            => timeEntries.FirstOrDefault(te => te.Duration == null) ?? throw new NoRunningEntryException();
    }
}
