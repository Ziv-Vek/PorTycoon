//
//  SwEventsLocalApi.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//

#import "SwEventsLocalApi.h"
#import "SwStorageUtils.h"
#import "SwConstants.h"
#import "SwUtils.h"

@implementation SwEventsLocalApi {
    id<SwWisdomStorageProtocol> localStorage;
    SwStoredEventDictMapper *storedEventMapper;
}

- (id)initWithStorage:(id<SwWisdomStorageProtocol>)storage
   storedEventsMapper:(SwStoredEventDictMapper *)seMapper {
    if (!(self = [super init])) return nil;
    localStorage = storage;
    storedEventMapper = seMapper;
    
    return self;
}

- (NSInteger)storeEvent:(NSDictionary *)event {
    NSDictionary *eventDict = event;
    NSDictionary *dict = @{
        SW_COLUMN_ATTEMPT : [@(INITIAL_EVENT_ATTEMPT) stringValue],
        SW_COLUMN_EVENT : eventDict
    };
    
    return [localStorage insertInto:SW_BACKUP_EVENTS_TABLE values:dict];
}

- (NSInteger)storeTemporaryEvent:(NSDictionary *)event {
    NSDictionary *eventDict = event;
    NSDictionary *dict = @{
        SW_COLUMN_ATTEMPT : [@(INITIAL_EVENT_ATTEMPT) stringValue],
        SW_COLUMN_EVENT : [SwUtils replaceNilDict:eventDict]
    };
    
    return [localStorage insertInto:SW_TMP_EVENTS_TABLE values:dict];
}

- (NSInteger)storeEvents:(NSArray *)events {
    NSMutableArray *parsedEvents = [NSMutableArray array];
    for (SwStoredEvent *event in events) {
        NSDictionary *parsedEvent = [SwUtils replaceNilDict:[event eventDetailsDict]];
        NSError *jsonError;
        NSData *jsonData = [SwUtils dataWithJSONObject:parsedEvent error:&jsonError];
        NSString *eventContent = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        
        NSDictionary *eventDict = @{
            SW_COLUMN_ATTEMPT : [@(event.attempt) stringValue],
            SW_COLUMN_EVENT : eventContent
        };
        
        [parsedEvents addObject:eventDict];
    }
    return [localStorage insertInto:SW_BACKUP_EVENTS_TABLE listWithValues:parsedEvents];
}

- (NSArray *)getEventsWithLimit:(NSInteger)limit {
    NSString *orderBy = [NSString stringWithFormat:@"%@ DESC", SW_COLUMN_ID];
    return [localStorage queryFrom:SW_BACKUP_EVENTS_TABLE orderBy:orderBy limit:limit];
}

- (NSArray *)getTemporaryEventsWithLimit:(NSInteger)limit {
    NSString *orderBy = [NSString stringWithFormat:@"%@ DESC", SW_COLUMN_ID];
    return [localStorage queryFrom:SW_TMP_EVENTS_TABLE orderBy:orderBy limit:limit];
}

- (NSInteger)updateEvents:(NSArray *)events {
    if (!events || events.count == 0) {
        return 0;
    }
    
    NSMutableArray *parsedEvents = [NSMutableArray array];
    for (SwStoredEvent *event in events) {
        [parsedEvents addObject:[storedEventMapper map:event]];
    }
    
    NSString *whereClause = [NSString stringWithFormat:@"%@ = ?", SW_COLUMN_ID];
    return [localStorage updateIn:SW_BACKUP_EVENTS_TABLE
                      whereClause:whereClause
                        whereArgs:@[SW_COLUMN_ID]
                   listWithValues:parsedEvents];
}

- (NSInteger)deleteEvents:(NSArray *)events {
    return [self deleteFromTable:SW_BACKUP_EVENTS_TABLE events:events];
}

- (NSInteger)deleteTemporaryEvents:(NSArray *)events {
    return [self deleteFromTable:SW_TMP_EVENTS_TABLE events:events];
}

- (NSInteger)deleteFromTable:(NSString *)table events:(NSArray *)events {
    if (!events || events.count == 0) {
        return 0;
    }
    
    SwStoredEvent *firstEvent = [events objectAtIndex:0];
    SwStoredEvent *lastEvent = [events objectAtIndex:(events.count - 1)];
    NSInteger min = MIN(firstEvent.rowId, lastEvent.rowId);
    NSInteger max = MAX(firstEvent.rowId, lastEvent.rowId);
    NSMutableArray *whereArgs = [NSMutableArray array];
    [whereArgs addObject:[@(min) stringValue]];
    [whereArgs addObject:[@(max) stringValue]];
    NSString *whereClause = [NSString stringWithFormat:@"%@ BETWEEN ? AND ?", SW_COLUMN_ID];
    return [localStorage deleteFrom:table
                        whereClause:whereClause
                          whereArgs:whereArgs];
}

- (NSInteger)deleteAllEvents {
    return [localStorage deleteAllFrom:SW_BACKUP_EVENTS_TABLE];
}

- (NSInteger)deleteAllTemporaryEvents {
    return [localStorage deleteAllFrom:SW_TMP_EVENTS_TABLE];
}

@end
