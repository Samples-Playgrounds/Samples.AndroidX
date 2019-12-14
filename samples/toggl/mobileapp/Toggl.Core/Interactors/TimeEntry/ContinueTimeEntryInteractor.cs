using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public sealed class ContinueTimeEntryInteractor : IInteractor<Task<IThreadSafeTimeEntry>>
    {
        private readonly long timeEntryId;
        private readonly ITimeService timeService;
        private readonly TimeEntryStartOrigin continueMode;
        private readonly IInteractorFactory interactorFactory;

        public ContinueTimeEntryInteractor(
            IInteractorFactory interactorFactory,
            ITimeService timeService,
            TimeEntryStartOrigin continueMode,
            long timeEntryId)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(continueMode, nameof(continueMode));

            this.timeEntryId = timeEntryId;
            this.continueMode = continueMode;
            this.interactorFactory = interactorFactory;
            this.timeService = timeService;
        }

        public async Task<IThreadSafeTimeEntry> Execute()
        {
            var existingTimeEntry = await interactorFactory
                .GetTimeEntryById(timeEntryId)
                .Execute();

            var timeEntry = await interactorFactory
                .CreateTimeEntry(existingTimeEntry.AsRunningTimeEntryPrototype(timeService.CurrentDateTime), continueMode)
                .Execute()
                .ConfigureAwait(false);

            return timeEntry;
        }
    }
}
