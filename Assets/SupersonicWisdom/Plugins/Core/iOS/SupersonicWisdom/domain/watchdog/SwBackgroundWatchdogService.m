//
//  BackgroundWatchdog.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import "SwBackgroundWatchdogService.h"

@implementation SwBackgroundWatchdogService

- (id)init {
    if (!(self = [super init])) return nil;
    self.delegates = [NSMutableArray array];
    return self;
}

- (void)registerBackgroundDelegate:(id<BackgroundWatchdogDelegate>)delegate {
    [self.delegates addObject:delegate];
}

- (void)unregisterBackgroundDelegate:(id<BackgroundWatchdogDelegate>)delegate {
    [self.delegates removeObject:delegate];
}

- (void)unregisterAllDelegates {
    [self.delegates removeAllObjects];
}

- (void)onAppMovedToBackground {
    for (id<BackgroundWatchdogDelegate> delegate in self.delegates) {
        if (delegate != nil && [delegate respondsToSelector:@selector(onAppMovedToBackground)]) {
            [delegate onAppMovedToBackground];
        }
    }
}

- (void)onAppMovedToForeground {
    for (id<BackgroundWatchdogDelegate> delegate in self.delegates) {
        if (delegate != nil && [delegate respondsToSelector:@selector(onAppMovedToForeground)]) {
            [delegate onAppMovedToForeground];
        }
    }
}

@end
