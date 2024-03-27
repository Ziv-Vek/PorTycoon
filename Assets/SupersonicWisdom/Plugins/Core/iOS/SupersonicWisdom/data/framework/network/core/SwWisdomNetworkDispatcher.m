//
//  SwWisdomNetworkDispatcher.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 19/11/2020.
//

#import "SwWisdomNetworkDispatcher.h"

@implementation SwWisdomNetworkDispatcher

+ (void)dispatch:(SwWisdomRequestExecutorTask *)task {
    [task executeRequestAsync];
}

+ (NSInteger)executeRequest:(SwWisdomRequestExecutorTask *)task {
    return [task executeRequest];
}

@end
