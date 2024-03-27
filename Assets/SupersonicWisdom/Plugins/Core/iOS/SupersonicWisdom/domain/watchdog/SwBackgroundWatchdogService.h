//
//  BackgroundWatchdog.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwBackgroundWatchdog.h"

@interface SwBackgroundWatchdogService : NSObject <SwBackgroundWatchdog>

@property (strong) NSMutableArray* delegates;

- (id)init;

@end
