//
//  SwEventsRemoteStorageDelegate.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//

#import "Reachability.h"

@interface SwNetworkCallbacks : NSObject

typedef void(^OnNetworkResponse)(NSString *key, BOOL successfully, NSInteger responseCode, NSData *responseData);
typedef void(^OnSwResponse)(NSString *response);
typedef void(^OnSwNetworkConnectionChanged)(BOOL isConnected, NetworkStatus status);
@property (nonatomic) OnNetworkResponse callback;

@end
