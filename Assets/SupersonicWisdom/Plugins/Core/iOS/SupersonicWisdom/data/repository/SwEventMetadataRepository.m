//
//  SwEventMetadataRepository.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "SwEventMetadataRepository.h"

@implementation SwEventMetadataRepository {
    SwEventMetadataLocalDataSource *localDataSource;
}

- (id)initWith:(SwEventMetadataLocalDataSource *)dataSrc {
    if (!(self = [super init])) return nil;
    localDataSource = dataSrc;
    return self;
}

- (SwEventMetadataDto *)get {
    return [localDataSource get];
}

- (void)put:(SwEventMetadataDto *)metadata {
    [localDataSource put:metadata];
}

- (void)update:(SwEventMetadataDto *)metadata { 
    [localDataSource update:metadata];
}

@end
