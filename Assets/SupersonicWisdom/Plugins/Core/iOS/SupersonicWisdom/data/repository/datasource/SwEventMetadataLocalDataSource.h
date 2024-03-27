//
//  SwEventMetadataLocalDataSource.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventMetadataLocalApi.h"

@interface SwEventMetadataLocalDataSource : NSObject

- (id)initWithApi:(SwEventMetadataLocalApi *)api;
- (void)put:(SwEventMetadataDto *)metadata;
- (SwEventMetadataDto *)get;
- (void)update:(SwEventMetadataDto *)metadata;

@end
