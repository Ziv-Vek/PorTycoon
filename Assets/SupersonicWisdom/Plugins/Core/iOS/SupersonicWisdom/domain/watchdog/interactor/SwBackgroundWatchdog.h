//
//  SwBackgroundWatchdog.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "BackgroundWatchdogDelegate.h"

@protocol SwBackgroundWatchdog <NSObject, BackgroundWatchdogDelegate>

@required
- (void)registerBackgroundDelegate:(id<BackgroundWatchdogDelegate>)delegate;
- (void)unregisterBackgroundDelegate:(id<BackgroundWatchdogDelegate>)delegate;
- (void)unregisterAllDelegates;

@end
