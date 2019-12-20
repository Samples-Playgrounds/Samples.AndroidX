using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Views;
using Toggl.iOS.Cells.Common;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.ViewSources.Common;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared.Extensions.Reactive;
using UIKit;

namespace Toggl.iOS.ViewControllers.Common
{
    public sealed class SelectorViewController<T> : UIViewController
    {
        private readonly ImmutableList<SelectOption<T>> options;
        private readonly string title;
        private readonly Action<T> onChosen;
        private int selectedIndex;

        private UITableView tableView;
        private SelectorTableViewSource source;
        private CompositeDisposable disposeBag = new CompositeDisposable();


        public SelectorViewController(
            string title,
            IEnumerable<SelectOption<T>> options,
            int initialIndex,
            Action<T> onChosen)
        {
            selectedIndex = initialIndex;
            this.title = title;
            this.options = options.ToImmutableList();
            this.onChosen = onChosen;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = title;

            View.BackgroundColor = ColorAssets.TableBackground;

            tableView = new UITableView(View.Bounds);
            tableView.BackgroundColor = ColorAssets.TableBackground;
            tableView.TableFooterView = new UIView();
            tableView.SeparatorColor = ColorAssets.Separator;
            View.AddSubview(tableView);

            source = new SelectorTableViewSource(tableView, selectedIndex, onItemSelected);
            tableView.Source = source;

            Observable.Return(options.Select(item => item.ItemName).ToImmutableList())
                .Subscribe(tableView.Rx().ReloadItems(source));
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            var selected = options[selectedIndex];
            onChosen(selected.Item);
        }

        private void onItemSelected(int selectedIndex)
        {
            this.selectedIndex = selectedIndex;

            if (NavigationController != null)
            {
                NavigationController.PopViewController(true);
            }
            else
            {
                this.Dismiss();
            }
        }
    }
}
