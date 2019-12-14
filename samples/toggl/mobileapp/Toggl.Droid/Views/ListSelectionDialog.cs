using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using AndroidX.AppCompat.App;
using Toggl.Core.UI.Views;
using Activity = Android.App.Activity;

namespace Toggl.Droid.Views
{
    public class ListSelectionDialog<T>
    {
        private readonly List<SelectOption<T>> options;
        private readonly string title;
        private readonly Action<T> onChosen;
        private readonly int initialIndex;
        private Activity activity;
        private AlertDialog dialog;

        public ListSelectionDialog(
            Activity activity,
            string title,
            IEnumerable<SelectOption<T>> options,
            int initialIndex,
            Action<T> onChosen)
        {
            this.activity = activity;
            this.initialIndex = initialIndex;
            this.title = title;
            this.options = options.ToList();
            this.onChosen = onChosen;
        }

        public void Show()
        {
            if (activity == null)
                throw new InvalidOperationException("Dialog has already been dismissed.");

            var texts = options.Select(option => option.ItemName).ToArray();

            dialog = new AlertDialog.Builder(activity)
                .SetTitle(title)
                .SetSingleChoiceItems(texts, initialIndex, onItemChosen)
                .Show();

            dialog.CancelEvent += onCancelled;
        }

        private void onCancelled(object sender, EventArgs e)
        {
            onChosen(options[initialIndex].Item);
            activity = null;
        }

        private void onItemChosen(object sender, DialogClickEventArgs args)
        {
            onChosen(options[args.Which].Item);

            dialog.Dismiss();
            activity = null;
        }
    }
}
