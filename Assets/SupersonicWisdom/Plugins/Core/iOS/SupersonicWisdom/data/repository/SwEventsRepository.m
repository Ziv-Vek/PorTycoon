//
//  SwEventsRepository.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "SwEventsRepository.h"

@implementation SwEventsRepository {
    SwEventsRemoteDataSource *remoteDataSource;
    SwEventsLocalDataSource *localDataSource;
    NSObject *lockObj;
}

- (id)initWithRemoteDataSource:(SwEventsRemoteDataSource *)remote
               localDataSource:(SwEventsLocalDataSource *)local {
    if (!(self = [super init])) return nil;
    remoteDataSource = remote;
    localDataSource = local;
    lockObj = [NSObject new];
    
    return self;
}

- (void)sendEventAsync:(NSDictionary *)event withResponseCallback:(OnNetworkResponse)callback {
    @synchronized (lockObj) {
        [remoteDataSource sendEventAsync:event withResponseCallback:callback];
    }
}

- (void)sendEventsAsync:(NSArray *)events withResponseCallback:(OnNetworkResponse)callback {
    @synchronized (lockObj) {
        [remoteDataSource sendEventsAsync:events withResponseCallback:callback];
    }
}

- (NSInteger)storeEvent:(NSDictionary *)event {
    @synchronized (lockObj) {
        return [localDataSource storeEvent:event];
    }
}

- (NSInteger)storeEvents:(NSArray *)events {
    @synchronized (lockObj) {
        return [localDataSource storeEvents:events];
    }
}

- (NSInteger)storeTemporaryEvent:(NSDictionary *)event {
    @synchronized (lockObj) {
        return [localDataSource storeTemporaryEvent:event];
    }
}

- (NSArray *)getEvents:(NSInteger)amount {
    @synchronized (lockObj) {
        return [localDataSource getEventsWithLimit:amount];
    }
}

- (NSArray *)getTemporaryEvents:(NSInteger)amount {
    @synchronized (lockObj) {
        return [localDataSource getTemporaryEventsWithLimit:amount];
    }
}

- (NSInteger)updateSyncEventAttempts:(NSArray *)events {
    @synchronized (lockObj) {
        return [localDataSource updateEvents:events];
    }
}

- (NSInteger)deleteEvents:(NSArray *)events {
    @synchronized (lockObj) {
        return [localDataSource deleteEvents:events];
    }
}

- (NSInteger)deleteTemporaryEvents:(NSArray *)events {
    @synchronized (lockObj) {
        return [localDataSource deleteTemporaryEvents:events];
    }
}

- (NSInteger)deleteAllTemporaryEvents {
    @synchronized (lockObj) {
        return [localDataSource deleteAllTemporaryEvents];
    }
}

- (NSInteger)deleteAllEvents {
    @synchronized (lockObj) {
        return [localDataSource deleteAllEvents];
    }
}

@end
