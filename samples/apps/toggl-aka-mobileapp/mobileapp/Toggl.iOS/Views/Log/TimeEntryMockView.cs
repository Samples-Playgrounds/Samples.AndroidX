using Foundation;
using System;
using System.Linq;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views
{
    public partial class TimeEntryMockView : UIView
    {
        private static readonly Random Random = new Random();

        private const int minClientWidth = 74;
        private const int maxClientWidth = 84;
        private const int minProjectWidth = 42;
        private const int maxProjectWidth = 84;
        private const int minDescriptionWidth = 74;
        private const int maxDescriptionWidth = 110;

        private readonly UIColor[] Colors =
            Core.UI.Helper.Colors.DefaultProjectColors.Select(c => c.ToNativeColor()).ToArray();

        public TimeEntryMockView(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            NSBundle.MainBundle.LoadNib(nameof(TimeEntryMockView), this, null);

            RootView.Frame = Bounds;
            AddSubview(RootView);

            //1/3 chance of being visible
            var clientVisible = Random.Next() % 3 == 0;
            if (clientVisible)
                ClientWidthConstraint.Constant = Random.Next(minClientWidth, maxClientWidth);
            else
                ClientView.Hidden = true;

            ProjectWidthConstraint.Constant = Random.Next(minProjectWidth, maxProjectWidth);
            ProjectView.BackgroundColor = Colors[Random.Next(0, Colors.Length)];
            DescriptionWidthConstraint.Constant = Random.Next(minDescriptionWidth, maxDescriptionWidth);

            this.InsertSeparator();

            SetNeedsLayout();
            SetNeedsUpdateConstraints();
        }
    }
}
