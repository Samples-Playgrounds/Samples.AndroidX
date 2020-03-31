using Foundation;
using System;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.EntityCreation
{
    public sealed partial class CreateEntityViewCell : BaseTableViewCell<CreateEntitySuggestion>
    {
        public static readonly NSString Key = new NSString(nameof(CreateEntityViewCell));
        public static readonly string Identifier = nameof(CreateEntityViewCell);
        public static readonly UINib Nib;

        private NSAttributedString cachedAddIcon;

        static CreateEntityViewCell()
        {
            Nib = UINib.FromName(nameof(CreateEntityViewCell), NSBundle.MainBundle);
        }

        public CreateEntityViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            cachedAddIcon = "".PrependWithAddIcon(TextLabel.Font.CapHeight);

            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            var result = new NSMutableAttributedString(cachedAddIcon);
            var text = new NSMutableAttributedString(Item.CreateEntityMessage);
            var textColor = Colors.StartTimeEntry.Placeholder.ToNativeColor();

            text.AddAttribute(UIStringAttributeKey.ForegroundColor, textColor, new NSRange(0, text.Length));
            result.Append(text);

            TextLabel.AttributedText = result;
        }
    }
}
