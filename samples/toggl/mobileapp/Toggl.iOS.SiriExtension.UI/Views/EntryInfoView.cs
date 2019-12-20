using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.SiriExtension.UI
{
    [Register("EntryInfoView")]
    public partial class EntryInfoView : UIView
    {
        public UILabel DescriptionLabel => descriptionLabel;
        public UILabel TimeLabel => timeLabel;
        public UILabel TimeFrameLabel => timeFrameLabel;

        public EntryInfoView(IntPtr p) : base(p)
        {
            //Your initialization code.
        }
    }
}
