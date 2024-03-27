//
//  SwEventMetadataManager.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "SwEventMetadataManager.h"
#import "SwEventMetadataRepositoryProtocol.h"

@implementation SwEventMetadataManager {
    id<SwEventMetadataRepositoryProtocol> metadataRepository;
}

- (id)initWithRepository:(id<SwEventMetadataRepositoryProtocol>)repository {
    if (!(self = [super init])) return nil;
    metadataRepository = repository;
    return self;
}

- (void)set:(SwEventMetadataDto *)metadata {
    [metadataRepository put:metadata];
}

- (SwEventMetadataDto *)get {
    return [metadataRepository get];
}

- (void)update:(SwEventMetadataDto *)metadata {
    [metadataRepository update:metadata];
}

@end
