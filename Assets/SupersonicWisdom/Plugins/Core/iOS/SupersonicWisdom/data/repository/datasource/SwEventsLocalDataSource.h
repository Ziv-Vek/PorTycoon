//
//  SwEventsLocalDataSource.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventsLocalApi.h"

@interface SwEventsLocalDataSource : NSObject

- (id)initWithLocalApi:(SwEventsLocalApi *)api;
- (NSInteger)storeEvent:(NSDictionary *)event;
- (NSInteger)storeEvents:(NSArray *)events;
- (NSInteger)storeTemporaryEvent:(NSDictionary *)event;
- (NSArray *)getEventsWithLimit:(NSInteger)limit;
- (NSArray *)getTemporaryEventsWithLimit:(NSInteger)limit;
- (NSInteger)updateEvents:(NSArray *)events;
- (NSInteger)deleteEvents:(NSArray *)events;
- (NSInteger)deleteTemporaryEvents:(NSArray *)events;
- (NSInteger)deleteAllTemporaryEvents;
- (NSInteger)deleteAllEvents;

@end
