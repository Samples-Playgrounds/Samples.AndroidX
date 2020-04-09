# The Android Startup

This document explains the Android startup and also how the ViewModels are injected on `Activities` and `Fragments`, because of their "special" lifecycle.

## Startup

We are using a custom application, located at `Toggl.Droid/Startup/TogglApplication.cs`, that of course, extends the android `Application`. For purposes of the application startup, we are only overriding the `OnCreate`, which does the following:
1. Initializes `Firebase`
1. Initializes `AppCenter`
1. Initializes `AndroidDependencyContainer`, by `calling AndroidDependencyContainer.EnsureInitialized`.

Calling `AndroidDependencyContainer.EnsureInitialized` makes sure our custom dependency container and ViewModel framework works properly. Failing to call it will simply crash the app.

Once the application has been started, it will stay there until there's at least one activity or service running in foreground/background. The system might kill an app in background to free resources based on some rules (please read the [official docs](https://developer.android.com/topic/performance/memory-overview.html) for more details on this topic). 

We have the following entry-points for the application:

1. Cold start from the launcher
2. Url navigation (Deeplinks)
3. Application shortcuts
4. Notifications
5. The "Track with Toggl" option when selecting text from ther apps 

Each of the options above will start the application if it's not there yet. The Application is not recreated after it has been created once and has not been killed, either by the system or by the user.

With the application started, an intent will be generated and it will be handled by the `SplashScreen`

### SplashScreen

Located at: `Toggl.Droid/Startup/SplashScreen.cs`

The `SplashScreen` is our app main launcher and also responsible for handling `toggl://` urls and specialized "Track with Toggl" intents.

The `SplashScreen` does the following:

1. Registers for timezone changed broadcasts to update our `ITimeService`
1. Registers for application lifecycle events for use with the `BackgroundService` (currently, disabled for android)
1. Creates an instance of the `App` helper and calls `NavigateIfUserDoesNotHaveFullAccess`, which will navigate to non-authenticated-user screens depending on the state of the user auth, api token, or app version.
1. Clears any existing view model cache in the application. 
1. If the user does have full access to the app, it will either:
   1. Navigate to the root activity (currently `MainTabBarActivity`) if there's no data passed in the intent
   1. Handle the data in the `SplashScreen` intent by calling the `SplashScreen.DeepLink`'s`handleDeepLink` with `Uri` parsed from the intent data.
   
### SplashScreen.DeepLink

Located at: `Toggl.Droid/Startup/SplashScreen.Deeplink.cs`

The main method here is `handleDeeplink`, which simply uses the `IDeeplinkParser` to parse the Uri, uses the `DeeplinkParametersExtensions` to handle the common cases, simply starting the root main activity after that. For the cases that need special care, needed view models are located, parameters set, activity extras set if necessary and intents are fired as necessary.

The intents created here have the `ActivityFlags.ClearTask | ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.TaskOnHome` flags set, which for all purposes (when this as written), creates a "fresh start" for the application, killing any activities and view model state that could be there. 

In special, `TaskOnHome` makes the intents fired here cause the same effect as if the application was launched from the launcher, that is, the splash screen won't be re-launched if the user press home then comes back to the application by clicking on the launcher.

`ClearTask | ClearTop | NewTask`, together, make sure that any leftover activities are destroyed.

### Activities, Fragments and DialogFragments Presenters and the ViewModelCache

Due to how the Android framework handles `Activities` and `Fragments` restoration, they need to have empty constructors. If something needs to injected into `Activities` or `Fragments` for them to work, field injection, service locators or other methods need to be used. (Parameters can be passed through a `bundle` for `Fragments` and through intents for `Activities`)

The `ViewModelCache` is initialized in the `AndroidDependencyContainer` and is used in the `ActivityPresenter`, `DialogFragmentPresenter` presenters before we fire activity intents and show dialog fragments, such that the `ViewModels` can be fetched during the beginning of their lifecycles.

On the `ReactiveActivity`, the `ViewModelCache` is used in the `OnCreate` to fetch the `ViewModel`, being safe to use it after calling the base constructor on `Activities` that inherit it. The `ViewModelCache` is called to clear a `ReactiveActivity`'s ViewModel cache when the `ViewModel`'s `Close` method is called.

Due to constraints on restoration, the `ReactiveActivity` needs to have a sealed `OnCreate` method. This is to ensure that our bailing out mechanism (the one that falls back to the `SplashScreen` if needed) is called. There are other extension points (some of which are mandatory), so there's no need for other activities to override that method.

On the `ReactiveDialogFragment`, the `ViewModelCache` is used to get the `ViewModel` in the `OnViewCreated` and cleared similarly when the `ViewModel`'s `Close` is called. Please note that the `OnCreateView` should only be used to initialize the views (call `InitializeViews`) and the Dialog won't have a reference to the `ViewModel` at that point.
It's only safe to use the `ViewModel` after calling `base.OnViewCreated` in the classes that inherit from the `ReactiveDialogFragment`.

The `ReactiveTabFragment`s are exceptions and their ViewModel's lifecycle is controlled by the `MainTabBarViewModel`, being the viewModels created and initialized by the `MainTabBarViewModel`. 

The `ViewModelCache`'s `ClearAll` should be called when `Activities` need to be started while clearing the stack (eg. when an activity is called with `ClearTop | ClearTask | NewTask` flags) to prevent the cache from keeping a reference to a `ViewModel` when not in use.
