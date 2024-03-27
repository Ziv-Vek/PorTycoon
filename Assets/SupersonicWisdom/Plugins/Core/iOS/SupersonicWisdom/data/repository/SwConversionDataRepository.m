//
//  SwConversionDataRepository.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import "SwConversionDataRepository.h"

@implementation SwConversionDataRepository {
    SwConversionDataLocalDataSource *localDataSource;
}

- (id)initWith:(SwConversionDataLocalDataSource *)dataSource {
    if (!(self = [super init])) return nil;
    localDataSource = dataSource;
    return self;
}

- (NSString *)getConversionData {
    return [localDataSource conversionData];
}

@end
