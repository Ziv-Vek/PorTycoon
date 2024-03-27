//
//  SwStoredEvent.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//

#import "SwStoredEvent.h"

@implementation SwStoredEvent


- (id)initWithId:(NSInteger)rowID attempt:(NSInteger)attemptNum event:(NSDictionary *)eventDetailsDict {
    if (!(self = [super init])) return nil;
    _rowId = rowID;
    _attempt = attemptNum;
    _eventDetailsDict = eventDetailsDict;
    return self;
}

- (void)increaseAttempt {
    _attempt++;
}

@end
