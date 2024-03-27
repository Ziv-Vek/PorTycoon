//
//  SwEventReportOperation.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 21/12/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventsRepositoryProtocol.h"

@interface SwEventReportOperation : NSOperation {
    BOOL executing;
    BOOL finished;
    BOOL cancelled;
}

@property (readonly, strong) NSDictionary *event;
@property (readonly, strong) id<SwEventsRepositoryProtocol> eventsRepository;

- (id)initWith:(id<SwEventsRepositoryProtocol>)repository andEvent:(NSDictionary *)eventDetails;
- (void)completeOperation;

@end
