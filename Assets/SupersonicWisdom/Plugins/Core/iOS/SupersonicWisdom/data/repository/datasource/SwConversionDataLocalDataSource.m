//
//  SwConversionDataLocalDataSource.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import "SwConversionDataLocalDataSource.h"

@implementation SwConversionDataLocalDataSource {
    SwConversionDataLocalApi *localApi;
}

- (id)initWithApi:(SwConversionDataLocalApi *)api {
    if (!(self = [super init])) return nil;
    localApi = api;
    return self;
}

- (NSString *)conversionData {
    return [localApi conversionData];
}

@end
