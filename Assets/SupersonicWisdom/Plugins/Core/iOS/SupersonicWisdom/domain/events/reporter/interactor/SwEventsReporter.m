//
//  SwEventsReporter.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 21/12/2020.
//

#import "SwEventsReporter.h"
#import "SwEventReportOperation.h"

@implementation SwEventsReporter {
    id<SwEventsRepositoryProtocol> eventsRepository;
    NSOperationQueue *reporterQueue;
}

- (id)initWithRepository:(id<SwEventsRepositoryProtocol>)repository {
    if (!(self = [super init])) return nil;
    eventsRepository = repository;
    reporterQueue = [[NSOperationQueue alloc] init];
    [reporterQueue setMaxConcurrentOperationCount:3];
    return self;
}

- (void)reportEvent:(NSDictionary *)eventDetails {
    NSOperation *reportOperation = [[SwEventReportOperation alloc] initWith:eventsRepository andEvent:eventDetails];
    [reporterQueue addOperation:reportOperation];
}

@end
