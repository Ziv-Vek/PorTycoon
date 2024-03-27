//
//  BackgroundWatchdogRegistrar.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import "SwBackgroundWatchdogRegistrar.h"
#import "SwBackgroundWatchdogService.h"

@implementation SwBackgroundWatchdogRegistrar

- (id)initWithWatchdog:(id<SwBackgroundWatchdog>)watchdog {
    if (!(self = [super init])) return nil;
    self.watchdog = watchdog;
    return self;
}

- (void)registerWatchdogDelegate:(id<BackgroundWatchdogDelegate>)delegate {
    [self.watchdog registerBackgroundDelegate:delegate];
}

- (void)unregisterWatchdogDelegate:(id<BackgroundWatchdogDelegate>)delegate {
    [self.watchdog unregisterBackgroundDelegate:delegate];
}

- (void)unregisterAllDelegates {
    [self.watchdog unregisterAllDelegates];
}

@end
