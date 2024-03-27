//
//  SwStoredEventDictMapper.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//

#import <Foundation/Foundation.h>
#import "SwStoredEvent.h"

@interface SwStoredEventDictMapper : NSObject

- (NSDictionary *)map:(SwStoredEvent *)storedEvent;
- (SwStoredEvent *)reverse:(NSDictionary *)eventDict;

@end
