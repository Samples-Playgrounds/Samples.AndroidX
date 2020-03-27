using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    internal sealed class EmptyBarView : UIView
    {
        private const float bottomLineHeight = 1;

        public EmptyBarView()
        {
            var bottomLine = new UIView
            {
                BackgroundColor = Colors.Reports.BarChart.EmptyBar.ToNativeColor(),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            AddSubview(bottomLine);

            bottomLine.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            bottomLine.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            bottomLine.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            bottomLine.HeightAnchor.ConstraintEqualTo(bottomLineHeight).Active = true;
        }
    }
}
