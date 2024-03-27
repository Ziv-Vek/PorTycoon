//
//  ApplicationLifecycleService.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import "SwApplicationLifecycleService.h"

@implementation SwApplicationLifecycleService

- (id)init {
    if (!(self = [super init])) return nil;
    self.watchdogs = [NSMutableArray array];
    return self;
}

- (void)startWatch {
    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(didEnterBackground)
                                                 name:UIApplicationDidEnterBackgroundNotification
                                               object:nil];

    [[NSNotificationCenter defaultCenter] addObserver:self
                                             selector:@selector(willEnterForeground)
                                                 name:UIApplicationWillEnterForegroundNotification
                                               object:nil];
}

- (void)stopWatch {
    [[NSNotificationCenter defaultCenter] removeObserver:self
                                                    name:UIApplicationDidEnterBackgroundNotification
                                                  object:nil];
    
    [[NSNotificationCenter defaultCenter] removeObserver:self
                                                    name:UIApplicationWillEnterForegroundNotification
                                                  object:nil];
}

- (void)registerBackgroundWatchdog:(id<SwBackgroundWatchdog>)watchdog {
    [self.watchdogs addObject:watchdog];
}

- (void)unregisterBackgroundWatchdog:(id<SwBackgroundWatchdog>)watchdog {
    [self.watchdogs removeObject:watchdog];
}

- (void)unregisterAllBackgroundWatchdogs {
    [self.watchdogs removeAllObjects];
}

- (void)didEnterBackground {
    for (id<SwBackgroundWatchdog> watchdog in self.watchdogs) {
        if (watchdog != nil) {
            [watchdog onAppMovedToBackground];
        }
    }
}

- (void)willEnterForeground {
    for (id<SwBackgroundWatchdog> watchdog in self.watchdogs) {
        if (watchdog != nil) {
            [watchdog onAppMovedToForeground];
        }
    }
}

@end
