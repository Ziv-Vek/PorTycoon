//
//  SwEventsReporter.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 21/12/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventsReporterProtocol.h"
#import "SwEventsRepository.h"

@interface SwEventsReporter : NSObject <SwEventsReporterProtocol>

- (id)initWithRepository:(id<SwEventsRepositoryProtocol>)repository;

@end
