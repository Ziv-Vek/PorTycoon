//
//  SwEventsSyncOperation.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 21/12/2020.
//

#import "SwEventsSyncOperation.h"
#import "SwConstants.h"

@implementation SwEventsSyncOperation {
    ResponseBlock responseHandler;
}

@synthesize eventsRepository, storedEventDictMapper, responseCode;

- (id)initWith:(id<SwEventsRepositoryProtocol>)repository
    withMapper:(SwStoredEventDictMapper *)seDictMapper
   andCallback:(ResponseBlock)responseCallback {
    
    if (!(self = [super init])) return nil;
    eventsRepository = repository;
    storedEventDictMapper = seDictMapper;
    executing = NO;
    finished = NO;
    cancelled = NO;
    responseHandler = responseCallback;
    
    return self;
}

- (BOOL)isCancelled {
    return cancelled;
}

- (BOOL)isExecuting {
    return executing;
}

- (BOOL)isFinished {
    return finished;
}

- (BOOL)isConcurrent {
    return YES;
}

- (void)start {
    if ([self isCancelled]) {
        [self finishOperation];
    
        return;
    }
    
    [self executeOperation];
}

- (void)main {
    NSArray *eventDicts = [eventsRepository getEvents:EVENTS_LIMIT];
    if (!eventDicts || eventDicts.count == 0) {
        [self completeOperation];
        return;
    }
    
    NSMutableArray<SwStoredEvent*> *events = [NSMutableArray array];
    for (NSDictionary *eventDict in eventDicts) {
        if ([self isCancelled]) {
            [self completeOperation];
            return;
        }
        
        SwStoredEvent *seEvent = [storedEventDictMapper reverse:eventDict];
        [seEvent increaseAttempt];
        [events addObject:seEvent];
    }
    
    [eventsRepository updateSyncEventAttempts:events];
    [eventsRepository sendEventsAsync:events withResponseCallback:^(NSString *key, BOOL successfully, NSInteger responseCode, NSData *responseBody) {
        if ([self isCancelled]) {
            [self completeOperation];
            return;
        }
        
        [self handleResponseForEvents:events withStatus:successfully andCode:responseCode];
        [self completeOperation];
    }];
}

- (void)cancel {
    [self willChangeValueForKey:OP_KEY_IS_CANCELLED];
    cancelled = YES;
    [self didChangeValueForKey:OP_KEY_IS_CANCELLED];
}

- (void)finishOperation {
    [self willChangeValueForKey:OP_KEY_IS_FINISHED];
    finished = YES;
    [self didChangeValueForKey:OP_KEY_IS_FINISHED];
}

- (void)executeOperation {
    [self willChangeValueForKey:OP_KEY_IS_EXECUTING];
    [NSThread detachNewThreadSelector:@selector(main) toTarget:self withObject:nil];
    executing = YES;
    [self didChangeValueForKey:OP_KEY_IS_EXECUTING];
}

- (void)completeOperation {
    [self willChangeValueForKey:OP_KEY_IS_FINISHED];
    [self willChangeValueForKey:OP_KEY_IS_EXECUTING];
    executing = NO;
    finished = YES;
    [self didChangeValueForKey:OP_KEY_IS_EXECUTING];
    [self didChangeValueForKey:OP_KEY_IS_FINISHED];
}

- (void)handleResponseForEvents:(NSArray<SwStoredEvent*> *)events withStatus:(BOOL)success andCode:(NSInteger)responseCode {
    responseHandler(events, responseCode);
}

@end
