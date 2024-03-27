//
//  SwEventsLocalApi.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//

#import <Foundation/Foundation.h>
#import "SwWisdomStorageProtocol.h"
#import "SwStoredEventDictMapper.h"

@interface SwEventsLocalApi : NSObject

- (id)initWithStorage:(id<SwWisdomStorageProtocol>)storage storedEventsMapper:(SwStoredEventDictMapper *)seMapper;


- (NSInteger)storeEvent:(NSDictionary *)event;
- (NSInteger)storeTemporaryEvent:(NSDictionary *)event;
- (NSInteger)storeEvents:(NSArray *)events;
- (NSArray *)getEventsWithLimit:(NSInteger)limit;
- (NSArray *)getTemporaryEventsWithLimit:(NSInteger)limit;
- (NSInteger)updateEvents:(NSArray *)events;
- (NSInteger)deleteEvents:(NSArray *)events;
- (NSInteger)deleteTemporaryEvents:(NSArray *)events;
- (NSInteger)deleteAllTemporaryEvents;
- (NSInteger)deleteAllEvents;


@end
