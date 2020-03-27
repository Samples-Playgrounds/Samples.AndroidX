using System;
using ObjCRuntime;

namespace Toggl.iOS.Intents
{
    [Native]
    public enum ContinueTimerIntentResponseCode : long
    {
        Unspecified = 0,
        Ready,
        ContinueInApp,
        InProgress,
        Success,
        Failure,
        FailureRequiringAppLaunch,
        FailureNoApiToken = 100,
        SuccessWithEntryDescription
    }

    [Native]
    public enum ShowReportIntentResponseCode : long
    {
        Unspecified = 0,
        Ready,
        ContinueInApp,
        InProgress,
        Success,
        Failure,
        FailureRequiringAppLaunch
    }

    [Native]
    public enum ShowReportPeriodReportPeriod : long
    {
        Unknown = 0,
        Today = 1,
        Yesterday = 2,
        ThisWeek = 3,
        LastWeek = 4,
        ThisMonth = 5,
        LastMonth = 6,
        ThisYear = 7,
        LastYear = 8
    }

    [Native]
    public enum ShowReportPeriodIntentResponseCode : long
    {
        Unspecified = 0,
        Ready,
        ContinueInApp,
        InProgress,
        Success,
        Failure,
        FailureRequiringAppLaunch
    }

    [Native]
    public enum StartTimerFromClipboardIntentResponseCode : long
    {
        Unspecified = 0,
        Ready,
        ContinueInApp,
        InProgress,
        Success,
        Failure,
        FailureRequiringAppLaunch,
        FailureNoApiToken = 100,
        FailureSyncConflict
    }

    [Native]
    public enum StartTimerIntentResponseCode : long
    {
        Unspecified = 0,
        Ready,
        ContinueInApp,
        InProgress,
        Success,
        Failure,
        FailureRequiringAppLaunch,
        FailureNoApiToken = 100,
        FailureSyncConflict
    }

    [Native]
    public enum StopTimerIntentResponseCode : long
    {
        Unspecified = 0,
        Ready,
        ContinueInApp,
        InProgress,
        Success,
        Failure,
        FailureRequiringAppLaunch,
        FailureNoTimerRunning = 100,
        FailureNoApiToken,
        SuccessWithEmptyDescription,
        FailureSyncConflict
    }
}