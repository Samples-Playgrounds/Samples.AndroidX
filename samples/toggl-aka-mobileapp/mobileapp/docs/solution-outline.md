
# Toggl Mobile App Solution Outline

This document outlines the projects contained in this repository, dependencies and responsibilities.

In general, our goal is to keep concerns separated as much as sensibly possible. It is important to us that any subset of the projects (and including their dependencies) can be treated as modules and reused independently for other Toggl apps.


## :computer: Toggl.Shared

Toggl.Shared is a shared helper library.

Dependencies: None

It contains several simple helper types, as well as shared model interfaces.


## :satellite: Toggl.Networking

Toggl.Networking provides a thin layer around the public Toggl Api.

Dependencies: Toggl.Shared

Its main responsibility is calling the correct Http endpoints, and (de)serializing models from/for the api.
To allow for the latter, it has limited knowledge of Toggl's business logic in the sense that it will only serialise model properties valid for a specific endpoint and given business logic specific constraints.


## :crystal_ball: Toggl.Storage

Toggl.Storage is our local storage (database) project.

Dependencies: Toggl.Shared

This project contains interfaces representing the local storage repository and database models of a Toggl app. These interfaces can be implemented in additional projects corresponding to specifics database/storage frameworks/libraries.


## :rocket: Toggl.Core

Toggl.Core contains Toggl's business logic.

Dependencies: Toggl.Shared, Toggl.Networking, Toggl.Storage

Next to containing business logic, this project also ties together the Toggl Api and local storage with a comprehensive syncing algorithm.
This is the _foundation_ for any of our Toggl mobile apps, though it contains no mobile specific code itself, and can be used to run a Toggl app on any platform.


## :twisted_rightwards_arrows: Toggl.Core.UI

Toggl.Core.UI is a shared UI view model library.

Dependencies: Toggl.Shared, Toggl.Core

This project contains shared view models for all our app. Only business logic related directly to specific UI components that cannot be handled easily in Toggl.Core is dealt with here. Otherwise the project is kept as light as possible.
This allows us to use a lot of UI behaviour between the apps, and keep the platform specific code minimal.

For an overview about how our app connects the cross platforms layer to the platform one, check [this doc](app-lifecycle/overview.md)

## :sunny: Toggl.iOS & :robot: Toggl.Droid

Dependencies: Toggl.Core.UI

These projects contain .storyboard/.xib/.axml files and minimal UI code binding to the view models of Toggl.Core.UI. The only non-UI code in this project are platform specific features that can not be handled otherwise.

## :vertical_traffic_light: Tests

### :wrench: Unit Tests

All of our projects are unit tested using xUnit, in corresponding projects with the '.Tests' suffix.

We strive for test coverage as near to 100% as possible. All new code should be submitted with appropriate tests, and can only be merged if all tests pass.

To facilitate this, unit tests are run on every commit automatically.

### :link: Integration Tests

Toggl.Networking has extensive integration tests, which tests all endpoints against Toggl's staging api.

These tests are run nightly on the `develop` branch.

### :art: UI Tests

Toggl.iOS has UI Tests, which test all UI components for correct behaviour.

These tests are run nightly on the `develop` branch.

### :calling: Test builds

To enable every member of the team, and others inside Toggl to test the current state of the app easily, we create automatic AdHoc builds every night automatically.

The builds, as well as the results of the integration and UI tests are posted to Slack for easy access.
