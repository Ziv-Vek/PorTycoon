//
//  SwEventMetadataManagement.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import "SwEventMetadataDto.h"

@protocol SwEventMetadataManagement <NSObject>

@required
- (void)set:(SwEventMetadataDto *)metadata;
- (SwEventMetadataDto *)get;
- (void)update:(SwEventMetadataDto *)metadata;

@end
