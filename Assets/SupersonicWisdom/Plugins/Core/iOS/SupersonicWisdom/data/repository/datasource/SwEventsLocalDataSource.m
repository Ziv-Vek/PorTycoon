//
//  SwEventsLocalDataSource.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//

#import "SwEventsLocalDataSource.h"

@implementation SwEventsLocalDataSource {
    SwEventsLocalApi *localApi;
}

- (id)initWithLocalApi:(SwEventsLocalApi *)api {
    if (!(self = [super init])) return nil;
    localApi = api;
    
    return self;
}

- (NSInteger)storeEvent:(NSDictionary *)event {
    return [localApi storeEvent:event];
}

- (NSInteger)storeEvents:(NSArray *)events {
    return [localApi storeEvents:events];
}

- (NSInteger)storeTemporaryEvent:(NSDictionary *)event {
    return [localApi storeTemporaryEvent:event];
}

- (NSArray *)getEventsWithLimit:(NSInteger)limit {
    return [localApi getEventsWithLimit:limit];
}

- (NSArray *)getTemporaryEventsWithLimit:(NSInteger)limit {
    return [localApi getTemporaryEventsWithLimit:limit];
}

- (NSInteger)updateEvents:(NSArray *)events {
    return [localApi updateEvents:events];
}

- (NSInteger)deleteEvents:(NSArray *)events {
    return [localApi deleteEvents:events];
}

- (NSInteger)deleteTemporaryEvents:(NSArray *)events {
    return [localApi deleteTemporaryEvents:events];
}

- (NSInteger)deleteAllTemporaryEvents {
    return [localApi deleteAllTemporaryEvents];
}

- (NSInteger)deleteAllEvents {
    return [localApi deleteAllEvents];
}

@end
