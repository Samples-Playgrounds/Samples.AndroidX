﻿using FluentAssertions;
using FsCheck;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SelectClientViewModelTests
    {
        public abstract class SelectClientViewModelTest : BaseViewModelTests<SelectClientViewModel, SelectClientParameters, long?>
        {
            protected SelectClientParameters Parameters { get; }
                = SelectClientParameters.WithIds(10, null);

            protected override SelectClientViewModel CreateViewModel()
               => new SelectClientViewModel(InteractorFactory, NavigationService, SchedulerProvider, RxActionFactory);

            protected ITestableObserver<IEnumerable<SelectableClientBaseViewModel>> CreateClientsObserver()
            {
                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableClientBaseViewModel>>();
                ViewModel.Clients.Subscribe(observer);
                return observer;
            }

            protected List<IThreadSafeClient> GenerateClientList()
                => GenerateClientList(Enumerable.Range(1, 10).Select(i => i.ToString()).ToArray());

            protected List<IThreadSafeClient> GenerateClientList(string[] clientNames) =>
                Enumerable.Range(0, clientNames.Length).Select(i =>
                {
                    var client = Substitute.For<IThreadSafeClient>();
                    client.Id.Returns(i + 1);
                    client.Name.Returns(clientNames[i]);
                    return client;
                }).ToList();
        }

        public sealed class TheConstructor : SelectClientViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useInteractorFactory,
                bool useNavigationService,
                bool useSchedulerProvider,
                bool useRxActionFactory)
            {
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SelectClientViewModel(interactorFactory, navigationService, schedulerProvider, rxActionFactory);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheInitializeMethod : SelectClientViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task AddsAllClientsToTheListOfSuggestions()
            {
                var clients = GenerateClientList();
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
                await ViewModel.Initialize(Parameters);
                var observer = CreateClientsObserver();

                TestScheduler.Start();

                observer.LastEmittedValue().Count().Should().Equals(clients.Count);
            }

            [Fact, LogIfTooSlow]
            public async Task AddsANoClientSuggestion()
            {
                var clients = GenerateClientList();
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
                await ViewModel.Initialize(Parameters);
                var observer = CreateClientsObserver();

                TestScheduler.Start();

                observer.LastEmittedValue().First().Name.Should().Be(Resources.NoClient);
                observer.LastEmittedValue().First().Should().BeOfType<SelectableClientViewModel>();
            }

            [Fact, LogIfTooSlow]
            public async Task SetsNoClientAsSelectedIfTheParameterDoesNotSpecifyTheCurrentClient()
            {
                var clients = GenerateClientList();
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
                await ViewModel.Initialize(Parameters);
                var observer = CreateClientsObserver();

                TestScheduler.Start();

                observer.LastEmittedValue().Single(c => c.Selected).Name.Should().Be(Resources.NoClient);
            }

            [Theory, LogIfTooSlow]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(6)]
            [InlineData(7)]
            [InlineData(8)]
            [InlineData(9)]
            public async Task SetsTheAppropriateClientAsTheCurrentlySelectedOne(int id)
            {
                var parameter = SelectClientParameters.WithIds(10, id);
                var clients = GenerateClientList();
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
                await ViewModel.Initialize(parameter);
                var observer = CreateClientsObserver();

                TestScheduler.Start();

                observer.LastEmittedValue().Single(c => c.Selected).Name.Should().Be(id.ToString());
            }

            [Fact, LogIfTooSlow]
            public async Task ClientListIsSorted()
            {
                var clientNames = new[] { "Microsoft", "Amazon", "Google", "Steam", "Facebook" };
                var clients = GenerateClientList(clientNames);
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
                await ViewModel.Initialize(Parameters);
                var observer = CreateClientsObserver();

                TestScheduler.Start();

                var resultClientsNames = observer.LastEmittedValue().Select(vm => vm.Name);
                // First item is skipped because when the filter is empty,
                // the collection contains 'No client' item which is always shown at the beginning
                resultClientsNames.Skip(1).Should().BeInAscendingOrder();
            }
        }

        public sealed class TheCloseWithDefaultResultMethod : SelectClientViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.Initialize(Parameters);

                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsNull()
            {
                await ViewModel.Initialize(Parameters);

                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                (await ViewModel.Result).Should().BeNull();
            }
        }

        public sealed class TheSelectClientAction : SelectClientViewModelTest
        {
            private readonly SelectableClientViewModel client = new SelectableClientViewModel(9, "Client A", false);

            public TheSelectClientAction()
            {
                var clients = GenerateClientList();
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
            }

            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.Initialize(Parameters);

                ViewModel.SelectClient.Execute(client);
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSelectedClientId()
            {
                await ViewModel.Initialize(Parameters);

                ViewModel.SelectClient.Execute(client);
                TestScheduler.Start();

                (await ViewModel.Result).Should().Be(client.Id);
            }

            [Fact, LogIfTooSlow]
            public async Task CreatesANewClientWithTheGivenNameInTheCurrentWorkspace()
            {
                long workspaceId = 10;
                var newClient = new SelectableClientCreationViewModel("Some name of the client");
                await ViewModel.Initialize(Parameters);

                ViewModel.SelectClient.Execute(newClient);
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .CreateClient(Arg.Is(newClient.Name), Arg.Is(workspaceId))
                    .Execute();
            }

            [Theory, LogIfTooSlow]
            [InlineData("   abcde", "abcde")]
            [InlineData("abcde     ", "abcde")]
            [InlineData("  abcde ", "abcde")]
            [InlineData("abcde  fgh", "abcde  fgh")]
            [InlineData("      abcd\nefgh     ", "abcd\nefgh")]
            public async Task TrimsNameFromTheStartAndTheEndBeforeSaving(string name, string trimmed)
            {
                await ViewModel.Initialize(Parameters);

                ViewModel.SelectClient.Execute(new SelectableClientCreationViewModel(name));
                TestScheduler.Start();

                await InteractorFactory
                    .Received()
                    .CreateClient(Arg.Is(trimmed), Arg.Any<long>())
                    .Execute();
            }

        }

        public sealed class TheClientsProperty : SelectClientViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task UpdateWhenFilterTextChanges()
            {
                var clients = GenerateClientList();
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
                await ViewModel.Initialize(Parameters);
                var observer = CreateClientsObserver();

                ViewModel.FilterText.OnNext("0");
                TestScheduler.Start();

                observer.LastEmittedValue().Count().Should().Equals(1);
            }

            [Fact, LogIfTooSlow]
            public async Task AddCreationCellWhenNoMatchingSuggestion()
            {
                var clients = GenerateClientList();
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
                await ViewModel.Initialize(Parameters);
                var observer = CreateClientsObserver();

                var nonExistingClientName = "Some none existing name";
                ViewModel.FilterText.OnNext(nonExistingClientName);
                TestScheduler.Start();

                observer.LastEmittedValue().First().Name.Should().Equals(nonExistingClientName);
                observer.LastEmittedValue().First().Should().BeOfType<SelectableClientCreationViewModel>();
            }

            [Theory, LogIfTooSlow]
            [InlineData(" ")]
            [InlineData("\t")]
            [InlineData("\n")]
            [InlineData("               ")]
            [InlineData("      \t  \n     ")]
            [InlineData(null)]
            public async Task DoesNotSuggestCreatingClientsWhenTheDescriptionConsistsOfOnlyWhiteCharacters(string name)
            {
                var clients = GenerateClientList();
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
                await ViewModel.Initialize(Parameters);
                var observer = CreateClientsObserver();

                ViewModel.FilterText.OnNext(name);
                TestScheduler.Start();

                observer.LastEmittedValue().First().Should().NotBeOfType<SelectableClientCreationViewModel>();
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotSuggestCreationWhenTextMatchesAExistingClientName()
            {
                var clients = GenerateClientList();
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
                await ViewModel.Initialize(Parameters);
                var observer = CreateClientsObserver();

                ViewModel.FilterText.OnNext(clients.First().Name);
                TestScheduler.Start();

                observer.LastEmittedValue().First().Should().NotBeOfType<SelectableClientCreationViewModel>();
            }

            [Fact, LogIfTooSlow]
            public async Task ClientListIsSortedAfterFilterChange()
            {
                var clientNames = new[] { "Microsoft", "Amazon", "Google", "Steam", "Facebook" };
                var clients = GenerateClientList(clientNames);
                InteractorFactory.GetAllClientsInWorkspace(Arg.Any<long>())
                    .Execute()
                    .Returns(Observable.Return(clients));
                await ViewModel.Initialize(Parameters);
                var observer = CreateClientsObserver();

                ViewModel.FilterText.OnNext("a");
                TestScheduler.Start();

                var resultClientsNames = observer.LastEmittedValue().Select(vm => vm.Name);
                // First item is skipped because when the filter is not empty,
                // the collection contains 'Create client XYZ' item which is always shown at the beginning
                resultClientsNames.Skip(1).Should().BeInAscendingOrder();
            }
        }
    }
}
