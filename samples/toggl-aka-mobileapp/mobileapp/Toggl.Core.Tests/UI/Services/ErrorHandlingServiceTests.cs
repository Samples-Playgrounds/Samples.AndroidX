using FluentAssertions;
using NSubstitute;
using System;
using Toggl.Core.Exceptions;
using Toggl.Core.Services;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Core.UI.ViewModels;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Network;
using Toggl.Storage.Models;
using Toggl.Storage.Settings;
using Xunit;

namespace Toggl.Core.Tests.UI.Services
{
    public sealed class ErrorHandlingServiceTests
    {
        public abstract class BaseErrorHandlingServiceTests
        {
            protected INavigationService NavigationService { get; }
            protected IAccessRestrictionStorage AccessRestrictionStorage { get; }
            protected IErrorHandlingService ErrorHandlingService { get; }
            protected IDatabaseUser User { get; }

            public BaseErrorHandlingServiceTests()
            {
                User = Substitute.For<IDatabaseUser>();
                var token = Guid.NewGuid().ToString();
                User.ApiToken.Returns(token);
                NavigationService = Substitute.For<INavigationService>();
                AccessRestrictionStorage = Substitute.For<IAccessRestrictionStorage>();
                ErrorHandlingService =
                    new ErrorHandlingService(NavigationService, AccessRestrictionStorage);
            }
        }

