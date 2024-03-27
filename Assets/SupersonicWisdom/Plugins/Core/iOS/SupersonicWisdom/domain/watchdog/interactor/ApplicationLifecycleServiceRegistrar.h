//
//  ApplicationLifecycleerviceRegistrar.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwApplicationLifecycle.h"
#import "../callback/BackgroundWatchdogDelegate.h"
#import "SwBackgroundWatchdogService.h"

@interface SwApplicationLifecycleServiceRegistrar : NSObject

@property (nonatomic, strong) id<SwApplicationLifecycle> service;

- (id)initWithService:(id<SwApplicationLifecycle>)service;
- (void)startService;
- (void)stopService;
- (void)registerBackgroundWatchdog:(SwBackgroundWatchdogService *) watchdog;
- (void)unregisterBackgroundWatchdog:(SwBackgroundWatchdogService *) watchdog;
- (void)unregisterAllWatchdogs;


@end
