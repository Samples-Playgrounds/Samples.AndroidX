using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models.Reports;
using static System.Math;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportsBarChartViewModel : ViewModel
    {
        private const int maximumLabeledNumberOfDays = 7;

        private const int roundToMultiplesOf = 2;

        private readonly IDisposable reportsDisposable;

        public IObservable<IImmutableList<BarViewModel>> Bars { get; }

        public IObservable<int> MaximumHoursPerBar { get; }

        public IObservable<IImmutableList<DateTimeOffset>> HorizontalLegend { get; }

        public IObservable<DateFormat> DateFormat { get; }

        private readonly DateFormat defaultDateFormat = Shared.DateFormat.FromLocalizedDateFormat("mm/dd");

        public ReportsBarChartViewModel(
            ISchedulerProvider schedulerProvider,
            ISingletonDataSource<IThreadSafePreferences> preferencesDataSource,
            IObservable<ITimeEntriesTotals> reports,
            INavigationService navigationService)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(preferencesDataSource, nameof(preferencesDataSource));
            Ensure.Argument.IsNotNull(reports, nameof(reports));

            DateFormat = preferencesDataSource.Current
                .Select(preferences => preferences.DateFormat)
                .AsDriver(onErrorJustReturn: defaultDateFormat, schedulerProvider: schedulerProvider);

            var finalReports = reports.Replay(1);
            reportsDisposable = finalReports.Connect();

            Bars = finalReports.Select(bars)
                .AsDriver(onErrorJustReturn: ImmutableList<BarViewModel>.Empty, schedulerProvider: schedulerProvider);

            MaximumHoursPerBar = finalReports.Select(upperHoursLimit)
                .AsDriver(onErrorJustReturn: 0, schedulerProvider: schedulerProvider);

            HorizontalLegend = finalReports.Select(weeklyLegend)
                .AsDriver(onErrorJustReturn: ImmutableList<DateTimeOffset>.Empty, schedulerProvider: schedulerProvider);
        }

        public override void ViewDestroyed()
        {
            base.ViewDestroyed();

            reportsDisposable.Dispose();
        }

        private IImmutableList<BarViewModel> bars(ITimeEntriesTotals report)
        {
            var upperLimit = upperHoursLimit(report);
            return report.Groups.Select(normalizedBar(upperLimit)).ToImmutableList();
        }

        private int upperHoursLimit(ITimeEntriesTotals report)
        {
            var maximumTotalTrackedTimePerGroup = report.Groups.Max(group => group.Total);
            var rounded = (int)Ceiling(maximumTotalTrackedTimePerGroup.TotalHours / roundToMultiplesOf) * roundToMultiplesOf;
            return Max(roundToMultiplesOf, rounded);
        }

        private Func<ITimeEntriesTotalsGroup, BarViewModel> normalizedBar(double maxHours)
            => group =>
            {
                var billableHours = group.Billable.TotalHours;
                var nonBillableHours = group.Total.TotalHours - billableHours;
                return new BarViewModel(billableHours / maxHours, nonBillableHours / maxHours);
            };

        private IImmutableList<DateTimeOffset> weeklyLegend(ITimeEntriesTotals report)
            => report.Groups.Length <= maximumLabeledNumberOfDays && report.Resolution == Resolution.Day
                ? daysRange(report.Groups.Length, report.StartDate)
                : ImmutableList<DateTimeOffset>.Empty;

        private IImmutableList<DateTimeOffset> daysRange(int numberOfDays, DateTimeOffset startDate)
            => Enumerable.Range(0, numberOfDays)
                .Select(i => startDate.AddDays(i))
                .ToImmutableList();
    }
}
