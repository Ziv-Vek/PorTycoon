//
//  SwConversionDataLocalApi.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import <Foundation/Foundation.h>

@interface SwConversionDataLocalApi : NSObject

- (id)initWithLocalStorage:(NSUserDefaults *)storage;
- (NSString *)conversionData;

@end
