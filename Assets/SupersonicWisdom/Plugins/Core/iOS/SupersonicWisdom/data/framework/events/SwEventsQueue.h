//
//  SwEventsQueue.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/12/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventsQueueProtocol.h"
#import "SwEventsRepository.h"
#import "SwEventsSyncOperation.h"

@interface SwEventsQueue : NSObject <SwEventsQueueProtocol>

- (id)initWith:(SwEventsRepository *)repository withMapper:(SwStoredEventDictMapper *)seDictMapper andInterval:(NSTimeInterval)interval;
- (void)initializeHandler;
- (void)destroyHandler;

@end
