//
//  SwConversionDataRepository.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwConversionDataRepositoryProtocol.h"
#import "SwConversionDataLocalDataSource.h"

@interface SwConversionDataRepository : NSObject <SwConversionDataRepositoryProtocol>

- (id)initWith:(SwConversionDataLocalDataSource *)dataSource;
@end
