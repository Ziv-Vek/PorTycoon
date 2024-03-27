//
//  SwEventMetadataDto.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 28/12/2020.
//

#import "SwEventMetadataDto.h"


@implementation SwEventMetadataDto{
    NSString *jsonString;
}

- (id)initFromString:(NSString *)metadataJson {
    if (!(self = [super init])) return nil;
    self -> jsonString = metadataJson;
    
    return self;
}

-(NSString *)get{
    return jsonString;
}

@end
