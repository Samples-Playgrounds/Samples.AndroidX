Push Notifications
==================

Our app uses push notifications to trigger sync when a change occurs on the server before the user syncs to the app. The idea is to keep time entries in sync at all times (as much as the mobile operating system allows us to).

It is important to keep in mind that it is possible that a notification is not delivered to the app by the device (Apple Push Notification service or Firebase Cloud Messaging) or the operating system does not call our app when it is in background (to preserve battery life). It therefore isn't possible for us to guarantee that the app will always be in sync with the server even if the user is online all the time and the server sends push notifications to our app frequently.

> __NOTE__: Firebase is only enabled for non-DEBUG builds, this means that silent remote notifications can only be tested through AppStore (including Test Flight) or AdHoc builds. If you need to test it on device you'll have to remove the preprocessor directives in AppDelegate (iOS) and TogglApplication (Android) and add the necessary keys to GoogleService-info.plist (iOS) and google-services.json (Android)

This document covers several important parts of the whole implementation:

1. What the apps do when a push notification is received
1. Firebase Console setup
1. FCM token lifecycle and communication with the API server
1. iOS specific documentation
1. Android specific documentation

## App Response To Push Notifications

The app reacts differently when a push notification is received depending on the state of the app:
- when the app is in the foreground and it is being used by the user, we pull just the time entries first to be able to update the currently running time entry as soon as possible, but then we also immediately execute a full sync
- when the app is in the background, we fetch just the time entries using "sync light" (see below).

We use Firebase Remote Config to control the rollout of the feature to the users. The config key is `handle_push_notifications`.

### "Sync Light"

The name _Sync Light_ is a nickname for a simplified lightweight sync workflow triggered by `ISyncManager.PullTimeEntries()`. It sends just 1 HTTP requests and it is intended to finish much faster than regular _Full Sync_ and so it could be used when the app is in background and we have a limited time to update the app. The workflow is described in detail in [the syncing docs](syncing/pull-time-entries.md).

## Firebase Console Setup

We use the `Toggl Mobile` project for push notification. This is the same project we use for analytics and performance measurement. The APNs auth key is uploaded to all of our profiles - Debug, AdHoc, and production. Other than that, there is nothing interesting about the Firebase setup.

## FCM Token Lifecycle

During push sync, we try to push an FCM token if one of these conditions is met:
- the FCM token hasn't been pushed to the server yet (e.g., right after the user logs in, or if the FCM token changes),
- it is more than 5 days since the token was pushed to the server.
If the HTTP request fails, we do not repeat it right away, but we will attempt the next time, because the first condition still hasn't been met.

If the FCM token changes, our app is notified by the operating system, and we attempt to push the new token to the server right away. If this request fails (e.g., the user is offline at the moment), we will ignore the failure and the token will be synced later when the app syncs.

When the user logs out of the app, we invalidate the currently valid FCM token to stop receiving further notifications. We also try to inform the server so it can remove the token from the DB but we don't repeat the request if it fails, because the token stored on the server is useless anyway.

We use Firebase Remote Config to control whether the tokens are pushed to the server or not. The configuration key is `register_push_notifications_token_with_server`.

The API V9 endpoints we use are `POST|DELETE api/v9/me/push_services`. The documentation for these endpoints can be found [in Notion](https://www.notion.so/API-endpoints-d8b2ed5a93d74d8893f1862eb57eb903).

## iOS Specific Documentation

### Configuration

In order for remote push notifications to work we need to add the `remote-notification` value to the `UIBackgroundModes` key in the `info.plist`. This allows the app to be launched or resumed in background when a push notification arrives and gives it a small amount of time to download the new content.

We also need to add the `aps-environment` key to the `Entitlements.plist` file with a value of `development`. This will be replaced by `production` by the build script for production builds.

> NOTE: The system will launch the app (or wake it from the suspended state) and put it in the background state. However, it will not automatically launch the app if the user has force-quit it. In that situation, the user must relaunch your app or restart the device before the system attempts to launch the app automatically again.

### Code

When the app starts we need to set the `AppDelegate` as a delegate for the FCM instance.

The implementation of that delegate and the rest of the platform specific code is in `AppDelegate.Notifications.cs`.

When `RegisteredForRemoteNotifications` is called, we set the APNs token to the FCM (Firebase Cloud Messaging) shared instance.

`IMessagingDelegate` has a method `DidReceiveRegistrationToken` that's called everytime the token changes. In that method we subscribe to the push notifications (which also sends the token to the server) via the appropriate interactor, only if the user is logged in.

The remaining method in that file is `DidReceiveRemoteNotification` which gets called when the app is launched into background (or waked) from a remote notification or when a notification is received while the app is in foreground. We check if the user is logged in and then call the appropriate syncing interactor depending on the state of the app (foreground or background)

## Android Specific Documentation

The push notifications handling is done by the book, following Firebase and Xamarin guidelines; except that we are using an outdated version of the Firebase libraries, because of currently available Xamarin libraries and the incompatibility issues with our app. Basically there's some missing methods in `Firebase.Perf`, we have a custom built library and updating to use more recent Firebase libraries is problematic. Please check https://github.com/toggl/mobileapp/issues/3542, https://github.com/toggl/mobileapp/pull/5303 and https://github.com/xamarin/GooglePlayServicesComponents/issues/151 for more information.

There are three important classes involved in droid specifics:
- `TogglFirebaseIIDService`:

  Responsible for reacting to FCM token changes, triggering registration with backend if necessary.

- `TogglFirebaseMessagingService`:

  Responsible for reacting to incoming push notifications, scheduling a `SyncJobService` that will actually trigger a sync when run.

- `SyncJobService`:

  One of these jobs is scheduled when a push notification arrives. A simple lock based on shared preferences is used to prevent multiple sync jobs to be scheduled at the same time.
  When the job runs, it will check whether or not the application is in background or foreground, running the appropriate sync strategy described in [one of the sections above](#app-response-to-push-notifications).

  This job is common android `JobService`, scheduled using the `JobScheduler` and it's up to the system to decided when it runs. Ideally, it should run as soon as the device has internet access, but it can take more time or run only when the app comes to foreground (e.g on lower end devices; when the device is on power saving mode; when the user has blacklisted the app). On android devices with API levels > Pie, an extra job build option is put to make sure the job runs as soon as possible when the app is in foreground.
