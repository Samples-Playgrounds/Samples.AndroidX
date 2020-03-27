# App lifecycle overview

## Introduction

From the inception of our apps until circa june 2019 our apps relied on using the [MvvmCross](https://github.com/MvvmCross/MvvmCross) framework. Due to performance and code reliability reasons, we decided that we'd rather write our own backbone for our apps.

## App startup

The startup is a very performance critical and platform specific moment of the app. We want our cold start to be as fast as possible (and we ensure that by making our app footpring as tiny as we can) and once we are on hot start, we wanna bootstrap as little as possible so the user gets the least amount of time in a hanging state.

Given how different each platform is when it comes to booting the application, the startup process is documented in two separate docs, one for [Android](startup-android.md) and one for [iOS](startup-ios.md)

## The ViewModel lifecycle

Since navigation and the general ViewModel lifecycle was hands down the best part of using MvvmCross, we decided to base our system heavily on what was provided that, with a few differences:

- It should be more statically typed (MvvmCross' is too permissive)
- It shouldn't keep track of what Activity/ViewController is at the top, as this is not the system way of doing things
- It must have a more unified way of handling initialization of the ViewModels (MvvmCross has a lot of legacy burden in this area)

Our final decisions on this regard are documented [here](viewmodel-lifecycle.md)

## Bindings

MvvmCross bindings had many problems, like lots of indirection, lack of resilience to changes and verbosity. We completely replaced the old binding mechanism with Observables, and you can check the best practices [here](../rx-binding-guidelines.md)
