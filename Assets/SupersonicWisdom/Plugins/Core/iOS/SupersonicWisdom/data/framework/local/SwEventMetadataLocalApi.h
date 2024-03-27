//
//  SwEventMetadataLocalApi.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventMetadataDto.h"

@interface SwEventMetadataLocalApi : NSObject

- (id)initWithLocalStorage:(NSUserDefaults *)storage;
- (void)save:(SwEventMetadataDto *)metadata;
- (SwEventMetadataDto *)get;
- (void)update:(SwEventMetadataDto *)metadata;

@end
