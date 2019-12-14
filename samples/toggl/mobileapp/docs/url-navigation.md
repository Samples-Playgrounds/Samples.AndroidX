# url-based navigation

This documents describe how deeplinks and url based navigation works and what are the urls we support.


## Architecture

All deeplinks and url navigation are handled by the `UrlHandler` instance. This service is created by the dependency containers on the platform level.

Depending on the url, the `UrlHandler` can:
1. Do some action that manipulates data (e.g: create a time entry)
1. Navigate to a particular view mdoel (e.g: open the EditTimeEntryViewModel)
1. Change the presentation in some way (e.g: change the visible tab)


## Internal navigation

In iOS, we use deeplinks internally for:
1. 3D Touch shortcuts
1. Siri intents
1. Calendar notifications

In Android:
// TODO: Finish this


## Adding new urls

When you need to add new urls that will be handled by our apps, take into account the following:

1. Put the path, parameter names and every other string value in `ApplicationUrls`
1. Create static classes grouping the values (e.g: `ApplicationUrls.TimeEntry`)
1. Use static methods in those classes to build the actual url


## Supported urls

### Time entries

*Start time entry*

Description: starts a new time entry
Effects: a new time entry is created, no navigation or presentation changes
Url: toggl://tracker/timeEntry/start

Parameters:

1. workspaceId: `long`, optional, defaults to the user's default workspace id
1. startTime: `datetime`, optional, defaults to `DateTimeOffset.Now`
1. description: `string`, optional, defaults to `string.Empty`
1. projectId: `long`, optional
1. taskId: `long`, optional
1. tags: `long[]`, optional
1. billable: `bool`, optional, defaults to `false`
1. source: `TimeEntryStartOrigin`, optional, defaults to `TimeEntryStartOrigin.Deeplink`

Examples:
1. toggl://tracker/timeEntry/start
1. toggl://tracker/timeEntry/start?workspaceId=1&startTime="2019-04-18T09:35:47Z"&description=Toggl&projectId=1&taskId=1&tags=[1,2,3]&billable=true&source=Siri


*Continue last time entry*

Description: continues the last time entry
Effects: a new time entry is created, no navigation or presentation changes
Url: toggl://tracker/timeEntry/continue

Parameters: none

Examples:
1. toggl://tracker/timeEntry/continue


*Stop time entry*

Description: stops the running time entry, if any
Effects: the running time entry is stopped, no navigation or presentation changes
Url: toggl://tracker/timeEntry/stop

Parameters:

1. stopTime: `datetime`, optional, defaults to `DateTimeOffset.Now`
1. source: `TimeEntryStopOrigin`, optional, defaults to `TimeEntryStopOrigin.Deeplink`

Examples:
1. toggl://tracker/timeEntry/stop
1. toggl://tracker/timeEntry/stop?stopTime="2019-04-18T09:45:47Z"&source=Siri


*Create time entry*

Description: creates a time entry with the given parameters
Effects: a new time entry is created, no navigation or presentation changes
Url: toggl://tracker/timeEntry/create?workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&duration=600&description="Toggl"&projectId=1&taskId=1&tags=[]&billable=true&source=Siri

Parameters:

1. workspaceId: `long`, optional, defaults to the user's default workspace id
1. startTime: `datetime`, required
1. stopTime: `datetime`, required if duration is not present
1. duration: `timespan`, required if stopTime is not present
1. description: `string`, optional, defaults to `string.Empty`
1. projectId: `long`, optional
1. taskId: `long`, optional
1. tags: `long[]`, optional
1. billable: `bool`, optional, defaults to `false`
1. source: `TimeEntryStartOrigin`, optional, defaults to `TimeEntryStartOrigin.Deeplink`

Note:
If both stopTime and duration are provided, duration will take precedence over stopTime.

Examples:
1. toggl://tracker/timeEntry/create?workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&duration=600&description="Toggl"&projectId=1&taskId=1&tags=[]&billable=true&source=Siri


*Update time entry*

Description: updates a time entry with the given parameters
Effects: a time entry is updated, no navigation or presentation changes
Url: toggl://tracker/timeEntry/update?timeEntryId=1workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&description="Toggl"&projectId=1&taskId=1&tags=[]&billable=true

Parameters:

1. timeEntryId: `long`, required
1. workspaceId: `long`, optional, defaults to the user's default workspace id
1. startTime: `datetime`, optional
1. stopTime: `datetime`, optional
1. duration: `timespan`, optional
1. description: `string`, optional
1. projectId: `long`, optional
1. taskId: `long`, optional
1. tags: `long[]`, optional
1. billable: `bool`, optional

Examples:
1. toggl://tracker/timeEntry/update?timeEntryId=1workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&description="Toggl"&projectId=1&taskId=1&tags=[]&billable=true


*New time entry*

Description: navigates to `StartTimeEntryViewModel` passing the given parameters
Effects: no changes in data, navigates to the start time entry view
Url: toggl://tracker/timeEntry/new?workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&duration=600&description="Toggl"&projectId=1&tags=[]

Parameters:

1. workspaceId: `long`, optional
1. startTime: `datetime`, optional
1. stopTime: `datetime`, optional
1. duration: `timespan`, optional
1. description: `string`, optional
1. projectId: `long`, optional
1. tags: `long[]`, optional

Examples:
1. toggl://tracker/timeEntry/new
1. toggl://tracker/timeEntry/new?workspaceId=1&startTime="2019-04-18T09:35:47Z"&stopTime="2019-04-18T09:45:47Z"&duration=600&description="Toggl"&projectId=1&tags=[]


*Edit time entry*

Description: navigates to `EditTimeEntryViewModel` passing the time entry id
Effects: no changes in data, navigates to the edit time entry view
Url: toggl://tracker/timeEntry/edit?timeEntryId=1

Parameters:

1. timeEntryId: `long`, required

Examples:
1. toggl://tracker/timeEntry/edit?timeEntryId=1


*Reports*

Description: changes presentation to load and show reports
Effects: no changes in data, the reports view is visible and loaded
Url: toggl://tracker/reports

Parameters:

1. workspaceId: `long`, optional
1. startDate: `datetime`, optional
1. endDate: `datetime`, optional

Examples:
1. toggl://tracker/reports
1. toggl://tracker/reports?workspaceId=1&startDate="2019-04-18T09:35:47Z"&endDate="2019-04-18T09:45:47Z"


*Calendar*

Description: changes presentation to show the calendar and creates a time entry from the given event
Effects: the calendar view is visible, a time entry is created (optional)
Url: toggl://tracker/calendar

Parameters:

1. eventId: `long`, optional

Examples:
1. toggl://tracker/calendar
1. toggl://tracker/calendar?eventId=1
