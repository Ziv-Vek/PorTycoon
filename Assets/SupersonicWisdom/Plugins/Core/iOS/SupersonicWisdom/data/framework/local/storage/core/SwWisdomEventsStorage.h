//
//  SwWisdomEventsStorage.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/12/2020.
//

#import <Foundation/Foundation.h>
#import "SwWisdomStorageProtocol.h"
#import "SwWisdomDBManager.h"

@interface SwWisdomEventsStorage : NSObject <SwWisdomStorageProtocol>

- (id)initWithDatabase:(SwWisdomDBManager *)databaseManager;


@end
