State machine
=============

The syncing algorithms consists of individual loosely coupled _states_ which have a set of dynamically configured transitions based on the different results of the states.

The mechanism of evaluating the states and transitioning between them is implemented with two classes: `StateMachine` and `StateMachineOrchestrator`. These two classes are completely agnostic of the Toggl business logic.

The `StateMachine`
-----------------

The state machine has only a single purpose: it starts in some entry point (_to be precise: a transition from an entry point_) and performs transitions from one state to another until a dead end is reached or an error occurs.

The state machine can only process one state at a time.

The `StateMachineOrchestrator`
-----------------------------

The orchestrator provides a _simple_ interface for the client to start one of the two supported operations: `Pull` or `Push`. The orchestrator holds the entry points and invokes the `StateMachine` with the correct entry point for either of the operations.


Freezing the state machine
--------------------------

When the user logs out of the app we need to stop syncing as fast as possible and block any further syncing. This operation is called _freezing_ and it waits until the app finishes the current state that is being evaluated and then completes and locks itself so it is not possible to run syncing after this point.

To continue with syncing, new `StateMachine` and `StateMachineOrchestrator` instances must be created - the previous state machine with its state (e.g., the state in which it was frozen) cannot be resumed and the frozen instance should be destroyed.

---

Previous topic: [Pull Time Entries](pull-time-entries.md)
Next topic: [State queue](state-queue.md)
Go to the [Index](index.md)
