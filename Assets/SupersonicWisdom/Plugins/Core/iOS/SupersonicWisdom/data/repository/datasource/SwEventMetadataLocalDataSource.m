//
//  SwEventMetadataLocalDataSource.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "SwEventMetadataLocalDataSource.h"

@implementation SwEventMetadataLocalDataSource {
    SwEventMetadataLocalApi *localApi;
}

- (id)initWithApi:(SwEventMetadataLocalApi *)api {
    if (!(self = [super init])) return nil;
    localApi = api;
    return self;
}

- (void)put:(SwEventMetadataDto *)metadata {
    [localApi save:metadata];
}

- (SwEventMetadataDto *)get {
    return [localApi get];
}

- (void)update:(SwEventMetadataDto *)metadata {
    [localApi update:metadata];
}

@end
