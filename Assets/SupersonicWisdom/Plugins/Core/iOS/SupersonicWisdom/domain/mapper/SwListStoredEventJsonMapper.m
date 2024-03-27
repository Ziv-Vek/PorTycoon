//
//  SwListStoredEventJsonMapper.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//

#import "SwListStoredEventJsonMapper.h"
#import "SwStoredEvent.h"
#import "SwConstants.h"
#import "SwUtils.h"

@implementation SwListStoredEventJsonMapper {
    NSObject *lockObj;
}

- (id)init{
    if (!(self = [super init])) return nil;
    lockObj = [NSObject new];
    return self;
}

- (NSData *)map:(NSArray *)events {
    @synchronized (lockObj) {
        NSMutableArray *parsedEvents = [NSMutableArray array];
        NSError *jsonError;
        if (!events || events.count == 0) {
            return [SwUtils dataWithJSONObject:parsedEvents error:&jsonError];
        }
        
        for (SwStoredEvent *event in events) {
            NSMutableDictionary *eventDetails = [[event eventDetailsDict] mutableCopy];
            NSNumber *attemptNumber = [NSNumber numberWithLong:[event attempt]];
            [eventDetails setValue:attemptNumber forKey:KEY_ATTEMPTS];
            [parsedEvents addObject:eventDetails];
        }
        
        NSDictionary *dict = @{@"events" : parsedEvents};
        return [SwUtils dataWithJSONObject:dict error:&jsonError];
    }
}

- (NSArray *)reverse:(NSDictionary *)dictEvents {
    NSObject *events = [dictEvents objectForKey: @"events"];
    if (!events) return @[];
    if (![events isKindOfClass: [NSArray class]]) return @[];
    NSArray *eventsList = (NSArray *)events;
    [SWSDKLogger logMessage:@"Unsupported method"];
    
    return eventsList;
}

@end
