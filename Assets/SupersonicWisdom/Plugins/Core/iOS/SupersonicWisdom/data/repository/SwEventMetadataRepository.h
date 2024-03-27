//
//  SwEventMetadataRepository.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventMetadataDto.h"
#import "SwEventMetadataLocalDataSource.h"
#import "SwEventMetadataRepositoryProtocol.h"

@interface SwEventMetadataRepository : NSObject <SwEventMetadataRepositoryProtocol>

- (id)initWith:(SwEventMetadataLocalDataSource *)dataSrc;

@end
