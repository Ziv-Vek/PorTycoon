//
//  SwWisdomRequestExecutorTask.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 19/11/2020.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "SwWisdomRequest.h"
#import "SwConnectivityManager.h"

@interface SwWisdomRequestExecutorTask : NSObject <NSURLSessionDelegate, NSURLSessionTaskDelegate>

@property(assign) UIBackgroundTaskIdentifier bgTaskId;

- (id)initWithRequest:(SwWisdomRequest *)request andWithNetworkUtils:(SwConnectivityManager *)connectivity;
- (void)executeRequestAsync;
- (NSInteger)executeRequest;

@end
