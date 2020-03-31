//
// ShowReportIntent.m
//
// This file was automatically generated and should not be edited.
//

#import "ShowReportIntent.h"

@implementation ShowReportIntent



@end

@interface ShowReportIntentResponse ()

@property (readwrite, NS_NONATOMIC_IOSONLY) ShowReportIntentResponseCode code;

@end

@implementation ShowReportIntentResponse

@synthesize code = _code;

- (instancetype)initWithCode:(ShowReportIntentResponseCode)code userActivity:(nullable NSUserActivity *)userActivity {
    self = [super init];
    if (self) {
        _code = code;
        self.userActivity = userActivity;
    }
    return self;
}

@end
