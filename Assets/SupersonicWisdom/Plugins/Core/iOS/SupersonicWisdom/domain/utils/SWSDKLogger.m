//
//  SWSDKLogger.m
//  SupersonicWisdom
//
//  Created by Perry Sh on 05/09/2022.
//

#import "SWSDKLogger.h"
#import "SwUtils.h"

@implementation SWSDKLogger

static BOOL _isEnabled = NO;

+ (void)setIsEnabled:(BOOL)isEnabled {
    _isEnabled = isEnabled;
}

+ (void)logMessage:(NSString *)message, ... {
    if (!_isEnabled) return;

    // From: https://stackoverflow.com/a/4804807/2735029
    va_list args;
    va_start(args, message);
    NSLogv(message, args);
    va_end(args);
}

@end
