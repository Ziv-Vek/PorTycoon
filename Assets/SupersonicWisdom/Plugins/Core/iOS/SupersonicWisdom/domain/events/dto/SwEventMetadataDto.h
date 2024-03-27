//
//  EventMetadataDto.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import <Foundation/Foundation.h>

@interface SwEventMetadataDto : NSObject

- (id)initFromString:(NSString *)metadataJson;
- (NSString *)get;

@end
