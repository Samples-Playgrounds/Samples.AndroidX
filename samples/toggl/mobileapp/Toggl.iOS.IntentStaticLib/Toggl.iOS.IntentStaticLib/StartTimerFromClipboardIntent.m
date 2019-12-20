//
// StartTimerFromClipboardIntent.m
//
// This file was automatically generated and should not be edited.
//

#import "StartTimerFromClipboardIntent.h"

@implementation StartTimerFromClipboardIntent

@dynamic workspace, billable, projectId, tags;

@end

@interface StartTimerFromClipboardIntentResponse ()

@property (readwrite, NS_NONATOMIC_IOSONLY) StartTimerFromClipboardIntentResponseCode code;

@end

@implementation StartTimerFromClipboardIntentResponse

@synthesize code = _code;

- (instancetype)initWithCode:(StartTimerFromClipboardIntentResponseCode)code userActivity:(nullable NSUserActivity *)userActivity {
    self = [super init];
    if (self) {
        _code = code;
        self.userActivity = userActivity;
    }
    return self;
}

@end
