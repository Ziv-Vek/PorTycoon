//
//  SwEventsTablesMergeOperation.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 21/12/2020.
//

#import "SwEventsTablesMergeOperation.h"
#import "SwConstants.h"

@implementation SwEventsTablesMergeOperation

@synthesize eventsRepository, storedEventDictMapper;

- (id)initWithRepository:(id<SwEventsRepositoryProtocol>)repository andEventDictMapper:(SwStoredEventDictMapper *)seDictMapper{
    if (!(self = [super init])) return nil;
    eventsRepository = repository;
    storedEventDictMapper = seDictMapper;
    
    return self;
}

- (void)main {
    NSArray *eventDicts = [eventsRepository getTemporaryEvents:EVENTS_LIMIT];
    if (!eventDicts || eventDicts.count == 0) {
        return;
    }
    NSMutableArray<SwStoredEvent*> *events = [NSMutableArray array];
    for (NSDictionary *eventDict in eventDicts) {
        SwStoredEvent *event = [storedEventDictMapper reverse:eventDict];
        [events addObject:event];
    }
    
    if ([eventsRepository storeEvents:events] > 0) {
        [eventsRepository deleteTemporaryEvents:events];
    } else {
        [events removeAllObjects];
    }
}

@end
