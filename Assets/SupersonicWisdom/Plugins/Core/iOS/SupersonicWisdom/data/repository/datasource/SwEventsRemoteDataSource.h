//
//  SwEventsRemoteDataSource.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventsRemoteApi.h"
#import "SwNetworkCallbacks.h"

@interface SwEventsRemoteDataSource : NSObject

- (id)initWithApi:(SwEventsRemoteApi *)api;
- (void)sendEventAsync:(NSDictionary *)details withResponseCallback:(OnNetworkResponse)callback;
- (void)sendEventsAsync:(NSArray *)events withResponseCallback:(OnNetworkResponse)callback;

@end