        public sealed class TheConstructor
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useNavigationService, bool useAccessRestrictionStorage)
            {
                var navigationService = useNavigationService ? Substitute.For<INavigationService>() : null;
                var accessRestrictionStorage = useAccessRestrictionStorage ? Substitute.For<IAccessRestrictionStorage>() : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new ErrorHandlingService(navigationService, accessRestrictionStorage);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheApiDeprecatedException : BaseErrorHandlingServiceTests
        {
            private ApiDeprecatedException exception => new ApiDeprecatedException(Substitute.For<IRequest>(), Substitute.For<IResponse>());

            [Fact, LogIfTooSlow]
            public void ReturnsTrueForApiDeprecatedException()
            {
                var result = ErrorHandlingService.TryHandleDeprecationError(exception);

                result.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void RestricsAccessForApiDeprecatedException()
            {
                ErrorHandlingService.TryHandleDeprecationError(exception);

                AccessRestrictionStorage.Received().SetApiOutdated();
            }

            [Fact, LogIfTooSlow]
            public void NavigatesToOutdatedAppViewModelForApiDeprecatedException()
            {
                ErrorHandlingService.TryHandleDeprecationError(exception);

                NavigationService.Received().Navigate<OutdatedAppViewModel>(null);
            }
        }

        public sealed class TheClientDeprecatedException : BaseErrorHandlingServiceTests
        {
            private ClientDeprecatedException exception => new ClientDeprecatedException(Substitute.For<IRequest>(), Substitute.For<IResponse>());

            [Fact, LogIfTooSlow]
            public void ReturnsTrueForClientDeprecatedException()
            {
                var result = ErrorHandlingService.TryHandleDeprecationError(exception);

                result.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void RestricsAccessForClientDeprecatedException()
            {
                ErrorHandlingService.TryHandleDeprecationError(exception);

                AccessRestrictionStorage.Received().SetClientOutdated();
            }

            [Fact, LogIfTooSlow]
            public void NavigatesToOutdatedAppViewModelForClientDeprecatedException()
            {
                ErrorHandlingService.TryHandleDeprecationError(exception);

                NavigationService.Received().Navigate<OutdatedAppViewModel>(null);
            }
        }

        public sealed class TheNoWorkspaceException : BaseErrorHandlingServiceTests
        {
            private NoWorkspaceException exception => new NoWorkspaceException();

            [Fact, LogIfTooSlow]
            public void ReturnsTrueForNoWorkspaceException()
            {
                var result = ErrorHandlingService.TryHandleNoWorkspaceError(exception);

                result.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void RestrictsAccessForNoWorkspaceException()
            {
                ErrorHandlingService.TryHandleNoWorkspaceError(exception);

                AccessRestrictionStorage.Received().SetNoWorkspaceStateReached(true);
            }
        }

        public sealed class TheOtherExceptionsThanDeprecationExceptions : BaseErrorHandlingServiceTests
        {
            private Exception exception => new Exception();

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForDifferentExceptions()
            {
                ErrorHandlingService.TryHandleDeprecationError(exception);

                NavigationService.DidNotReceive().Navigate<OutdatedAppViewModel>(null);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotRestrictAccessForDifferentExceptions()
            {
                ErrorHandlingService.TryHandleDeprecationError(exception);

                AccessRestrictionStorage.DidNotReceive().SetApiOutdated();
                AccessRestrictionStorage.DidNotReceive().SetClientOutdated();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotNavigateForDifferentExceptions()
            {
                ErrorHandlingService.TryHandleDeprecationError(exception);

                NavigationService.DidNotReceive().Navigate<OutdatedAppViewModel>(null);
            }
        }

        public sealed class TheUnauthorizedException : BaseErrorHandlingServiceTests
        {
            private UnauthorizedException exception => new UnauthorizedException(createRequest(), Substitute.For<IResponse>());

            private IRequest createRequest()
            {
                var request = Substitute.For<IRequest>();
                var headers = new[] { Credentials.WithApiToken(User.ApiToken).Header };
                request.Headers.Returns(headers);
                return request;
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTrueForClientDeprecatedException()
            {
                var result = ErrorHandlingService.TryHandleUnauthorizedError(exception);

                result.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void RestricsAccessForClientDeprecatedException()
            {
                ErrorHandlingService.TryHandleUnauthorizedError(exception);

                AccessRestrictionStorage.Received().SetUnauthorizedAccess(Arg.Is(User.ApiToken));
            }

            [Fact, LogIfTooSlow]
            public void NavigatesToOutdatedAppViewModelForClientDeprecatedException()
            {
                ErrorHandlingService.TryHandleUnauthorizedError(exception);

                NavigationService.Received().Navigate<TokenResetViewModel>(null);
            }

            [Fact, LogIfTooSlow]
            internal void ReturnsTrueButDoesNotNavigateOrSetUnathorizedAccessFlagWhenTheApiTokenCannotBeExtractedFromTheRequest()
            {
                var request = Substitute.For<IRequest>();
                request.Headers.Returns(new HttpHeader[0]);
                var exceptionWithoutApiToken = new UnauthorizedException(request, Substitute.For<IResponse>());

                var handled = ErrorHandlingService.TryHandleUnauthorizedError(exceptionWithoutApiToken);

                handled.Should().BeTrue();
                NavigationService.DidNotReceive().Navigate<TokenResetViewModel>(null);
                AccessRestrictionStorage.DidNotReceive().SetUnauthorizedAccess(Arg.Any<string>());
            }
        }

        public sealed class TheOtherExceptionsThanUnauthorizedException : BaseErrorHandlingServiceTests
        {
            private Exception exception => new Exception();

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForDifferentExceptions()
            {
                ErrorHandlingService.TryHandleUnauthorizedError(exception);

                NavigationService.DidNotReceive().Navigate<TokenResetViewModel>(null);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotRestrictAccessForDifferentExceptions()
            {
                ErrorHandlingService.TryHandleUnauthorizedError(exception);

                AccessRestrictionStorage.DidNotReceive().SetUnauthorizedAccess(Arg.Any<string>());
            }

            [Fact, LogIfTooSlow]
            public void DoesNotNavigateForDifferentExceptions()
            {
                ErrorHandlingService.TryHandleUnauthorizedError(exception);

                NavigationService.DidNotReceive().Navigate<TokenResetViewModel>(null);
            }
        }

        public sealed class TheTryHandleNoDefaultWorkspaceError : BaseErrorHandlingServiceTests
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueForNoWorkspaceException()
            {
                var result = ErrorHandlingService.TryHandleNoDefaultWorkspaceError(new NoDefaultWorkspaceException());

                result.Should().BeTrue();
            }

            [Theory, LogIfTooSlow]
            [InlineData(typeof(Exception))]
            [InlineData(typeof(NoWorkspaceException))]
            [InlineData(typeof(FormatException))]
            [InlineData(typeof(NoRunningTimeEntryException))]
            public void ReturnsFalseForAnyOtherException(Type exceptionType)
            {
                var exception = (Exception)Activator.CreateInstance(exceptionType);

                var result = ErrorHandlingService.TryHandleNoDefaultWorkspaceError(exception);

                result.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void RestrictsAccessForNoWorkspaceException()
            {
                ErrorHandlingService.TryHandleNoDefaultWorkspaceError(new NoDefaultWorkspaceException());

                AccessRestrictionStorage.Received().SetNoDefaultWorkspaceStateReached(true);
            }
        }
    }
}
