using Foundation;
using System;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.StartTimeEntry
{
    public sealed partial class ReactiveTaskSuggestionViewCell : BaseTableViewCell<TaskSuggestion>
    {
        public static readonly NSString Key = new NSString(nameof(ReactiveTaskSuggestionViewCell));
        public static readonly UINib Nib;

        static ReactiveTaskSuggestionViewCell()
        {
            Nib = UINib.FromName(nameof(ReactiveTaskSuggestionViewCell), NSBundle.MainBundle);
        }

        protected ReactiveTaskSuggestionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            FadeView.FadeRight = true;

            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            TaskNameLabel.Text = Item.Name;
        }
    }
}
