namespace Toggl.Shared
{
    public struct PushNotificationsConfiguration
    {
        public bool RegisterPushNotificationsTokenWithServer { get; }
        public bool HandlePushNotifications { get; }

        public PushNotificationsConfiguration(bool registerPushNotificationsTokenWithServer, bool handlePushNotifications)
        {
            RegisterPushNotificationsTokenWithServer = registerPushNotificationsTokenWithServer;
            HandlePushNotifications = handlePushNotifications;
        }
    }
}
