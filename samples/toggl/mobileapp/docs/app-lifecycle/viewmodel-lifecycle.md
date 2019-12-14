# ViewModel Lifecycle

## Navigation

 Navigating from one view to other is as simple as calling the `Navigate` method and supplying the type of the ViewModel you want to show. The Navigate method returns a `Task`, and said task only completes when the view you navigated to is closed, whether said view returns a value or not.

 Navigation only works when a ViewModel implements `ViewModel<TInput, TOutput>`. Helper types were introduced for ViewModels that don't need an input and/or do not yield and output. This means that, in order to navigate to a ViewModel that takes some kind of `Tinput`, it's mandatory to provide said input.

 ## IView interface

This was introduced as a way to get rid of the need to keep track of what Activity/ViewController is currently on top of the stack. Since this concept is not natural to Android/iOS, keeping track of it was cumbersome, required lots of static state being kept and has caused us a lot of stress, especially on Android.

The new approach we decided to take was instead give the ViewModel a reference to the View it's attached to. This View will attach itself to the ViewModel when it's displayed and detach itself when no longer in use. This will allow the ViewModel to perform tasks that are heavily dependent on using Activities in a more natural way.

This refactor made us get rid of the `IGoogleService`, `IDialogService` and half of the `IPermissionService` interfaces. The methods of those interfaces are now part of `IView`, since a reference to the currently visible view has always been needed anyways.

Attach is called on `ViewDidLoad` for ViewControllers, on `OnCreate` for Activities and on `OnViewCreated` for Fragments.
Dettach is called on the same places as ViewDestroyed for ViewControllers, `OnDestroy` for activities and `OnDestroyView` for Fragments.  

 ## Lifecycle methods

 Each ViewModel has a set of methods that get called and can be used to better reflect what is going on in the app

Legend:
:checkered_flag: This method is guaranteed to always run on the main thread. 
:robot: `Activity` equivalent of the method
:gem: `Fragment` equivalent of the method
:apple: `UIViewController` equivalent of the method

 ### Constructor

 The constructor should receive all of the ViewModel's dependencies so they can be easily tested. Every property that can be initialized in the constructor *should* be initialized in the constructor, since it can then be safely made immutable on a .net level.

 ### Initialize

 This method is called right after construction. It's asynchronous to simplify things up. It's guaranteed to finish before presentation starts, so try to keep it simple. The idea is to move this method and make it return void, to prevent people from taking too long to finish here and force the usage of `IObservable` and subscriptions instead.

 It replaces the `Init`, `Prepare`, `Prepare<T>` and `Initialize` methods from MvvmCross.

### ViewAppearing :checkered_flag:

This method is called right before the View is made visible to the user

:apple: `ViewWillAppear`
:robot: `OnStart`
:gem: `OnStart`

### ViewAppeared :checkered_flag:

This method is called right after the View is made visible to the user

:apple: `ViewDidAppear`
:robot: `OnResume`
:gem: `OnResume`

### ViewDisappearing :checkered_flag:

This method is called right before the View is disappears to the user

:apple: `ViewWillDisappear`
:robot: `OnPause`
:gem: `OnPause`

### ViewDisappeared :checkered_flag:

This method is called right after the View is disappears to the user

:apple: `ViewDidDisappear`
:robot: `OnStop` 
:gem: `OnStop`

### ViewDestroyed :checkered_flag:

This method is called right after the View has been destroyed

:apple: `DidMoveToParentViewController`, for children. `Dismiss` for modals. Called when navigating away on Roots.
:robot: `OnDestroy`
:gem: `OnDestroyView`
