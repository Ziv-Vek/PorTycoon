//
//  SwWisdomNetworkManager.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import "SwWisdomNetworkManager.h"
#import "SwWisdomRequest.h"
#import "SwConnectivityManager.h"

@implementation SwWisdomNetworkManager {
    SwWisdomNetworkDispatcher *dispatcher;
    SwWisdomRequestExecutorTask *currentTask;
    SwConnectivityManager *connectivity;
}

@synthesize connectTimeout, readTimeout;

- (id)initWithNetworkUtils:(SwConnectivityManager *)connectivityManager {
    if (!(self = [super init])) return nil;
    connectivity = connectivityManager;
    
    return self;
}

- (void)sendAsync:(NSString *)key
              url:(NSString *)url
         withBody:(NSData *)body
         callback:(OnNetworkResponse)callback {
    
    [self sendAsync:key url:url withBody:body connectTimeout:connectTimeout readTimeout:readTimeout callback:callback];
}

- (void)sendAsync:(NSString *)key
              url:(NSString *)url
         withBody:(NSData *)body
   connectTimeout:(NSTimeInterval)requestTimeout
      readTimeout:(NSTimeInterval)resourceTimeout
         callback:(OnNetworkResponse)callback {
    SwWisdomRequest *request = [[SwWisdomRequest alloc] initWithUrl:url method:POST body:body];
    [request addHeader:@"Content-Type" value:@"application/json"];
    [request setConnectTimeout:requestTimeout];
    [request setReadTimeout:resourceTimeout];
    [request setKey:key];

    if (callback) {
        [request responseCallback:callback];
    }
    SwWisdomRequestExecutorTask *task = [[SwWisdomRequestExecutorTask alloc] initWithRequest:request andWithNetworkUtils:connectivity];
    [SwWisdomNetworkDispatcher dispatch:task];
}

@end
