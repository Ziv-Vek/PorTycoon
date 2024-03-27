//
//  SwEventReportOperation.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 21/12/2020.
//

#import "SwEventReportOperation.h"
#import "SwStoredEvent.h"
#import "SwConnectivityManager.h"
#import "SwConstants.h"

@implementation SwEventReportOperation
@synthesize event, eventsRepository;

- (id)initWith:(id<SwEventsRepositoryProtocol>)repository andEvent:(NSDictionary *)eventDetails {
    if (!(self = [super init])) return nil;
    eventsRepository = repository;
    event = eventDetails;
    executing = NO;
    finished = NO;
    cancelled = NO;
    
    return self;
}

- (BOOL)isConcurrent {
    return YES;
}

- (BOOL)isExecuting {
    return executing;
}

- (BOOL)isFinished {
    return finished;
}

- (BOOL)isCancelled {
    return cancelled;
}

- (void)start {
    if ([self isCancelled]) {
        [self finishOperation];
        return;
    }
    
    [self executeOperation];
}

- (void)main {
    NSInteger rowId = [eventsRepository storeTemporaryEvent:event];
    if (rowId <= 0) {
        [self completeOperation];
        return;
    }
    
    SwStoredEvent *seEvent = [[SwStoredEvent alloc] initWithId:rowId attempt:INITIAL_EVENT_ATTEMPT event:event];
    NSArray<SwStoredEvent*> *events = @[seEvent];
    if ([self isCancelled]) {
        [self completeOperation];
        return;
    }
    
    [eventsRepository sendEventsAsync:events withResponseCallback:^(NSString *key, BOOL successfully, NSInteger responseCode, NSData *reponseBody) {
        if ([self isCancelled]) {
            [self completeOperation];
            return;
        }
        
        [self handleResponseForEvents:events withStatus:successfully andCode:responseCode];
        [self completeOperation];
    }];
}

- (void)executeOperation {
    [self willChangeValueForKey:OP_KEY_IS_EXECUTING];
    [NSThread detachNewThreadSelector:@selector(main) toTarget:self withObject:nil];
    executing = YES;
    [self didChangeValueForKey:OP_KEY_IS_EXECUTING];
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

- (void)completeOperation {
    [self willChangeValueForKey:OP_KEY_IS_FINISHED];
    [self willChangeValueForKey:OP_KEY_IS_EXECUTING];
    executing = NO;
    finished = YES;
    [self didChangeValueForKey:OP_KEY_IS_EXECUTING];
    [self didChangeValueForKey:OP_KEY_IS_FINISHED];
}

- (void)handleResponseForEvents:(NSArray<SwStoredEvent*> *)events withStatus:(BOOL)success andCode:(NSInteger)responseCode {
    if (success || responseCode == RESPONSE_CODE_BAD_REQUEST) {
        [eventsRepository deleteTemporaryEvents:events];
    } else if (responseCode == RESPONSE_CODE_NO_INTERNET) {
        if ([eventsRepository storeEvents:events] > 0) {
            [eventsRepository deleteTemporaryEvents:events];
        }
    } else {
        if ([eventsRepository storeEvents:events] > 0) {
            [eventsRepository deleteTemporaryEvents:events];
        }
    }
}

@end
