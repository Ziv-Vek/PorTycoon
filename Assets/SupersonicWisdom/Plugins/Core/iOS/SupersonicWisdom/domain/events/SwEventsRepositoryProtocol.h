//
//  SwEventsRepositoryProtocol.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "SwNetworkCallbacks.h"

@protocol SwEventsRepositoryProtocol <NSObject>

@required
- (void)sendEventAsync:(NSDictionary *)event withResponseCallback:(OnNetworkResponse)callback;
- (void)sendEventsAsync:(NSArray *)events withResponseCallback:(OnNetworkResponse)callback;

- (NSInteger)storeEvent:(NSDictionary *)event;
- (NSInteger)storeEvents:(NSArray *)events;
- (NSInteger)storeTemporaryEvent:(NSDictionary *)event;

- (NSArray *)getEvents:(NSInteger)amount;
- (NSArray *)getTemporaryEvents:(NSInteger)amount;

- (NSInteger)updateSyncEventAttempts:(NSArray *)events;
- (NSInteger)deleteEvents:(NSArray *)events;
- (NSInteger)deleteTemporaryEvents:(NSArray *)events;
- (NSInteger)deleteAllTemporaryEvents;
- (NSInteger)deleteAllEvents;

@end
