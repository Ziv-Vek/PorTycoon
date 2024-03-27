//
//  SwEventMetadataLocalApi.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import "SwEventMetadataLocalApi.h"
#import "SwEventMetadataMapper.h"
#import "SwUtils.h"

#define PREFS_EVENT_METADATA_KEY @"event_metadata"

@implementation SwEventMetadataLocalApi {
    NSUserDefaults *localStorage;
}

- (id)initWithLocalStorage:(NSUserDefaults *)storage {
    if (!(self = [super init])) return nil;
    localStorage = storage;
    
    return self;
}

- (void)save:(SwEventMetadataDto *)metadata {
    NSDictionary *dict = [SwEventMetadataMapper map:metadata];
    NSError *error;
    NSData *data = [SwUtils dataWithJSONObject:dict error:&error];
    
    if (error) return;
    [localStorage setObject:data forKey:PREFS_EVENT_METADATA_KEY];
    [localStorage synchronize];
}

- (SwEventMetadataDto *)get {
    NSData *data = [localStorage dataForKey:PREFS_EVENT_METADATA_KEY];
    if (!data) return nil;
    NSError *error;
    NSDictionary *dict = [NSJSONSerialization JSONObjectWithData:data options:NSJSONWritingSortedKeys error:&error];
    
    if (error) return nil;
    SwEventMetadataDto *metadata = [SwEventMetadataMapper reverse:dict];
    return metadata;
}

- (void)update:(SwEventMetadataDto *)metadata {
    SwEventMetadataDto *oldMetadata = [self get];
    [self save:[self merge:oldMetadata With:metadata]];
}

- (SwEventMetadataDto *)merge:(SwEventMetadataDto *)oldMetadata With:(SwEventMetadataDto *)newMetadata {
    if (!oldMetadata) {
        return newMetadata;
    }
    
    NSError *error;
    NSDictionary *oldDict = [SwUtils jsonStringToDict:[oldMetadata get]];
    NSDictionary *newDict = [SwUtils jsonStringToDict:[newMetadata get]];
    NSDictionary *mergedDict = [SwUtils mergeDictionaries:oldDict other:newDict];
    
    NSString *mergedStr = [SwUtils stringWithJSONObject:mergedDict error:&error];
    
    return [[SwEventMetadataDto alloc] initFromString:mergedStr];
}

@end
