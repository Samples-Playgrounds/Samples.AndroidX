using Toggl.Core.Models;
using Toggl.Core.UI.Interfaces;

namespace Toggl.Core.UI.ViewModels.Selectable
{
    public class SelectableReportPeriodViewModel : IDiffableByIdentifier<SelectableReportPeriodViewModel>
    {
        public ReportPeriod ReportPeriod { get; }

        public bool Selected { get; set; }

        public SelectableReportPeriodViewModel(ReportPeriod reportPeriod, bool selected)
        {
            ReportPeriod = reportPeriod;
            Selected = selected;
        }

        public bool Equals(SelectableReportPeriodViewModel other) =>
            ReportPeriod == other.ReportPeriod && Selected == other.Selected;

        public long Identifier => (long)ReportPeriod;
    }
}
