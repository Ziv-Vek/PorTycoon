//
//  SwConversionDataLocalApi.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import "SwConversionDataLocalApi.h"

#define PREFS_EVENT_CONVERSION_DATA_KEY @"AFConversionData"

@implementation SwConversionDataLocalApi {
    NSUserDefaults *localStorage;
}

- (id)initWithLocalStorage:(NSUserDefaults *)storage {
    if (!(self = [super init])) return nil;
    localStorage = storage;
    return self;
}

- (NSString *)conversionData {
    return [localStorage stringForKey:PREFS_EVENT_CONVERSION_DATA_KEY];
}

@end
