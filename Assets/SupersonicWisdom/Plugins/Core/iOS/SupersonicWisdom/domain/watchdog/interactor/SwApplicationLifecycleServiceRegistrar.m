//
//  ApplicationLifecycleerviceRegistrar.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import "SwApplicationLifecycleServiceRegistrar.h"

@implementation SwApplicationLifecycleServiceRegistrar : NSObject 

- (id)initWithService:(id<SwApplicationLifecycle>)service {
    if (!(self = [super init])) return nil;
    self.service = service;
    return self;
}

- (void)startService {
    [self.service startWatch];
}

- (void)stopService {
    [self.service stopWatch];
}

- (void)registerBackgroundWatchdog:(id<SwBackgroundWatchdog>)watchdog {
    [self.service registerBackgroundWatchdog:watchdog];
}

- (void)unregisterBackgroundWatchdog:(id<SwBackgroundWatchdog>)watchdog {
    [self.service registerBackgroundWatchdog:watchdog];
}

- (void)unregisterAllWatchdogs {
    [self.service unregisterAllBackgroundWatchdogs];
}

@end
