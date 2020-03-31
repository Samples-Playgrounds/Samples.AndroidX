using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using System;
using Toggl.Core.Sync.States;
using Xunit;

namespace Toggl.Core.Tests.Sync.States
{
    public sealed class InvalidTransitionStateTests
    {
        [Fact, LogIfTooSlow]
        public void ThrowsWhenTheConstructorArgumentIsNull()
        {
            Action creatingState = () => new InvalidTransitionState(null);

            creatingState.Should().Throw<ArgumentNullException>();
        }

        [Theory, LogIfTooSlow]
        [InlineData("")]
        [InlineData("          ")]
        public void ThrowsWhenTheConstructorArgumentIsEmpty(string message)
        {
            Action creatingState = () => new InvalidTransitionState(message);

            creatingState.Should().Throw<ArgumentException>();
        }

        [Fact, LogIfTooSlow]
        public void DoesNotThrowWhenNobodySubscribesToTheReturnedObservable()
        {
            var state = new InvalidTransitionState("Error.");

            state.Start();
        }

        [Fact, LogIfTooSlow]
        public void ThrowsInvalidOperationException()
        {
            Exception caughtException = null;
            var state = new InvalidTransitionState("Some message");

            var observable = state.Start();
            observable.Subscribe(_ => { }, (Exception exception) => caughtException = exception);

            caughtException.Should().NotBeNull();
            caughtException.Should().BeOfType<InvalidOperationException>();
        }

        [Property]
        public void ThrowsInvalidOperationExceptionWithASpecificMessage(NonEmptyString message)
        {
            if (message.Get.Trim().Length == 0) return;

            Exception caughtException = null;
            var state = new InvalidTransitionState(message.Get);

            var observable = state.Start();
            observable.Subscribe(_ => { }, (Exception exception) => caughtException = exception);

            caughtException.Message.Should().Be($"Invalid transition: {message}");
        }
    }
}
