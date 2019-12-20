using Foundation;
using System;
using Toggl.Core.Autocomplete.Suggestions;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views
{
    public sealed partial class TaskSuggestionViewCell : BaseTableViewCell<TaskSuggestion>
    {
        public static readonly string Identifier = nameof(TaskSuggestionViewCell);
        public static readonly UINib Nib;

        static TaskSuggestionViewCell()
        {
            Nib = UINib.FromName(nameof(TaskSuggestionViewCell), NSBundle.MainBundle);
        }

        public TaskSuggestionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.InsertSeparator();
            ContentView.BackgroundColor = ColorAssets.CustomGray6;
            FadeView.FadeRight = true;
        }

        protected override void UpdateView()
        {
            TaskNameLabel.Text = Item.Name;
        }
    }
}
