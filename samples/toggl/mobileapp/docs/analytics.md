## Adding events to our analytics system

Toggl uses the Firebase/AppCenter analytics for events.

### ⚠ Precautionary notes
Numbers are hard. So we need to be careful with analytics. We are **not able** to change the events or their parameters, so it's important that they be named correctly, unambiguously and wisely so that no future corrections are needed. Adding new parameters is fine, but changing existing ones (by deprecating one and using another) will introduce errors in stats. We should strive to avoid those situations as much as possible. 

If, however, a situation arises and it becomes absolutely necessary to change the event or their parameters, this should be discussed properly and 👮‍♂️ **approved by the Team lead**. This scenario is rather easily prevented by investing more time into planning and research so that we know exactly we want to track and the possibility of expansions/variations in the future.

### 📃 The procedure
When you want to track something new, this is the procedure:

**Preparation.** First consider how to properly structure the event. It is better to have one parameterized event than two events that do the same thing. Bear in mind that (for now) we are unable to correlate multiple parameters from one event, so do proper care to understand what information you want saved and design your event accordingly.

📚 Example: it's better to have one event `PageOpened` with parameter `page` than multiple events such as `MainLogOpened`, `EditViewOpened`, `SettingsViewOpened`, etc. However, if you need some additional data tracked related to one of those operation, for example, source from which the Edit View got opened, use an additional event such as `EditViewOpened(source)`.

**Steps**.

1. Check whether the same data is already tracked in any shape or form.
    1. If yes, consider using the existing event or find a good way to merge the two events
    1. If not, continue
1. Consider how many event parameters you need.
    1. If there are 4 or less primitive parameters, follow `IAnalyticsEvent` chapter
    1. If there are 5 or more parameters or the event requires any additional logic, follow `ITrackableEvent` chapter. This is also the way to do it if you need a platform specific event.

⚠ The examples in the following chapters show how to do the same thing different ways. Make sure you use the above decision tree to decide which one you need, so that consistency is maintained.

## `IAnalyticsEvent`

✔ 1. Add your event as a **getter-only** property to `Toggl.Core.Analytics.IAnalyticsService` interface. Make sure your generic type has proper number of arguments and use only primitive types for parameters.

> 📚 Example: `IAnalyticsEvent<string, int> MyEvent { get; }`

✔ 2. Add your event as a ``{ get; protected set; }`` property to `Toggl.Core.Analytics.BaseAnalyticsService` class. Make sure that you have both accessors because otherwise the application will fail to initialize the analytics service on startup.

> 📚 Example: `public IAnalyticsEvent<string, int> MyEvent { get; protected set; }`

✔ 3. Mark the property you've just created in `BaseAnalyticsService` with an `AnalyticsEvent` attribute. Add one string argument for each of the event parameters. These strings are the names for the event parameters as seen from the perspective of Firebase/AppCenter. 

⚠ Name your parameters wisely!

⚠ Note that the number of arguments given to the `AnalyticsEvent` attribute has to match the number of generic parameters of the event property. In the case of the mismatch, a test will fail with a message which will give you a hint about which event is an offender and what counts are actual. eg.:

```
Expected collection to be empty, but found 
{{ Name = EditEntrySelectTag, AttributeArgumentCount = 1, GenericTypeCount = 0 }, 
{ Name = StartEntrySelectProject, AttributeArgumentCount = 0, GenericTypeCount = 1 }}.
```

> 📚 Example: `[AnalyticsEvent("Source", "Value")] `

You are now able to track the event by using your property through `IAnalyticsService`:
```cs
analyticsService.MyEvent.Track("SourceExample", 14);
```

## `ITrackableEvent`
For more complex events, or events with additional logic/behavior, these are the steps:

✔ 1. Create a class and implement the `ITrackableEvent` interface.
* If the event is the same for both platforms, put it into the `Toggl.Core.Analytics` folder, otherwise find an appropriate platform-specific location for the class.

✔ 2. Implement the `EventName` property by providing the event name, as seen from Firebase/AppCenter perspective.

⚠ To keep things consistent with `IAnalyticsEvent` events, use `PascalCase` when creating names.

> 📚 Example: `public string EventName => "MyComplexEvent";`

✔ 3. Implement the `ToDictionary` property so that it returns a dictionary of arguments for the event.

In the following example, `source` and `value` variables are local variables of the event class.

```cs
public Dictionary<string, string> ToDictionary()
{
    return new Dictionary<string, string>
    {
        ["Source"] = source,
        ["Value"] = value
    };
}
```

ℹ You are able to use objects of this class as the regular .NET objects. Run their methods, perform calculations on them, do whatever you want. At the moment you actually want to track it (the current state of it, that is), you can do it like this:

```cs
var myEvent = new MyEvent("SourceExample", 14);
analyticsService.Track(myEvent);
```

Make sure your `ToDictionary` always returns a correct set of arguments/values.
