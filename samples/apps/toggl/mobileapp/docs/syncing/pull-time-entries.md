Pull Time Entries Sync Workflow
===============================

This workflow is designed to be as lightweight as possible by sending only 1 GET HTTP request to fetch the latest changes to the time entries. This is necessary because we want to be able to trigger this as often as possible and to get the results fast and reliably and at the same time to avoid overloading the server with many requests.

Fetching Just Time Entries
--------------------------

We send only 1 HTTP requests which fetches time entries the same way we do that during full sync. We respect the rate limitting constraints and this step might fail, if our leaky bucket overflows (for details see [rate limitting](rate-limitting.md)). The error handling and other aspects of handling HTTP is the same as with the [pull sync](pull-sync.md).

Placeholders
------------

A time entry can depend on having several other dependencies in the database: a workspace, a project, a task, and any number of tags. If these entities aren't present in the database, we create placeholders for these entities which will be replaced with the valid data during the next regular pull sync.

Analytics
---------

We log several events to keep track of how many times we actually have to create placeholders for unknown entities. These analytics events are:
- `WorkspacePlaceholdersCreated`
- `ProjectPlaceholdersCreated`
- `TaskPlaceholdersCreated`
- `TagPlaceholdersCreated`

Each of these events has a parameter called `NumberOfCreatedPlaceholders`. We track this event every time even when no placeholders are created. The tracked value will be 0 in that case.

Where everything is implemented in the code
-------------------------------------------

The code is located (mostly) under the namespace `Toggl.Foundation.Sync.States.PullTimeEntries`.

We initiate the HTTP request in the state class `FetchJustTimeEntriesSinceState`.

The logic of creating project placeholders is implemented in the generic `CreatePlaceholdersState` and the concrete variants for the individual types of entities can be created by the `CreatePlaceholdersStateFactory`.

The states are instantiated and connected in the `Toggl.Foundation.TogglSyncManagerFactory` class.

---

Previous topic: [Push sync](push-sync.md)
Next topic: [State machine](state-machine.md)
Go to the [Index](index.md)
