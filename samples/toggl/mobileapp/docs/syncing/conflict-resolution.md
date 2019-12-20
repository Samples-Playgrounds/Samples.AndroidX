Conflict resolution
===================

We need to merge our local state of an entity with the state we get from the server.

A conflict resolver is a function with the following signature:

```csharp
ConflictResolutionMode Resolve<T>(T localEntity, T serverEntity);
```

`ConflictResolutionMode` is an `enum` which has _four possible values_:

- `Create`
- `Update`
- `Delete`
- `Ignore`

We use several conflict resolvers:

| Class name | Where it is used | What does it do |
|:---------- |:---------------- |:----------------|
| `AlwaysOverwrite` | Used for pulling workspace features. | If there is already an existing local entity - update it, otherwise create a new one. |
| `OverwriteUnlessNeedsSync` | Used for pulling preferences. | If there is no local entity, create it. When there is a local entity and it needs sync, ignore the server data, otherwise update the local entity. |
| `safeAlwaysDelete` | For the `DeleteAll` method of the `IDataSource`. _This is not a separate class, just a method inside of the `DataSource` class._ | Deletes everything. |
| `ignoreIfChangedLocally` | Used during push sync for overwriting local data with the data from the response from the server.  _This is not a separate class, just a method inside of the `BaseDataSource` class._ | This conflict resolver updates the local data only if the user hasn't edited the data during the duration of the HTTP request roundtrip. |
| `PreferNewer` | Used for pulling the rest of the entities. | Checks which of the local entity and the server entity was updated most recently (using `ILastChangedDatable.At`) and uses the properties from this entity. |

### PreferNewer - further remarks

This is the most used and most complex conflict resolver and it deserves some more information about what it does.

- This resolver also checks if the entity on server was deleted or not (for deletable entities).
- Local entities which are in sync or are marked as "refetching needed" - placeholders - are always overwritten.
- An extra parameter `TimeSpan MarginOfError` can be used to acount for network delays ("5-second rule").

Rivals resolution
-----------------

There is a second type of conflict resolution which we call rivals resolution. The difference is that the previously described conflict resolution is _between the local and server version_ of the _same entity_, while rivals are _two different entities of the same type which_ are in conflict.

Currently we use this only for a single purpose (to make sure there is just one running time entry) but we could use this concept to prevent projects/tags/clients with the same name in the same workspace.

A resolver must implement the interface `IRivalsResolver<T>`.

### Time entries rivals resolution

If two time entries are rivals (both of them have the `Duration` set to `null`), one of them will be stopped.

The time entry which was updated most recently will be left running and the other one will be stopped. We use the `ILastChangedDatable.At` to check which time entry is more up-to-date.

Selecting the stop time (duration) for the stopped time entry is a complex process - from all of the time entries which start after the start of the stopped entry we select the one with the minimum value or the current time if the list was empty.

_Implementation detail: To run the Realm query efficiently we create our own expression for evaluating which time entries start after a given time entry._

---

Next topic: [Pull sync](pull-sync.md)
Go to the [Index](index.md)
