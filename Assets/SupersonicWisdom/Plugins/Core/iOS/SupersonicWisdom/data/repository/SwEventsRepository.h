//
//  SwEventsRepository.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventsRemoteDataSource.h"
#import "SwEventsLocalDataSource.h"
#import "SwEventsRepositoryProtocol.h"

@interface SwEventsRepository : NSObject <SwEventsRepositoryProtocol>

- (id)initWithRemoteDataSource:(SwEventsRemoteDataSource *)remote
               localDataSource:(SwEventsLocalDataSource *)local;

@end
