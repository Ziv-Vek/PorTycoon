//
//  SwWisdomRequest.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 19/11/2020.
//

#import "SwWisdomRequest.h"

#define DEFAULT_TIMEOUT 1.0

@implementation SwWisdomRequest {
    NSURL *requestUrl;
    HttpMethod requestMethod;
    NSData *requestBody;
    NSMutableDictionary *requestHeaders;
    id<SwWisdomResponseDelegate>requestResponseDelegate;
    OnNetworkResponse responseCallback;
    NSTimeInterval connectTimeout;
    NSTimeInterval readTimeout;
    NSString *requestKey;
}

- (id)initWithUrl:(NSString *)url method:(HttpMethod)method body:(NSData *)body {
    if (!(self = [super init])) return nil;
    requestUrl = [NSURL URLWithString:url];
    requestMethod = method;
    requestBody = body;
    connectTimeout  = DEFAULT_TIMEOUT;
    readTimeout  = DEFAULT_TIMEOUT;
    requestHeaders = [NSMutableDictionary dictionary];
    
    return self;
}

- (void)addHeader:(NSString *)name value:(NSString *)value {
    requestHeaders[name] = value;
}

- (void)responseCallback:(OnNetworkResponse)callback {
    responseCallback = callback;
}

- (NSDictionary *)headers {
    return requestHeaders;
}

- (NSURL *)url {
    return requestUrl;
}

- (HttpMethod)httpMethod {
    return requestMethod;
}

- (NSData *)body {
    return requestBody;
}

- (void)setConnectTimeout:(NSTimeInterval)timeout {
    connectTimeout = timeout;
}

- (NSTimeInterval)getConnectTimeout {
    return connectTimeout;
}

- (void)setReadTimeout:(NSTimeInterval)timeout {
    readTimeout = timeout;
}

- (NSTimeInterval)getReadTimeout {
    return readTimeout;
}

- (void)setKey:(NSString *)key {
    requestKey = key;
}

- (NSString *)getKey {
    return requestKey;
}

- (void)onResponseFailedWithError:(NSString *)error statusCode:(NSInteger)code response:(NSData *)data {
    if (responseCallback) {
        responseCallback(requestKey, NO, code, data);
    }
}

- (void)onResponseSucceededWithStatusCode:(NSInteger)code response:(NSData *)data {
    if (responseCallback) {
        responseCallback(requestKey, YES, code, data);
    }
}

@end
