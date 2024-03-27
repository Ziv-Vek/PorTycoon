//
//  SwListStoredEventJsonMapper.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//
#import <Foundation/Foundation.h>

@interface SwListStoredEventJsonMapper : NSObject

- (NSData *)map:(NSArray *)events;
- (NSArray *)reverse:(NSDictionary *)dictEvents;

@end
