using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Core.UI.Views;
using Toggl.iOS.Extensions;

namespace Toggl.iOS.ViewControllers
{
    public partial class ReactiveViewController<TViewModel>
    {
        public IObservable<bool> Confirm(string title, string message, string confirmButtonText, string dismissButtonText)
            => this.ShowConfirmDialog(title, message, confirmButtonText, dismissButtonText);

        public IObservable<Unit> Alert(string title, string message, string buttonTitle)
            => this.ShowAlertDialog(title, message, buttonTitle);

        public IObservable<bool> ConfirmDestructiveAction(ActionType type, params object[] formatArguments)
            => this.ShowConfirmDestructiveActionDialog(type, formatArguments);

        public IObservable<T> Select<T>(string title, IEnumerable<SelectOption<T>> options, int initialSelectionIndex)
            => this.ShowSelectDialog(title, options, initialSelectionIndex);
        
        public IObservable<T> SelectAction<T>(string title, IEnumerable<SelectOption<T>> options)
            => this.ShowActionSheet(title, options);        
    }
}
