//
//  SwEventMetadataMapper.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 25/11/2020.
//

#import "SwEventMetadataMapper.h"
#import "SwUtils.h"
#import "SwConstants.h"

@implementation SwEventMetadataMapper

+ (NSDictionary *)map:(SwEventMetadataDto *)event {
    @synchronized (self) {
        NSString *eventStr = [event get];
        NSDictionary *dict = [SwUtils jsonStringToDict:eventStr];
        
    return dict;
    }
}

+ (SwEventMetadataDto *)reverse:(NSDictionary *)eventDict {
    @synchronized (self) {
        NSError *error;
        NSData *data = [SwUtils dataWithJSONObject:eventDict error:&error];
        NSString *str = [[NSString alloc] initWithData:data encoding:NSASCIIStringEncoding];
        SwEventMetadataDto *event = [[SwEventMetadataDto alloc] initFromString:str];
        
        return event;
    }
}

@end
