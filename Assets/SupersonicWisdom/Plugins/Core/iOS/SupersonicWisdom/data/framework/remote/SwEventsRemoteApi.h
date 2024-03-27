//
//  SwEventsRemoteApi.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwWisdomNetwork.h"
#import "SwNetworkCallbacks.h"
#import "SwListStoredEventJsonMapper.h"

@interface SwEventsRemoteApi : NSObject

- (id)initWithNetwork:(id<SwWisdomNetwork>)network subdomain:(NSString *)subdomain storedEventsMapper:(SwListStoredEventJsonMapper *)listMapper;
- (void)sendEventAsync:(NSDictionary *)details withResponseCallback:(OnNetworkResponse)callback;
- (void)sendEventsAsync:(NSArray *)events withResponseCallback:(OnNetworkResponse)callback;

@end
