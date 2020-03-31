using Foundation;
using Intents;
using IntentsUI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Models;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    using ShortcutSection = SectionModel<string, SiriShortcutViewModel>;

    public sealed partial class SiriShortcutsViewController : ReactiveViewController<SiriShortcutsViewModel>, IINUIAddVoiceShortcutViewControllerDelegate, IINUIEditVoiceShortcutViewControllerDelegate
    {
        private ISubject<Unit> refreshSubject = new Subject<Unit>();

        public SiriShortcutsViewController(SiriShortcutsViewModel viewModel) : base(viewModel, nameof(SiriShortcutsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Resources.SiriShortcuts;

            DescriptionLabel.Text = Resources.SiriShortcutsDescription;
            HeaderView.RemoveFromSuperview();
            TableView.TableHeaderView = HeaderView;
            HeaderView.TranslatesAutoresizingMaskIntoConstraints = false;
            HeaderView.WidthAnchor.ConstraintEqualTo(TableView.WidthAnchor).Active = true;

            HeaderView.InsertSeparator();

            TableView.TableFooterView = new UIView();

            var tableViewSource = new SiriShortcutsTableViewSource(TableView);
            TableView.Source = tableViewSource;

            refreshSubject
                .SelectMany(getAllShortcuts())
                .SelectMany(toViewModels)
                .Select(toSections)
                .ObserveOn(new NSRunloopScheduler())
                .Subscribe(TableView.Rx().ReloadSections(tableViewSource))
                .DisposedBy(DisposeBag);

            tableViewSource.Rx().ModelSelected()
                .Subscribe(handleShortcutTap)
                .DisposedBy(DisposeBag);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            refreshSubject.OnNext(Unit.Default);
        }

        private void handleShortcutTap(SiriShortcutViewModel shortcut)
        {
            if (!shortcut.IsActive)
            {
                if (shortcut.Type == SiriShortcutType.CustomStart)
                {
                    ViewModel.NavigateToCustomTimeEntryShortcut.Execute();
                    return;
                }

                if (shortcut.Type == SiriShortcutType.CustomReport)
                {
                    ViewModel.NavigateToCustomReportShortcut.Execute();
                    return;
                }

                var intent = IosDependencyContainer.Instance.IntentDonationService.CreateIntent(shortcut.Type);
                var vc = new INUIAddVoiceShortcutViewController(new INShortcut(intent));
                vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                vc.Delegate = this;
                PresentViewController(vc, true, null);
            }
            else
            {
                var vc = new INUIEditVoiceShortcutViewController(shortcut.VoiceShortcut);
                vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                vc.Delegate = this;
                PresentViewController(vc, true, null);
            }
        }

        private IObservable<IEnumerable<SiriShortcut>> getAllShortcuts()
        {
            return IosDependencyContainer.Instance.IntentDonationService.GetCurrentShortcuts()
                .CombineLatest(ViewModel.GetUserWorkspaces(), filterByWorkspace)
                .SelectMany(filterByProject)
                .Select(shortcuts => SiriShortcut.DefaultShortcuts.Concat(shortcuts));
        }

        private IEnumerable<SiriShortcut> filterByWorkspace(IEnumerable<SiriShortcut> shortcuts, IEnumerable<IThreadSafeWorkspace> userWorkspaces)
        {
            var workspaceIds = userWorkspaces.Select(ws => ws.Id);

            return shortcuts.Where(shortcut =>
            {
                if (shortcut.Parameters.WorkspaceId.HasValue)
                {
                    return workspaceIds.Contains(shortcut.Parameters.WorkspaceId.Value);
                }

                return true;
            });
        }

        private IObservable<IEnumerable<SiriShortcut>> filterByProject(IEnumerable<SiriShortcut> shortcuts)
        {
            return shortcuts
                .Select(shortcut =>
                {
                    if (shortcut.Parameters.ProjectId.HasValue)
                    {
                        return ViewModel.GetProject(shortcut.Parameters.ProjectId.Value)
                            .SelectValue(shortcut)
                            .Catch(Observable.Empty<SiriShortcut>());
                    }

                    return Observable.Return(shortcut);
                })
                .Merge().ToList();
        }


        private IObservable<IEnumerable<SiriShortcutViewModel>> toViewModels(IEnumerable<SiriShortcut> shortcuts)
        {
            return shortcuts
                .Select(shortcut =>
                {
                    if (shortcut.VoiceShortcut != null && shortcut.Parameters.ProjectId.HasValue)
                    {
                        return ViewModel.GetProject(shortcut.Parameters.ProjectId.Value)
                            .Select(project => new SiriShortcutViewModel(shortcut, project));
                    }

                    return Observable.Return(new SiriShortcutViewModel(shortcut));
                })
                .ToObservable().Merge().ToList();
        }

        private IImmutableList<ShortcutSection> toSections(IEnumerable<SiriShortcutViewModel> shortcuts)
        {
            var allShortcuts = shortcuts
                .Aggregate(new List<SiriShortcutViewModel>(), (acc, shortcut) =>
                {
                    if (shortcut.Type != SiriShortcutType.CustomStart && shortcut.Type != SiriShortcutType.CustomReport)
                    {
                        var index = acc.IndexOf(s => shortcut.Type == s.Type);
                        if (index != -1)
                        {
                            acc[index] = shortcut;
                            return acc;
                        }
                    }

                    acc.Add(shortcut);
                    return acc;
                })
                .OrderBy(shortcut => !shortcut.IsActive)
                .ToList();

            return ImmutableList.Create(
                new ShortcutSection(
                    Resources.SiriShortcutsTimerShortcuts,
                    allShortcuts.Where(s => s.IsTimerShortcut())
                ),
                new ShortcutSection(
                    Resources.SiriShortcutsReportsShortcuts,
                    allShortcuts.Where(s => s.IsReportsShortcut())
            ));
        }

        private long? stringToLong(string str)
        {
            return long.TryParse(str, out var i) ? i : (long?)null;
        }

        // IINUIAddVoiceShortcutViewControllerDelegate

        public void DidFinish(INUIAddVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            refreshSubject.OnNext(Unit.Default);
            controller.DismissViewController(true, null);
        }

        public void DidCancel(INUIAddVoiceShortcutViewController controller)
        {
            controller.DismissViewController(true, null);
        }

        // IINUIEditVoiceShortcutViewControllerDelegate

        public void DidUpdate(INUIEditVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            refreshSubject.OnNext(Unit.Default);
            controller.DismissViewController(true, null);
        }

        public void DidDelete(INUIEditVoiceShortcutViewController controller, NSUuid deletedVoiceShortcutIdentifier)
        {
            refreshSubject.OnNext(Unit.Default);
            controller.DismissViewController(true, null);
        }

        public void DidCancel(INUIEditVoiceShortcutViewController controller)
        {
            controller.DismissViewController(true, null);
        }
    }
}

