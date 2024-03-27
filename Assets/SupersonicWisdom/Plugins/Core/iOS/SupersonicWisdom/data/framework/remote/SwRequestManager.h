//
//  SwRemoteServer.h
//  SupersonicWisdom
//
//  Created by Omer Bentov on 17/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwWisdomNetwork.h"
#import "SwConnectivityManager.h"
#import "SwWisdomSessionCallback.h"

#define SW_REQUEST_KEY @"key"
#define SW_REQUEST_URL @"url"
#define SW_REQUEST_BODY @"body"
#define SW_REQUEST_CONNECTION_TIMEOUT @"connectionTimeout"
#define SW_REQUEST_READ_TIMEOUT @"readTimeout"
#define SW_REQUEST_CAP @"cap"
#define SW_REQUEST_ITERATION @"iteration"
#define SW_REQUEST_IS_REACHED_CAP @"isReachedCap"
#define SW_REQUEST_ERROR @"error"
#define SW_REQUEST_DATA_STRING @"dataString"
#define SW_REQUEST_IS_DONE @"isDone"
#define SW_REQUEST_IS_PENDING @"isPending"
#define SW_REQUEST_CODE @"code"
#define SW_REQUEST_TIME @"time"
#define SW_REQUEST_WAITING_MULTIPLIER 2.0

@interface SwRequestManager : NSObject <SwConnectivityStatusCallback>

- (id)initWithNetwork:(id<SwWisdomNetwork>)network connectivityManager:(SwConnectivityManager *)connectivityManager;
- (void)sendRequest:(NSString *)request withResponseCallback:(OnSwResponse)callback;

@end
