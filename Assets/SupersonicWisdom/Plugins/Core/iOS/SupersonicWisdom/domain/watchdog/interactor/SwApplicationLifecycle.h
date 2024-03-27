//
//  SwApplicationLifecycle.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "SwBackgroundWatchdogService.h"

@protocol SwApplicationLifecycle <NSObject>

@required
- (void)startWatch;
- (void)stopWatch;
- (void)registerBackgroundWatchdog:(id<SwBackgroundWatchdog>)watchdog;
- (void)unregisterBackgroundWatchdog:(id<SwBackgroundWatchdog>)watchdog;
- (void)unregisterAllBackgroundWatchdogs;

@end
