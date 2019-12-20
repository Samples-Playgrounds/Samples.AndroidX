using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Extensions;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        private void handleDeeplink(Uri uri)
        {
            var urlParser = IosDependencyContainer.Instance.DeeplinkParser;
            var interactorFactory = IosDependencyContainer.Instance.InteractorFactory;
            var timeService = IosDependencyContainer.Instance.TimeService;
            var navigationService = IosDependencyContainer.Instance.NavigationService;

            var parameters = urlParser.Parse(uri);

            parameters.Match(
                te => te.Start(interactorFactory, timeService),
                te => te.Continue(interactorFactory),
                te => te.Stop(interactorFactory, timeService),
                te => te.Create(interactorFactory),
                te => te.Update(interactorFactory, timeService),
                te => showNewTimeEntry(te),
                te => showEditTimeEntry(te),
                reports => showReports(reports),
                calendar => showCalendar(calendar));
        }

        private async Task showNewTimeEntry(DeeplinkNewTimeEntryParameters timeEntry)
        {
            var startTimeEntryParameters = timeEntry.ToStartTimeEntryParameters(IosDependencyContainer.Instance.TimeService);

            IosDependencyContainer.Instance.NavigationService
                .Navigate<StartTimeEntryViewModel, StartTimeEntryParameters, Unit>(startTimeEntryParameters, null);
        }

        private async Task showEditTimeEntry(DeeplinkEditTimeEntryParameters timeEntry)
        {
            IosDependencyContainer.Instance.NavigationService
                .Navigate<EditTimeEntryViewModel, long[], Unit>(new[] { timeEntry.TimeEntryId }, null);
        }

        private async Task showReports(DeeplinkShowReportsParameters reportsParameters)
        {
            var presenter = IosDependencyContainer.Instance.ViewPresenter;
            var change = new ShowReportsPresentationChange(reportsParameters.WorkspaceId, reportsParameters.StartDate, reportsParameters.EndDate);
            presenter.ChangePresentation(change);
        }

        private async Task showCalendar(DeeplinkShowCalendarParameters calendarParameters)
        {
            var presenter = IosDependencyContainer.Instance.ViewPresenter;
            var interactorFactory = IosDependencyContainer.Instance.InteractorFactory;

            if (calendarParameters.EventId != null)
            {
                var calendarEvent = await interactorFactory.GetCalendarItemWithId(calendarParameters.EventId).Execute();
                var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();

                await interactorFactory
                    .CreateTimeEntry(calendarEvent.AsTimeEntryPrototype(defaultWorkspace.Id), TimeEntryStartOrigin.CalendarEvent)
                    .Execute();
            }

            var change = new ShowCalendarPresentationChange();
            presenter.ChangePresentation(change);
        }
    }
}
