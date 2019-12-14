using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.SiriExtension.UI
{
    [Register("ConfirmationView")]
    public partial class ConfirmationView : UIView
    {
        public UILabel ConfirmationLabel => confirmationLabel;

        public ConfirmationView(IntPtr p) : base(p)
        {
            //Your initialization code.
        }
    }
}
