//
//  SwConversionDataManager.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import "SwConversionDataManager.h"

@implementation SwConversionDataManager {
    id<SwConversionDataRepositoryProtocol> conversionDataRepository;
}

- (id)initWith:(id<SwConversionDataRepositoryProtocol>)repository {
    if (!(self = [super init])) return nil;
    conversionDataRepository = repository;
    return self;
}

- (NSString *)conversionData {
    return [conversionDataRepository getConversionData];
}

@end
