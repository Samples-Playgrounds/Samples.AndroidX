using Foundation;
using System;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views
{
    public sealed partial class TagSuggestionViewCell : BaseTableViewCell<TagSuggestion>
    {
        public static readonly string Identifier = nameof(TagSuggestionViewCell);
        public static readonly UINib Nib;

        static TagSuggestionViewCell()
        {
            Nib = UINib.FromName(nameof(TagSuggestionViewCell), NSBundle.MainBundle);
        }

        public TagSuggestionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            this.InsertSeparator();
        }

        protected override void UpdateView()
        {
            NameLabel.Text = Item.Name;
        }
    }
}
