using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.UI.Views;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Activities
{
    public abstract partial class ReactiveActivity<TViewModel>
    {
        public IObservable<bool> Confirm(string title, string message, string confirmButtonText, string dismissButtonText)
            => this.ShowConfirmationDialog(title, message, confirmButtonText, dismissButtonText);

        public IObservable<T> Select<T>(string title, IEnumerable<SelectOption<T>> options, int initialSelectionIndex)
            => this.ShowSelectionDialog(title, options, initialSelectionIndex);

        public IObservable<T> SelectAction<T>(string title, IEnumerable<SelectOption<T>> options)
            => Observable.Throw<T>(new InvalidOperationException("This is not implemented for Android"));

        public IObservable<Unit> Alert(string title, string message, string buttonTitle)
            => this.ShowConfirmationDialog(title, message, buttonTitle, null).Select(_ => Unit.Default);

        public IObservable<bool> ConfirmDestructiveAction(ActionType type, params object[] formatArguments)
            => this.ShowDestructiveActionConfirmationDialog(type, formatArguments);
    }
}
