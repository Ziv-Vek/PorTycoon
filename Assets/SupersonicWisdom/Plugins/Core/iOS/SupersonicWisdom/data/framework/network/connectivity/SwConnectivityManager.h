//
//  SwConnectivityManager.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/02/2021.
//

#import <Foundation/Foundation.h>
#import "Reachability.h"
#import "SwNetworkCallbacks.h"
#import "SWSDKLogger.h"
#import "SwWisdomSessionCallback.h"

#define RESPONSE_CODE_BAD_REQUEST 400
#define RESPONSE_CODE_OK 200
#define RESPONSE_CODE_NO_INTERNET -6
#define REQUEST_TIMEOUT 3.0

@interface SwConnectivityManager : NSObject

@property (nonatomic, strong, readwrite) NSPointerArray* delegates;
+ (instancetype)internetConnectivity;
+ (instancetype)connectivityWithAddress:(const struct sockaddr *)hostAddress;
- (NetworkStatus)currentConnectivityStatus;
- (BOOL)isNetworkAvailable;
- (void)registerConnectivityDelegate:(id<SwConnectivityStatusCallback>)delegate;
- (void)unregisterConnectivityDelegate:(id<SwConnectivityStatusCallback>)delegate;

@end
