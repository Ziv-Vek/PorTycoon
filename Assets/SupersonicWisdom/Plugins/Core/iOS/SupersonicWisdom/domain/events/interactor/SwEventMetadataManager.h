//
//  SwEventMetadataManager.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventMetadataManagement.h"
#import "SwEventMetadataRepositoryProtocol.h"

@interface SwEventMetadataManager : NSObject <SwEventMetadataManagement>

- (id)initWithRepository:(id<SwEventMetadataRepositoryProtocol>)repository;

@end
