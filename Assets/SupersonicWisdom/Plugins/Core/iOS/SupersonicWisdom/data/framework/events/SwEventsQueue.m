//
//  SwEventsQueue.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/12/2020.
//

#import "SwEventsQueue.h"
#import "SwEventsSyncOperation.h"
#import "SwEventsTablesMergeOperation.h"
#import "SwConnectivityManager.h"
#import "SwConstants.h"

#define MAX_RETRIES 10
#define SYNC_INTERVAL 8.0
#define CONNECTIVITY_ISSUE_DELAY 10

@implementation SwEventsQueue {
    SwEventsRepository *eventsRepository;
    SwStoredEventDictMapper *storedEventDictMapper;
    NSOperationQueue *syncQueue;
    NSTimer *scheduler;
    BOOL isHandlerInitialized;
    NSTimeInterval initialSyncInterval;
    NSInteger currentRetry;
}

- (id)initWith:(SwEventsRepository *)repository withMapper:(SwStoredEventDictMapper *)seDictMapper andInterval:(NSTimeInterval)interval {
    if (!(self = [super init])) return nil;
    syncQueue = [[NSOperationQueue alloc] init];
    [syncQueue setQualityOfService:NSQualityOfServiceUserInteractive];
    [syncQueue setMaxConcurrentOperationCount:1];
    eventsRepository = repository;
    storedEventDictMapper = seDictMapper;
    isHandlerInitialized = NO;
    initialSyncInterval = interval;
    
    return self;
}

- (void)initializeHandler {
    NSOperation *mergeTablesOp = [[SwEventsTablesMergeOperation alloc] initWithRepository:eventsRepository
                                                                       andEventDictMapper:storedEventDictMapper];
    [self resetRetries];
    [syncQueue addOperation:mergeTablesOp];
    scheduler = [self createSchedulerWithInterval:initialSyncInterval];
    [scheduler fire];
    isHandlerInitialized = YES;
}

- (void)startQueue {
    if (!isHandlerInitialized) {
        [self initializeHandler];
    }
}

- (void)stopQueue {
    if (isHandlerInitialized) {
        [self destroyHandler];
    }
}

- (void)performSync {
    if (![self shouldPerformSync]) {
        [self restartSchedulerWithInterval:[self getNextSyncInterval]];
        return;
    }
    
    SwEventsSyncOperation *syncOp = [[SwEventsSyncOperation alloc] initWith:eventsRepository
                                                                 withMapper:storedEventDictMapper
                                                                andCallback:^(NSArray<SwStoredEvent *> *events, NSInteger responseCode) {
        BOOL isHandled = [self handleResponse:events withResponseCode:responseCode];
        if (isHandled) {
            [self restartSchedulerOnMainThreadWithInterval:[self getNextSyncInterval]];
        } else {
            [self restartSchedulerOnMainThreadWithInterval:CONNECTIVITY_ISSUE_DELAY];
        }
    }];
    [syncQueue addOperation:syncOp];
}

- (BOOL)shouldPerformSync {
    NSArray *queuedOperations = [syncQueue operations];
    for (NSObject *operation in queuedOperations) {
        if ([operation class] == [SwEventsSyncOperation class]) {
            return NO;
        }
    }
    
    NSArray *events = [eventsRepository getEvents:EVENTS_LIMIT];
    if (!events || events.count == 0) {
        return NO;
    }
    
    return YES;
}

- (void)destroyHandler {
    [self stopScheduler];
    [syncQueue cancelAllOperations];
    isHandlerInitialized = NO;
}

- (void)updateSyncInterval:(NSTimeInterval)interval {
    initialSyncInterval = interval;
}

/*
 * Calculates the next sync interval with exponential backoff policy
 * https://developer.android.com/reference/android/app/job/JobInfo#BACKOFF_POLICY_EXPONENTIAL
 */
- (NSTimeInterval)getNextSyncInterval {
    NSInteger numFailures = MIN(currentRetry, MAX_RETRIES);
    NSInteger exponent = numFailures - 1;
    NSInteger power = pow(2, exponent);
    
    return (initialSyncInterval * power);
}

- (void)restartSchedulerWithInterval:(NSTimeInterval)interval {
    [self stopScheduler];
    scheduler = [self createSchedulerWithInterval:interval];
}

- (void)restartSchedulerOnMainThreadWithInterval:(NSTimeInterval)interval {
    dispatch_sync(dispatch_get_main_queue(), ^{
        [self restartSchedulerWithInterval:interval];
    });
}

- (void)stopScheduler {
    if (scheduler && [scheduler isValid]) {
        [scheduler invalidate];
    }
    
    scheduler = nil;
}

- (NSTimer *)createSchedulerWithInterval:(NSTimeInterval)interval {
    return [NSTimer scheduledTimerWithTimeInterval:interval target:self selector:@selector(performSync) userInfo:nil repeats:NO];
}

- (BOOL)handleResponse:(NSArray<SwStoredEvent*> *)events withResponseCode:(NSInteger)responseCode {
    if (responseCode >= RESPONSE_CODE_OK && responseCode <= RESPONSE_CODE_BAD_REQUEST) {
        /*
         * Will handle BAD REQUEST together with success request to prevent code duplication,
         * in case logic if SUCCESS REQUEST will be changed check if need to separate logic of BAD REQUEST
         */
        [self handleSuccessResponse:events];
    } else if (responseCode != RESPONSE_CODE_NO_INTERNET) {
        [self handleErrorResponse:events];
    }
    
    return (responseCode != RESPONSE_CODE_NO_INTERNET);
}

- (void)handleSuccessResponse:(NSArray<SwStoredEvent*> *)events {
    [self resetRetries];
    [eventsRepository deleteEvents:events];
}

- (void)handleErrorResponse:(NSArray<SwStoredEvent*> *)events {
    if (currentRetry >= MAX_RETRIES) {
        [eventsRepository deleteAllTemporaryEvents];
        [eventsRepository deleteAllEvents];
    }
    
    currentRetry++;
}

- (void)resetRetries {
    currentRetry = 1;
}

@end
