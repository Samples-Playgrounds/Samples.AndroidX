using System;
using System.Collections.Generic;
using System.Reactive;

namespace Toggl.Core.UI.Views
{
    public interface IDialogProviderView
    {
        IObservable<bool> Confirm(
            string title,
            string message,
            string confirmButtonText,
            string dismissButtonText);
        IObservable<Unit> Alert(string title, string message, string buttonTitle);
        IObservable<bool> ConfirmDestructiveAction(ActionType type, params object[] formatArguments);
        IObservable<T> Select<T>(string title, IEnumerable<SelectOption<T>> options, int initialSelectionIndex);
        IObservable<T> SelectAction<T>(string title, IEnumerable<SelectOption<T>> options);
    }

    public struct SelectOption<T>
    {
        public T Item { get; }
        public string ItemName { get; }

        public SelectOption(T item, string itemName)
        {
            Item = item;
            ItemName = itemName;
        }
    }

    public enum ActionType
    {
        DiscardNewTimeEntry,
        DiscardEditingChanges,
        DeleteExistingTimeEntry,
        DeleteMultipleExistingTimeEntries,
        DiscardFeedback
    }
}
