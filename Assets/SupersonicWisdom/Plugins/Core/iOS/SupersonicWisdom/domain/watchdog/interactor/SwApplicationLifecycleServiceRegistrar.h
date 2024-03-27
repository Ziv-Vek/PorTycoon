//
//  ApplicationLifecycleerviceRegistrar.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwApplicationLifecycle.h"
#import "SwBackgroundWatchdog.h"

@interface SwApplicationLifecycleServiceRegistrar : NSObject

@property (strong) id<SwApplicationLifecycle> service;

- (id)initWithService:(id<SwApplicationLifecycle>)service;
- (void)startService;
- (void)stopService;
- (void)registerBackgroundWatchdog:(id<SwBackgroundWatchdog>)watchdog;
- (void)unregisterBackgroundWatchdog:(id<SwBackgroundWatchdog>)watchdog;
- (void)unregisterAllWatchdogs;


@end
