using System;
using Toggl.iOS.ViewSources.Generic.TableView;
using UIKit;

namespace Toggl.iOS.Cells
{
    public abstract class BaseTableViewCell<TModel> : UITableViewCell
    {
        private TModel item;
        public TModel Item
        {
            get => item;
            set
            {
                item = value;
                UpdateView();
            }
        }

        protected BaseTableViewCell()
        {
        }

        protected BaseTableViewCell(IntPtr handle)
            : base(handle)
        {
        }

        protected abstract void UpdateView();

        public static CellConfiguration<TModel> CellConfiguration(string cellIdentifier)
        {
            return (tableView, indexPath, model) =>
            {
                var cell = (BaseTableViewCell<TModel>)tableView.DequeueReusableCell(cellIdentifier, indexPath);
                cell.Item = model;
                return cell;
            };
        }
    }
}
