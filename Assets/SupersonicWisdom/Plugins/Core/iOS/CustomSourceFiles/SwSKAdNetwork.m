#import "SwSKAdNetwork.h"
#import "SwTypeUtil.h"

void _swUpdatePostbackConversionValue(int cv) {
    if (@available(iOS 15.4, *)) {
        NSInteger cvInteger = cv;
        [SKAdNetwork updatePostbackConversionValue:cvInteger completionHandler:^(NSError * _Nullable error) {
            NSString *errorCode = [NSString stringWithFormat: @"%ld", (long)[error code]];
            UnitySendMessage("SupersonicWisdom","UpdatePostbackUpdateCompletedMessage", [errorCode UTF8String]);
        }];
    } else {
        if (cv == 0){ // In new API : (CV = 0) -> Initial call
            if (@available(iOS 11.3, *)) {
               [SKAdNetwork registerAppForAdNetworkAttribution];
               UnitySendMessage("SupersonicWisdom","UpdatePostbackUpdateCompletedMessage", "");
            } else {
                 // Fallback on earlier versions
          }
       } else {
          if (@available(iOS 14.0, *)) {
                NSInteger cvInteger = cv;
                [SKAdNetwork updateConversionValue:cvInteger];
                UnitySendMessage("SupersonicWisdom","UpdatePostbackUpdateCompletedMessage", "");
            } else {
                // Fallback on earlier versions
            }
        }
    }
}

void _swUpdatePostbackConversionAndCoarseValue(int cv ,int completeCoarseValue, BOOL selectedLockWindow) {
    NSLog(@"_swUpdatePostbackConversionAndCoarseValue called with, cv: %d, completeCoarseValue: %d, selectedLockWindow: %d", cv, completeCoarseValue, selectedLockWindow);
    if (@available(iOS 16.1, *)) {
        NSInteger cvInteger = cv;
        SKAdNetworkCoarseConversionValue skAdNetworkCoarseConversionValue;
        
        switch (completeCoarseValue) {
            case 0:
                skAdNetworkCoarseConversionValue = SKAdNetworkCoarseConversionValueLow;
                break;
            case 1:
                skAdNetworkCoarseConversionValue = SKAdNetworkCoarseConversionValueMedium;
                break;
            case 2:
                skAdNetworkCoarseConversionValue = SKAdNetworkCoarseConversionValueHigh;
                break;
        }
        
        [SKAdNetwork
            updatePostbackConversionValue:cv
            coarseValue:skAdNetworkCoarseConversionValue
            lockWindow:selectedLockWindow
            completionHandler:^(NSError * _Nullable error) {
                NSString *errorCode = [NSString stringWithFormat: @"%ld", (long)[error code]];
                UnitySendMessage("SupersonicWisdom", "UpdatePostbackUpdateCompletedMessage", [errorCode UTF8String]);
        }];
    } else {
        _swUpdatePostbackConversionValue(cv);
    }
}

char* _swGetSkAdNetworks() {
    NSMutableArray *skAdNetworks = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"SKAdNetworkItems"];

    if (!skAdNetworks) {
        return swStringCopy([@"" UTF8String]);
    }
    NSMutableArray *networksArray = [[NSMutableArray alloc] init];

    for (NSDictionary *networkDict in skAdNetworks){
        NSString *network = [networkDict objectForKey:@"SKAdNetworkIdentifier"];
        if (network) {
            [networksArray addObject:network];
        }
    }
    
    NSString *joined = [networksArray componentsJoinedByString:@","];
    return swStringCopy([joined UTF8String]);
}

char* _swGetAdvertisingAttributionReportEndpoint() {
    NSString *endpoint = [[NSBundle mainBundle] objectForInfoDictionaryKey:@"NSAdvertisingAttributionReportEndpoint"];
    if (!endpoint) {
        swStringCopy([@"" UTF8String]);
    }

    return swStringCopy([endpoint UTF8String]);
}

int _swGetSkanVersion() {
    if (@available(iOS 16.1, *)){
        return 4;
    }
    else if (@available(iOS 14.5, *)) {
        return 3;
    }
    else if (@available(iOS 14.0, *)){
        return 2;
    }
    else if (@available(iOS 11.3, *)) {
        return 1;
    }
    
    return 0;
}
