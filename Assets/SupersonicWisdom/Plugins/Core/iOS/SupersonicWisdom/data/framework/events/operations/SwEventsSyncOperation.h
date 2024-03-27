//
//  SwEventsSyncOperation.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 21/12/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventsRepositoryProtocol.h"
#import "SwStoredEventDictMapper.h"

@interface SwEventsSyncOperation : NSOperation {
    BOOL executing;
    BOOL finished;
    BOOL cancelled;
}

@property (readonly, strong) id<SwEventsRepositoryProtocol> eventsRepository;
@property (readonly, strong) SwStoredEventDictMapper *storedEventDictMapper;
@property NSInteger responseCode;

typedef void (^ResponseBlock)(NSArray<SwStoredEvent*> *events, NSInteger responseCode);

- (id)initWith:(id<SwEventsRepositoryProtocol>)repository
    withMapper:(SwStoredEventDictMapper *)seDictMapper
   andCallback:(ResponseBlock) responseCallback;

- (void)completeOperation;

@end
