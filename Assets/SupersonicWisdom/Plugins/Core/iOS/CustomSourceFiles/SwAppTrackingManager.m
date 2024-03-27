#import "SwAppTrackingManager.h"
#import "dlfcn.h"

extern void UnitySendMessage(const char*, const char*, const char*);

@implementation SwAppTrackingManager


+(void) _sendUnityResponse:(NSUInteger)num {
    NSString *sMsg = [NSString stringWithFormat:@"%ld",(long)num ];
    UnitySendMessage("SupersonicWisdom","IOSTrackingAuthorizationChangeMessage",[sMsg UTF8String]);
    
}

+(void) _showTrackingConsentDialog {
    dlopen("/Library/Frameworks/AppTrackingTransparency.framework/AppTrackingTransparency", RTLD_LAZY);
    Class tm = NSClassFromString(@"ATTrackingManager");

    if (tm) {
        [tm requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
            [self _sendUnityResponse:status];
        }];
    } else {
            [self _sendUnityResponse:-1];
    }

}

+(int) _getTrackingConsentValue {
    dlopen("/Library/Frameworks/AppTrackingTransparency.framework/AppTrackingTransparency", RTLD_LAZY);
    Class tm = NSClassFromString(@"ATTrackingManager");

    if (tm) {
        return [tm trackingAuthorizationStatus];
    }
    
    return -1;
}

+(void) _openSettingsPage {
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
}


@end

void _swShowTrackingConsentDialog() {
    [SwAppTrackingManager _showTrackingConsentDialog];
}

int _swGetTrackingConsentValue() {
    return [SwAppTrackingManager _getTrackingConsentValue];
}
void _swOpenSettingsPage() {
    return [SwAppTrackingManager _openSettingsPage];
}

BOOL _swIsTrackingConsentMandatory() {
    return @available(iOS 14.5, *);
}
