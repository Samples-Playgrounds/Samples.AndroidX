using System;
using System.Collections.Generic;
using Foundation;
using Toggl.iOS.Shared.Models;
using UIKit;

namespace Toggl.iOS.TimerWidgetExtension
{
    public class SuggestionsDataSource : UITableViewSource
    {
        private const string identifier = "SuggestionCell";
        public IList<Suggestion> Suggestions { private get; set; }
        public Action<Suggestion> Callback { private get; set; }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(identifier, indexPath) as SuggestionTableViewCell;
            var suggestion = Suggestions[indexPath.Row];
            cell.PopulateCell(suggestion);
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
            => Suggestions.Count;

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            Callback(Suggestions[indexPath.Row]);
            tableView.DeselectRow(indexPath, true);
        }
    }
}
