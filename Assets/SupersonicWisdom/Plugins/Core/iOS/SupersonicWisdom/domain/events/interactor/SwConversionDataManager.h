//
//  SwConversionDataManager.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwConversionDataManagement.h"
#import "SwConversionDataRepositoryProtocol.h"

@interface SwConversionDataManager : NSObject <SwConversionDataManagement>

- (id)initWith:(id<SwConversionDataRepositoryProtocol>)repository;

@end
