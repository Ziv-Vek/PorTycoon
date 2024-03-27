//
//  SwWisdomNetworkDispatcher.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 19/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwWisdomRequestExecutorTask.h"

NS_ASSUME_NONNULL_BEGIN

@interface SwWisdomNetworkDispatcher : NSObject

+ (void)dispatch:(SwWisdomRequestExecutorTask *)task;
+ (NSInteger)executeRequest:(SwWisdomRequestExecutorTask *)task;

@end

NS_ASSUME_NONNULL_END
