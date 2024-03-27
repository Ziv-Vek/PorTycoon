//
//  SwEventMetadataMapper.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 25/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventMetadataDto.h"

@interface SwEventMetadataMapper : NSObject

+ (NSDictionary *)map:(SwEventMetadataDto *)event;
+ (SwEventMetadataDto *)reverse:(NSDictionary *)eventDict;

@end
