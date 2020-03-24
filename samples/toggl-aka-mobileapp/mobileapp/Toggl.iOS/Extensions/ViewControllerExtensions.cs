using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.UI.Views;
using Toggl.iOS.ViewControllers.Common;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Extensions
{
    internal static class ViewControllerExtensions
    {
        internal static void Dismiss(this UIViewController viewController)
        {
            if (viewController.NavigationController != null &&
                !viewController.NavigationController.ViewControllers[0].Equals(viewController))
            {
                viewController.NavigationController.PopViewController(true);
            }
            else if (viewController.PresentingViewController != null)
            {
                viewController.DismissViewController(true, null);
            }
        }

        internal static IObservable<bool> ShowConfirmDialog(this UIViewController viewController, string title, string message, string confirmButtonText, string dismissButtonText)
        {
            return Observable.Create<bool>(observer =>
            {
                var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

                var confirmAction = UIAlertAction.Create(confirmButtonText, UIAlertActionStyle.Default, _ =>
                {
                    observer.OnNext(true);
                    observer.OnCompleted();
                });

                var dismissAction = UIAlertAction.Create(dismissButtonText, UIAlertActionStyle.Cancel, _ =>
                {
                    observer.OnNext(false);
                    observer.OnCompleted();
                });

                alertController.AddAction(confirmAction);
                alertController.AddAction(dismissAction);
                alertController.PreferredAction = confirmAction;

                viewController.PresentViewController(alertController, true, null);

                return Disposable.Empty;
            });
        }

        internal static IObservable<Unit> ShowAlertDialog(this UIViewController viewController, string title, string message, string buttonTitle)
        {
            return Observable.Create<Unit>(observer =>
            {
                var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

                var confirmAction = UIAlertAction.Create(buttonTitle, UIAlertActionStyle.Default, _ =>
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                });

                alertController.AddAction(confirmAction);
                alertController.PreferredAction = confirmAction;

                viewController.PresentViewController(alertController, true, null);

                return Disposable.Empty;
            });
        }

        internal static IObservable<bool> ShowConfirmDestructiveActionDialog(this UIViewController viewController, ActionType type, params object[] formatArgs)
        {
            return Observable.Create<bool>(observer =>
            {
                var (titleFormat, confirmText, cancelText) = selectTextByType(type);

                var title = titleFormat != null
                    ? string.Format(titleFormat, formatArgs)
                    : null;

                var alertController = UIAlertController.Create(title, message: null, preferredStyle: UIAlertControllerStyle.ActionSheet);

                var cancelAction = UIAlertAction.Create(cancelText, UIAlertActionStyle.Cancel, _ =>
                {
                    observer.OnNext(false);
                    observer.OnCompleted();
                });

                var confirmAction = UIAlertAction.Create(confirmText, UIAlertActionStyle.Destructive, _ =>
                {
                    observer.OnNext(true);
                    observer.OnCompleted();
                });

                if (viewController.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
                {
                    var extraCancelAction = UIAlertAction.Create(cancelText, UIAlertActionStyle.Default, _ =>
                    {
                        observer.OnNext(false);
                        observer.OnCompleted();
                    });
                    alertController.AddAction(extraCancelAction);
                }

                alertController.AddAction(confirmAction);
                alertController.AddAction(cancelAction);

                applyPopoverDetailsIfNeeded(viewController, alertController);

                viewController.PresentViewController(alertController, true, null);

                return Disposable.Empty;
            });
        }

        internal static IObservable<T> ShowSelectDialog<T>(this UIViewController viewController, string title,
            IEnumerable<SelectOption<T>> options, int initialSelectionIndex)
        {
            if (viewController.NavigationController == null)
            {
                return viewController.ShowActionSheet(title, options);
            }

            return Observable.Create<T>(observer =>
            {
                var selector =  new SelectorViewController<T>(title, options, initialSelectionIndex, observer.CompleteWith);
                viewController.NavigationController.PushViewController(selector, true);
                return Disposable.Empty;
            });
        }

        internal static IObservable<T> ShowActionSheet<T>(this UIViewController viewController, string title,
                IEnumerable<SelectOption<T>> options)
        {
            return Observable.Create<T>(observer =>
            {
                var alertController = UIAlertController.Create(title, null, UIAlertControllerStyle.ActionSheet);

                foreach (var option in options)
                {
                    var action = UIAlertAction.Create(option.ItemName, UIAlertActionStyle.Default, _ =>
                    {
                        observer.OnNext(option.Item);
                        observer.OnCompleted();
                    });

                    alertController.AddAction(action);
                }

                var cancelAction = UIAlertAction.Create(Resources.Cancel, UIAlertActionStyle.Cancel, _ =>
                {
                    observer.OnCompleted();
                });

                alertController.AddAction(cancelAction);

                applyPopoverDetailsIfNeeded(viewController, alertController);

                viewController.PresentViewController(alertController, true, null);

                return Disposable.Empty;
            });
        }

        private static (string Title, string ConfirmButtonText, string CancelButtonText) selectTextByType(ActionType type)
        {
            switch (type)
            {
                case ActionType.DiscardNewTimeEntry:
                    return (Resources.ConfirmDeleteNewTETitle, Resources.Discard, Resources.Cancel);
                case ActionType.DiscardEditingChanges:
                    return (null, Resources.Discard, Resources.ContinueEditing);
                case ActionType.DeleteExistingTimeEntry:
                    return (null, Resources.Delete, Resources.Cancel);
                case ActionType.DeleteMultipleExistingTimeEntries:
                    return (Resources.DeleteMultipleTimeEntries, Resources.Delete, Resources.Cancel);
                case ActionType.DiscardFeedback:
                    return (null, Resources.Discard, Resources.ContinueEditing);
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }

        private static void applyPopoverDetailsIfNeeded(UIViewController presentingController, UIAlertController alert)
        {
            var popoverController = alert.PopoverPresentationController;
            if (popoverController != null && presentingController.TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
            {
                var view = presentingController.View;
                popoverController.SourceView = view;
                popoverController.SourceRect = new CGRect(view.Bounds.GetMidX(), view.Bounds.GetMidY(), 0, 0);
                popoverController.PermittedArrowDirections = new UIPopoverArrowDirection();
            }
        }
    }
}
