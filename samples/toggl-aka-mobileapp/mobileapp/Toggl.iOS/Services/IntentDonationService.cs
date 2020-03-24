using Foundation;
using Intents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.iOS.Intents;
using Toggl.iOS.Models;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Services
{
    public class IntentDonationService
    {
        private IAnalyticsService analyticsService;

        public IntentDonationService(IAnalyticsService analyticsService)
        {
            this.analyticsService = analyticsService;
        }

        public IObservable<IEnumerable<SiriShortcut>> GetCurrentShortcuts()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                return Observable.Return(Enumerable.Empty<SiriShortcut>());

            return Observable.Create<IEnumerable<SiriShortcut>>(observer =>
                {
                    INVoiceShortcutCenter.SharedCenter.GetAllVoiceShortcuts((shortcuts, error) =>
                    {
                        var siriShortcuts = shortcuts
                            .Select(shortcut => new SiriShortcut(shortcut));

                        observer.OnNext(siriShortcuts);
                    });

                    return new CompositeDisposable { };
                });
        }

        public INIntent CreateIntent(SiriShortcutType shortcutType)
        {
            switch (shortcutType)
            {
                case SiriShortcutType.Start:
                    var startTimerIntent = new StartTimerIntent();
                    startTimerIntent.SuggestedInvocationPhrase = Resources.StartTimerInvocationPhrase;
                    return startTimerIntent;
                case SiriShortcutType.StartFromClipboard:
                    var startTimerWithClipboardIntent = new StartTimerFromClipboardIntent();
                    return startTimerWithClipboardIntent;
                case SiriShortcutType.Continue:
                    var continueTimerIntent = new ContinueTimerIntent();
                    continueTimerIntent.SuggestedInvocationPhrase = Resources.ContinueTimerInvocationPhrase;
                    return continueTimerIntent;
                case SiriShortcutType.Stop:
                    var stopTimerIntent = new StopTimerIntent();
                    stopTimerIntent.SuggestedInvocationPhrase = Resources.StopTimerInvocationPhrase;
                    return stopTimerIntent;
                case SiriShortcutType.ShowReport:
                    var showReportIntent = new ShowReportIntent();
                    showReportIntent.SuggestedInvocationPhrase = Resources.ShowReportsInvocationPhrase;
                    return showReportIntent;
                /*
                case SiriShortcutType.CustomStart:
                    break;
                case SiriShortcutType.CustomReport:
                    break;
                */
                default:
                    throw new ArgumentOutOfRangeException(nameof(shortcutType), shortcutType, null);
            }
        }

        public void SetDefaultShortcutSuggestions()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                return;

            setupDefaultShortcuts();
        }

        public void DonateStartTimeEntry(IThreadSafeTimeEntry timeEntry)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                return;

            var relevantShortcuts = new List<INRelevantShortcut>();

            var startTimerIntent = new StartTimerIntent();
            startTimerIntent.Workspace = new INObject(timeEntry.Workspace.Id.ToString(), timeEntry.Workspace.Name);

            if (!string.IsNullOrEmpty(timeEntry.Description))
            {
                // If any of the tags or the project id were just created and haven't sync we ignore this action until the user repeats it
                if (timeEntry.ProjectId < 0 || timeEntry.TagIds.Any(tagId => tagId < 0))
                    return;

                if (timeEntry.ProjectId is long projectId)
                {
                    var projectINObject = new INObject(timeEntry.ProjectId.ToString(), timeEntry.Project.Name);
                    startTimerIntent.ProjectId = projectINObject;
                }

                startTimerIntent.EntryDescription = timeEntry.Description;

                var tags = timeEntry.TagIds.Select(tag => new INObject(tag.ToString(), tag.ToString())).ToArray();
                startTimerIntent.Tags = tags;

                var billable = new INObject(timeEntry.Billable.ToString(), timeEntry.Billable.ToString());
                startTimerIntent.Billable = billable;
                startTimerIntent.SuggestedInvocationPhrase = string.Format(Resources.SiriTrackEntrySuggestedInvocationPhrase, timeEntry.Description);


                // Relevant shortcut for the Siri Watch Face
                relevantShortcuts.Add(createRelevantShortcut(startTimerIntent));
            }
            else
            {
                startTimerIntent.SuggestedInvocationPhrase = Resources.StartTimerInvocationPhrase;
            }

            var startTimerInteraction = new INInteraction(startTimerIntent, null);
            startTimerInteraction.DonateInteraction(trackError);

            // Descriptionless Relevant Shortcut. Always added even if the intent has one
            var descriptionlessIntent = new StartTimerIntent();
            descriptionlessIntent.Workspace = new INObject(timeEntry.Workspace.Id.ToString(), timeEntry.Workspace.Name);
            var descriptionlessShortcut = createRelevantShortcut(descriptionlessIntent);
            relevantShortcuts.Add(descriptionlessShortcut);

            donateRelevantShortcuts(relevantShortcuts.ToArray());
        }

        public void DonateStopCurrentTimeEntry()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                return;

            var intent = new StopTimerIntent();
            intent.SuggestedInvocationPhrase = Resources.StopTimerInvocationPhrase;

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(trackError);

            var shortcut = createRelevantShortcut(intent);
            donateRelevantShortcuts(shortcut);
        }

        public void DonateShowReport(ReportPeriod period)
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                return;

            var intent = new ShowReportPeriodIntent();
            switch (period)
            {
                case ReportPeriod.Today:
                    intent.Period = ShowReportPeriodReportPeriod.Today;
                    break;
                case ReportPeriod.Yesterday:
                    intent.Period = ShowReportPeriodReportPeriod.Yesterday;
                    break;
                case ReportPeriod.LastWeek:
                    intent.Period = ShowReportPeriodReportPeriod.LastWeek;
                    break;
                case ReportPeriod.LastMonth:
                    intent.Period = ShowReportPeriodReportPeriod.LastMonth;
                    break;
                case ReportPeriod.ThisMonth:
                    intent.Period = ShowReportPeriodReportPeriod.ThisMonth;
                    break;
                case ReportPeriod.ThisWeek:
                    intent.Period = ShowReportPeriodReportPeriod.ThisWeek;
                    break;
                case ReportPeriod.ThisYear:
                    intent.Period = ShowReportPeriodReportPeriod.ThisYear;
                    break;
                case ReportPeriod.Unknown:
                    intent.Period = ShowReportPeriodReportPeriod.Unknown;
                    break;
            }

            intent.SuggestedInvocationPhrase = string.Format(
                Resources.SiriShowReportSuggestedInvocationPhrase,
                period.ToHumanReadableString().ToLower());

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(trackError);
        }

        public void DonateShowReport()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                return;

            var intent = new ShowReportIntent();
            intent.SuggestedInvocationPhrase = Resources.ShowReportsInvocationPhrase;
            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(trackError);
        }

        public void ClearAll()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                return;

            INInteraction.DeleteAllInteractions(_ => { });
            INVoiceShortcutCenter.SharedCenter.SetShortcutSuggestions(new INShortcut[0]);
            INRelevantShortcutStore.DefaultStore.SetRelevantShortcuts(new INRelevantShortcut[0], trackError);
        }

        private void setupDefaultShortcuts()
        {
            INRelevanceProvider[] startTimerRelevanceProviders = {
                new INDailyRoutineRelevanceProvider(INDailyRoutineSituation.Work),
                new INDailyRoutineRelevanceProvider(INDailyRoutineSituation.Gym),
                new INDailyRoutineRelevanceProvider(INDailyRoutineSituation.School)
            };

            INRelevanceProvider[] stopTimerRelevanceProviders = {
                new INDailyRoutineRelevanceProvider(INDailyRoutineSituation.Home)
            };

            var startShortcut = new INShortcut(CreateIntent(SiriShortcutType.Start));
            var startRelevantShorcut = new INRelevantShortcut(startShortcut);
            startRelevantShorcut.RelevanceProviders = startTimerRelevanceProviders;

            var startTimerWithClipboardShortcut = new INShortcut(CreateIntent(SiriShortcutType.StartFromClipboard));
            var startTimerWithClipboardRelevantShorcut = new INRelevantShortcut(startTimerWithClipboardShortcut);
            startTimerWithClipboardRelevantShorcut.RelevanceProviders = startTimerRelevanceProviders;

            var stopShortcut = new INShortcut(CreateIntent(SiriShortcutType.Stop));
            var stopRelevantShortcut = new INRelevantShortcut(stopShortcut);
            stopRelevantShortcut.RelevanceProviders = stopTimerRelevanceProviders;

            var reportShortcut = new INShortcut(CreateIntent(SiriShortcutType.ShowReport));

            var continueTimerShortcut = new INShortcut(CreateIntent(SiriShortcutType.Continue));
            var continueTimerRelevantShortcut = new INRelevantShortcut(continueTimerShortcut);
            continueTimerRelevantShortcut.RelevanceProviders = startTimerRelevanceProviders;

            var shortcuts = new[] { startShortcut, stopShortcut, reportShortcut, continueTimerShortcut, startTimerWithClipboardShortcut };
            INVoiceShortcutCenter.SharedCenter.SetShortcutSuggestions(shortcuts);

            var relevantShortcuts = new[] { startRelevantShorcut, stopRelevantShortcut, continueTimerRelevantShortcut, startTimerWithClipboardRelevantShorcut };
            INRelevantShortcutStore.DefaultStore.SetRelevantShortcuts(relevantShortcuts, trackError);
        }

        private INRelevantShortcut createRelevantShortcut(INIntent intent)
        {
            var shortcut = new INShortcut(intent);
            var relevantShortcut = new INRelevantShortcut(shortcut);
            relevantShortcut.RelevanceProviders = new List<INRelevanceProvider>().ToArray();
            return relevantShortcut;
        }

        private void donateRelevantShortcuts(params INRelevantShortcut[] relevantShortcuts)
        {
            INRelevantShortcutStore.DefaultStore.SetRelevantShortcuts(relevantShortcuts, trackError);
        }

        private void trackError(NSError error)
        {
            if (error == null)
                return;

            analyticsService.TrackAnonymized(new Exception(error.LocalizedDescription));
        }
    }
}
