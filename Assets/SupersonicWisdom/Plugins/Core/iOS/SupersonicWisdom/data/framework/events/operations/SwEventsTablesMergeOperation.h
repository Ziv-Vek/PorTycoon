//
//  SwEventsTablesMergeOperation.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 21/12/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventsRepositoryProtocol.h"
#import "SwStoredEventDictMapper.h"

@interface SwEventsTablesMergeOperation : NSOperation {
    BOOL executing;
    BOOL finished;
    BOOL cancelled;
}

@property (readonly) id<SwEventsRepositoryProtocol> eventsRepository;
@property (readonly, strong) SwStoredEventDictMapper *storedEventDictMapper;

- (id)initWithRepository:(id<SwEventsRepositoryProtocol>)repository andEventDictMapper:(SwStoredEventDictMapper *)seDictMapper;

@end
