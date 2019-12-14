The `SyncStateQueue`
====================

We use a "priority-queue-like" data structure to hold the information about which operation (`pull`, `push` or nothing - `sleep`) should be performed next.

This queue does not have anything to do with transitions operated by the `StateMachine`.

Our "queue" does not store duplicities (e.g. if the next operation should be `push` and the client enqueues another `push`, then only a single push operation will be considered).

Pull-then-push
--------------

There is only one non-trivial feature of this queue - after every `pull` we automatically perform a `push`. We also call this "full sync" in the `SyncManager`.

---

Previous topic: [State machine](state-machine.md)
Next topic: [Sync manager](sync-manager.md)
Go to the [Index](index.md)
