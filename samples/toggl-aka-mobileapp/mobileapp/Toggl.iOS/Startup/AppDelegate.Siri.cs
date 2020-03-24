using Foundation;
using Intents;
using System;
using System.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Intents;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        public override bool ContinueUserActivity(
            UIApplication application,
            NSUserActivity userActivity,
            UIApplicationRestorationHandler completionHandler)
        {
            var navigationService = IosDependencyContainer.Instance.NavigationService;

            var interaction = userActivity.GetInteraction();
            if (interaction == null ||
                (interaction.IntentHandlingStatus != INIntentHandlingStatus.DeferredToApplication
                && interaction.IntentHandlingStatus != INIntentHandlingStatus.Unspecified))
            {
                return false;
            }

            var intent = interaction?.Intent;

            switch (intent)
            {
                case StopTimerIntent _:
                    handleDeeplink(new Uri(ApplicationUrls.TimeEntry.Stop.FromSiri));
                    return true;
                case ShowReportIntent _:
                    handleDeeplink(new Uri(ApplicationUrls.Reports.Default));
                    return true;
                case ShowReportPeriodIntent periodIntent:
                    long? parseLong(string val) => long.TryParse(val, out var i) ? i : (long?)null;
                    long? workspaceId = parseLong(periodIntent.Workspace?.Identifier);
                    var period = periodIntent.Period.ToReportPeriod();
                    var viewPresenter = IosDependencyContainer.Instance.ViewPresenter;
                    var change = new ShowReportsPresentationChange(workspaceId, period);
                    viewPresenter.ChangePresentation(change);
                    return true;
                case StartTimerIntent startTimerIntent:
                    var timeEntryParams = createStartTimeEntryParameters(startTimerIntent);
                    navigationService.Navigate<MainViewModel>(null);
                    navigationService.Navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(timeEntryParams, null);
                    return true;
                default:
                    return false;
            }
        }

        private StartTimeEntryParameters createStartTimeEntryParameters(StartTimerIntent intent)
        {
            var tags = (intent.Tags == null || intent.Tags.Count() == 0)
                ? null
                : intent.Tags.Select(tagid => (long)Convert.ToDouble(tagid.Identifier));

            return new StartTimeEntryParameters(
                DateTimeOffset.Now,
                "",
                null,
                string.IsNullOrEmpty(intent.Workspace?.Identifier) ? null : (long?)Convert.ToDouble(intent.Workspace?.Identifier),
                intent.EntryDescription ?? "",
                string.IsNullOrEmpty(intent.ProjectId?.Identifier) ? null : (long?)Convert.ToDouble(intent.ProjectId?.Identifier),
                tags,
                TimeEntryStartOrigin.Siri
            );
        }
    }
}
