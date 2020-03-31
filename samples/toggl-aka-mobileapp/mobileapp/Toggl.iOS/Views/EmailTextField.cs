using CoreGraphics;
using Foundation;
using System;
using Toggl.iOS.Views;

namespace Toggl.Daneel.Views
{
    [Register(nameof(EmailTextField))]
    public sealed class EmailTextField : LoginTextField
    {
        public EmailTextField(CGRect frame) : base(frame)
        {
        }

        public EmailTextField(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            EditingChanged += onEditingChanged;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            EditingChanged -= onEditingChanged;
        }

        private void onEditingChanged(object sender, EventArgs e)
        {
            Text = Text.Trim();
        }
    }
}
