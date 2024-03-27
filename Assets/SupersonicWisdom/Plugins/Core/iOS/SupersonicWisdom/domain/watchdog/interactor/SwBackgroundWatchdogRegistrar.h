//
//  BackgroundWatchdogRegistrar.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwBackgroundWatchdogService.h"
#import "BackgroundWatchdogDelegate.h"

@interface SwBackgroundWatchdogRegistrar : NSObject

@property (strong) id<SwBackgroundWatchdog> watchdog;

- (id)initWithWatchdog:(SwBackgroundWatchdogService *)delegate;
- (void)registerWatchdogDelegate:(id<BackgroundWatchdogDelegate>)delegate;
- (void)unregisterWatchdogDelegate:(id<BackgroundWatchdogDelegate>)delegate;
- (void)unregisterAllDelegates;

@end
