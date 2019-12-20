## Binding Views to ViewModels Cookbook

* We have started to move away from Mvx Magic (aka ✨) in the way it deals with the bindings and started using RX bindings. In the transitional period, we used different ways to achieve the same thing. That has created lots of bugs and inconsistencies, but over the time we realized and fixed many of the internal mistakes we've made.
* However, we still need the guidelines for how we should approach the binding dilemmas and resolve all instances the same way. That will be easier to comprehend, review and fix.
* This document proposes the way we should approach binding and while not all of the code adheres to those guidelines, a slow switch could be made over time to be as consistent as possible. Also the document describes several use cases and what to do in each of them on both the **ViewModel** side as well as the **View** side.

### 📘 Set-Once variable (one time binding)
🏆 **When.**  A ViewModel informs (a View) about a value that is set once and never changed in the future.

🛠 **How.** Use regular property with a private setter.

In the VM:
```csharp
public string Title { get; } = Resource.CreateProject;
```

On the side of the view, during initialization logic, simply get the value and use it where appropriate.

```csharp
toolbar.Title = ViewModel.Title;
```

### 📘 ViewModel-To-View binding (one way binding)
🏆 **When.** If a ViewModel exposes a value that is driven by the viewmodel itself, or some other source that is otherwise completely independent on the view.

🛠 **How.** Expose the value as an observable.

```csharp
public IObservable<bool> IsEmpty { get; private set; } 
```
Then initialize it either from a subject, or derive from some other observable source. Note that in almost all cases, a driver should be exposed through the observable, so please use `AsDriver` and not merely `AsObservable`. Please read more about the drivers in the `Things to consider` chapter at the end of this docs.

```csharp
BehaviorSubject<bool> isEmptySubject { get; } 
    = new BehaviorSubject<bool>(false);

//...	

{
    IsEmpty = isEmptySubject
        .AsDriver(true, this.schedulerProvider);
}
```
or
```csharp
private IObservable<IEnumerable<string>> collection;

//...	

{
    IsEmpty = collection.Select(c => c.None());
}
```

### 📘 View-To-Viewmodel binding (one way to source)
🏆 **When.** If a ViewModel should update its property as a result of a change in the View (such as a value of a text field).

🛠 **How.** Expose a relay to the view.

On the ViewModel side:
```csharp
public BehaviorRelay<string> Name { get; private set; }

// ...

{
    // initialize from some source or provide an initial constant
    Name = new BehaviorRelay<string>(entity.Name);
}

```
The View can initialize it's value by using `relay.Value`, and then when changed, update it using the relay's `Accept` method.

```csharp
NameTextView.Rx().Text()
    .Subscribe(ViewModel.Name.Accept)
    .DisposedBy(DisposeBag);
```

💡 The reason why should use `Relay` here instead of `RxAction` is as follows. 
* Relays are very lightweight objects without all the bells and whistles of an RxAction. Many of the RxAction features are not needed for many tasks (such as simple propagation of data changes in widgets). Having a separate error observable is very often not needed, or the feature to prevent parallel execution. 
* RxAction prevents multiple parallel executions by utilizing it's `Executing` property which can cause glitches in combination with some widgets. Basically, as long as a widget can freely emit events, without being able to be disabled by action's `Enabled` property, it will create a glitch hazard. For some widgets, introducing the `Enabled` property could change the UX significantly so it should be avoided (i.e. Text field being disabled/enabled after each letter typed in would mess up with the focus/caret position).
* Bear in mind that the philosophy behind RxActions applies mostly to the buttons (buttons, switches, etc.) That is how they originated and while they _can_ be used for other widgets as well, think about whether it can have described side effects and avoid if possible

💡 The reason to expose Relay instead of the Subject itself is that in (almost) all cases, the View side should not be able to complete or error out the sequence. With public subject, it would be able do that. With relay, it can only use the value (or subscribe to it) and provide changes.

### 📘 two-way binding
🏆 **When.** If a value in the ViewModel can change over time by the ViewModel itself, or some other resource (independent of the view), but the View is also allowed to update that value. 

🛠 **How.** Expose a relay to the View and subscribe to it in the view.

The example is very similar to the previous one. On the ViewModel side:
```csharp
public BehaviorRelay<string> Name { get; private set; }

// ...

{
    // initialize from some source or provide an initial constant
    Name = new BehaviorRelay<string>(entity.Name);
}

```
The View can initialize it's value by using `relay.Value`, and then when changed, update it using the relay's `Accept` method.

```csharp
NameTextView.Rx().Text()
    .Subscribe(ViewModel.Name.Accept)
    .DisposedBy(DisposeBag);

ViewModel.Name
    .Subscribe(NameTextView.Rx().TextObserver())
    .DisposedBy(DisposeBag);
```

💡 This type of binding should be (and is) very rare and should be used with care. There are often ways when it can be deconstructed into any of the simpler forms as described before. Before solving the problem with this kind of binding, make sure that there are no simpler ways to do it.

