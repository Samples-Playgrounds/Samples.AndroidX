using System;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public struct BarViewModel
    {
        public double BillablePercent { get; }

        public double NonBillablePercent { get; }

        public BarViewModel(double billablePercent, double nonBillablePercent)
        {
            if (billablePercent < 0)
                throw new ArgumentOutOfRangeException($"Billable percentage cannot be negative, {billablePercent} given.");

            if (nonBillablePercent < 0)
                throw new ArgumentOutOfRangeException($"Non-billable percentage cannot be negative, {nonBillablePercent} given.");

            if (billablePercent + nonBillablePercent > 1)
                throw new ArgumentOutOfRangeException($"Billable and non-billable percentage cannot be more than 100% in total, given {billablePercent} + {nonBillablePercent} = {billablePercent + nonBillablePercent} > 1.");

            BillablePercent = billablePercent;
            NonBillablePercent = nonBillablePercent;
        }
    }
}
