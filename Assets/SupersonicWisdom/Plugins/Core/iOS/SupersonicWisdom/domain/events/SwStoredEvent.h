//
//  SwStoredEvent.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//
#import <Foundation/Foundation.h>

@interface SwStoredEvent : NSObject

@property (atomic, readonly) NSInteger rowId;
@property (atomic, readonly) NSInteger attempt;
@property (atomic, readonly, strong) NSDictionary *eventDetailsDict;

- (id)initWithId:(NSInteger)rowID attempt:(NSInteger)attemptNum event:(NSDictionary *)eventDetailsDict;
- (void)increaseAttempt;

@end