⚠ The reason the previous suggestion is made is that two way binding can easily turn into an infinite loop on 🤖 (maybe also on iOS, not sure). Look at the following scenario:
> * In the above code example, `NameTextView` textbox will emit events when text changes (user typing in)
> * That will cause the `ViewModel.Name` relay to accept a new value. 
> * Since, the relay is also an observable, it will in turn emit that value through the `ViewModel.Name` itself
> * `NameTextView` textbox itself observes the relay, so it will receive the new value and update it and then immediately inform the relay about it, effectively creating an infinite loop.

This does not happen on iOS because the `TextChanged` event is only emitted when the `.Text` property of the textbox is modified by user-initiated action, such as typing in, not by setting the value programatically. On Android, both user-initiated and programatical changes will cause the event to be raised. In this scenario it is important to set the optional parameter `ignoreUnchanged` of the `TextObserver` extension method to `true`. This way, the event will be ignored if the value has not changed, effectively closing this loop:
```csharp
ViewModel.Name
    .Subscribe(NameTextView.Rx().TextObserver(true))
    .DisposedBy(DisposeBag);
```
⚠ The problem mentioned above can happen even with other widgets as well. If you are using them, make sure that the widget is updated (in the binding extension, such as `.TextObserver`) only if the value has changed so that there are no infinite loops. Alternatively, a `DistinctUntilChanged` could be used, but it had proven to be more difficult to implement properly because there are multiple places that it can be attached to. In any case, use your best judgement.


### 📘 Binding to action controls
🏆 **When.** When the ViewModel should be notified that user has initiated an action, such as tapping the button, flipping a switch, etc.

🛠 **How.** Expose an RX action.

We are using two flavors of Rx actions. 
* `UIAction` that simple does some work as a result of an action
* `InputAction<T>` that does some work as a result of an action with some data of type `T` attached.

Typical example for `UIAction` is a button tap, while the switch button should be used with `InputAction<bool>`.

⚠ **IRxActionFactory**. Make sure to inject `IRxActionFactory` into your ViewModel and initialize all actions by using methods from that interface. There are three methods to be used from that interface, depending on what kind of handler is covering the action.
* If your handler is a simple `void handlerFunction()`, use `.FromAction`.
* If your handler is a method that takes a parameter, use `.FromAction<T>` where `T` is a parameter type. Of course, the action has to be `InputAction<T>` with the same type `T`.
* If your handler is an async method, use `.FromAsync`.

**BindAction.** The most typical way to bind action to a button is to use the `BindAction` extension method. This will cause the button to be disabled during the execution of the action thus preventing multiple parallel executions.

Do it like this, in the ViewModel (the example shows an async handler for a simple `InputAction`):
```csharp
public UIAction Save { get; private set; }

// ...

{
    Save = rxActionFactory.FromAsync(save);
}

private async void save()
{
    // ...
}
```

And the corresponding View part:

```csharp
SaveButton.Rx()
    .BindAction(ViewModel.Save)
    .DisposedBy(disposeBag);
```

**Manual subscription**. If your widget *must not* get enabled/disabled during the action execution, then you have to subscribe to the source observable with the action's `.Inputs` manually. In that case, do not use the `.BindAction` method. This way the widget's enabled state will not be changed during execution (as is the case with `BindAction`). However, bear in mind that all the observables of the action will continue to work normally (`.Errors`, `Executing`, `.Enabled`, ...) if you need them for other purposes.

The ViewModel part is the same as with `BindAction`, but the subscription is manual:

```csharp
SaveButton.Rx().Tap()
    .Subscribe(ViewModel.Save.Inputs)
    .DisposedBy(disposeBag);
```
⚠ In this scenario, always have in mind, that attempting to execute the action while it is already executing will emit the error through the `Errors` observable and that the call to that `Execute` will be ignored. Possibly that can be a desired behavior but in most cases it is not. Be careful! This issue goes to show that `RxAction` should not be used everywhere without good consideration of whether it's regular behavior is actually the expected behavior.

## 📚 Things to consider
Regardless of what kind of binding you are using, always consider the following very important things:

1. If the exposed value is possibly changed from threads other than the UI thread, expose it as a driver. In most cases that will be true, so make sure to remember to expose it as a driver.
```csharp
IsEmpty = isEmptySubject.AsDriver(this.schedulerProvider);
```
`AsDriver` prevents execution on inappropriate thread and also handles the errors gracefully by providing the fallback value to the View in case of an error. Additionally, drivers share the side effects so that with `AsDriver` there's only one subscription to the source (along with all the possible side effects), regardless of how many observers observe the driver. This will prevent unwanted multiple executions of the same side effect (preventing data errors such as multiple reports of the single instance of an analytics event, and also preventing performance problems). 

For example:
```csharp
public IObservable<string> Text { get; private set; }

{
    Text = textSubject
        .Do(text => Analytics.Track(eventName, text))
        .AsDriver(string.Empty, schedulerProvider);
}
```
If the `AsDriver` was omitted here, tracking would happen for every subscription to the `Text` observable. With the driver, regardless of the number of observers, there will be only one single tracking per observable event. This is helping with both the data correctness and also performance (would the side effects include any expensive operation, such as networking, IO, etc).

2. Always consider whether adding `DistinctUntilChanged` makes sense in a given context to reduce the number of needless events emitted.

📃 More information about [RxAction can be found here](https://github.com/toggl/mobileapp/blob/develop/docs/RxAction.md)
