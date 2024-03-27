//
//  SwEventsRemoteDataSource.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "SwEventsRemoteDataSource.h"

@implementation SwEventsRemoteDataSource {
    SwEventsRemoteApi *remoteApi;
}

- (id)initWithApi:(SwEventsRemoteApi *)api {
    if (!(self = [super init])) return nil;
    remoteApi = api;
    return self;
}

- (void)sendEventAsync:(NSDictionary *)details withResponseCallback:(OnNetworkResponse)callback {
    [remoteApi sendEventAsync:details withResponseCallback:callback];
}

- (void)sendEventsAsync:(NSArray *)events withResponseCallback:(OnNetworkResponse)callback {
    [remoteApi sendEventsAsync:events withResponseCallback:callback];
}

@end
