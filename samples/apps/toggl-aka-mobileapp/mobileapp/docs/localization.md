# Localization

Toggl uses the built-in mechanism for localization using `Resources.resx` file. The technique is described at [Xamarin's Localization](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/localization/text?tabs=macos)

When dealing with UI strings, we have to follow the process described below:

## ☀️ iOS

### 💻 For programatically built views

- Setup the UI's text using `System.Resources` namespace normally. 

### 📐 or Interface Builder (xib, storyboard) views 

- Create the view using IB file.
- Created outlets for the UI elements that need to be translated.

_Note: The string in the IB will be replaced, so it's better to make it something "placeholdery" obvious so we can catch the mistake early._

- Setup the text of those outlets in `ViewDidLoad` or `AwakeFromNib`

## 🤖 Droid

### 💻 For programmatically built views

- Setup the UI's text using the `System.Resources` namespace, much like iOS.

### 📐 or Xml-based views

- Setup meaningful Ids for views that can display text.
- Find the views normally on `InitializeViews`.
- It's safe the setup the text right after the `InitializeViews` call.

_Note: Leave android text attributes empty and use the `tools:text` or similar for previewing purposes._
