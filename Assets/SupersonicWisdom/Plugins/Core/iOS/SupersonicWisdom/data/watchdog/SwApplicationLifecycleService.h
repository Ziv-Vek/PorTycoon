//
//  ApplicationLifecycleService.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "SwBackgroundWatchdogService.h"
#import "SwApplicationLifecycle.h"

@interface SwApplicationLifecycleService : NSObject <SwApplicationLifecycle>

@property (strong) NSMutableArray *watchdogs;

- (id)init;

@end
