//
// ShowReportPeriodIntent.m
//
// This file was automatically generated and should not be edited.
//

#import "ShowReportPeriodIntent.h"

@implementation ShowReportPeriodIntent

@dynamic period, workspace;

@end

@interface ShowReportPeriodIntentResponse ()

@property (readwrite, NS_NONATOMIC_IOSONLY) ShowReportPeriodIntentResponseCode code;

@end

@implementation ShowReportPeriodIntentResponse

@synthesize code = _code;

@dynamic period;

- (instancetype)initWithCode:(ShowReportPeriodIntentResponseCode)code userActivity:(nullable NSUserActivity *)userActivity {
    self = [super init];
    if (self) {
        _code = code;
        self.userActivity = userActivity;
    }
    return self;
}

+ (instancetype)successIntentResponseWithPeriod:(ShowReportPeriodReportPeriod)period {
    ShowReportPeriodIntentResponse *intentResponse = [[ShowReportPeriodIntentResponse alloc] initWithCode:ShowReportPeriodIntentResponseCodeSuccess userActivity:nil];
    intentResponse.period = period;
    return intentResponse;
}

@end
