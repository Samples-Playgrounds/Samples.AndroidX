using NSubstitute;
using Toggl.Core.Interactors.Notifications;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Notifications
{
    public class UnscheduleAllNotificationsInteractorTests
    {
        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private readonly UnscheduleAllNotificationsInteractor interactor;

            public TheExecuteMethod()
            {
                interactor = new UnscheduleAllNotificationsInteractor(NotificationService);
            }

            [Fact, LogIfTooSlow]
            public void UnschedulesAllNotifications()
            {
                interactor.Execute();

                NotificationService.Received().UnscheduleAllNotifications();
            }
        }
    }
}
