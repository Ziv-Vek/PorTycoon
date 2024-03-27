//
//  SwConversionDataLocalDataSource.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwConversionDataLocalApi.h"

@interface SwConversionDataLocalDataSource : NSObject

- (id)initWithApi:(SwConversionDataLocalApi *)api;
- (NSString *)conversionData;

@end

