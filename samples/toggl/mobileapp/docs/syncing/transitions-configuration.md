Transitions configuration
=========================

Every sync state object has several _state results_ - the possible outcomes of the operation. It is important that these **states are instantiated just once** and are never changed - we use reference equality to match state results.

Every syncing state is a class which implements the `ISyncState` or `ISyncState<T>` interface, which defines a simple function called `Start` which may take some parameter (result of the previous state) and which returns an _observable of a transition_.

A transition is _just a container_ which holds reference to the state result and it might contain also a parameter for the state result.

We use the _state result_ from the transition to look for the next state in a dictionary-wrapper called `TransitionHandlerProvider`. We pick a transition handler (the `Start` method of an `ISyncState` or `ISyncState<T>`) and call it with the parameter specified in the transition (if any).

We configure the coupling between state results with their handlers in `TogglSyncManagerFactory`.

---

Previous topic: [Sync manager](sync-manager.md)
Next topic: [Sample sync state](example.md)
Go to the [Index](index.md)
