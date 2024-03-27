//
//  SwRemoteServer.m
//  SupersonicWisdom
//
//  Created by Omer Bentov on 17/11/2020.
//

#import "SwRequestManager.h"
#import "SwUtils.h"
#import "SwConnectivityManager.h"
#import "SwRemoteRequest.h"

@implementation SwRequestManager {
    id<SwWisdomNetwork> networkApi;
    NSMutableDictionary *waitingForNetwork;
    SwConnectivityManager *connectivity;
}

- (id)initWithNetwork:(id<SwWisdomNetwork>)network connectivityManager:(SwConnectivityManager *)connectivityManager{
    if (!(self = [super init])) return nil;
    networkApi = network;
    waitingForNetwork = [[NSMutableDictionary alloc] init];
    connectivity = connectivityManager;
    [connectivity registerConnectivityDelegate:self];
    
    return self;
}

- (void)sendRequest:(NSString *)request withResponseCallback:(OnSwResponse)responseListener{
    NSMutableDictionary *requestDict = [[SwUtils jsonStringToDict:request] mutableCopy];
    NSString *key = [requestDict valueForKey:SW_REQUEST_KEY];
    NSString *url = [requestDict valueForKey:SW_REQUEST_URL];
    NSString *body = [requestDict valueForKey:SW_REQUEST_BODY];
    NSTimeInterval connectionTimeout = [[requestDict valueForKey:SW_REQUEST_CONNECTION_TIMEOUT] integerValue] / 1000;
    NSTimeInterval readTimeout = [[requestDict valueForKey:SW_REQUEST_READ_TIMEOUT] integerValue] / 1000;
    NSData *bodyData = [SwUtils toJsonData:[SwUtils jsonStringToDict:body]];
    NSInteger cap = [[requestDict valueForKey:SW_REQUEST_CAP] integerValue];
    NSInteger iteration = [[requestDict valueForKey:SW_REQUEST_ITERATION] integerValue];
    
    if(iteration == 0){
        iteration = 1;
        [requestDict setValue:[NSNumber numberWithInteger:iteration] forKey:SW_REQUEST_ITERATION];
    }
    
    SwRemoteRequest *remoteRequest = [[SwRemoteRequest alloc] initWith:responseListener request:requestDict];
    
    if(![connectivity isNetworkAvailable]){
        [SWSDKLogger logMessage:@"SwRemoteServer | sendRequest | network is unavailable - caching request with key = %@", key];
        
        [waitingForNetwork setObject:remoteRequest forKey:key];
        [self notifyListener:key successfully:false responseCode:RESPONSE_CODE_NO_INTERNET responseData:[NSData alloc] cap:cap iteration:[[requestDict valueForKey:SW_REQUEST_ITERATION] integerValue] requestTime:0 listener:responseListener];
        
        return;
    }
    else{
        [SWSDKLogger logMessage:@"SwRemoteServer | sendRequest | network available - sending request with key = %@", key];
        
        NSDate *startTime = [NSDate date];
        [networkApi sendAsync:key url:url withBody:bodyData connectTimeout:connectionTimeout readTimeout:readTimeout callback:^(NSString *key, BOOL successfully, NSInteger responseCode, NSData *responseData){
            NSDate *endTime = [NSDate date];
            NSTimeInterval requestTime = [endTime timeIntervalSinceDate:startTime];
            
            [self notifyListener:key successfully:successfully responseCode:responseCode responseData:responseData cap:cap iteration:iteration requestTime:requestTime listener:responseListener];
            
            NSDictionary *mutRequestDict = [[remoteRequest getRequest] mutableCopy];
            NSInteger iteration = [[mutRequestDict valueForKey:SW_REQUEST_ITERATION] intValue];
            iteration = iteration + 1;
            [mutRequestDict setValue:[NSNumber numberWithInteger:iteration] forKey:SW_REQUEST_ITERATION];
            
            if(!successfully && iteration <= cap){
                [self resendRequest:mutRequestDict listener:responseListener];
            }
        }];
    }
}

-(void) notifyListener:(NSString *)key successfully:(BOOL)successfully responseCode:(NSInteger)responseCode responseData:(NSData *)responseData cap:(NSInteger)cap iteration:(NSInteger)iteration requestTime:(NSTimeInterval)requestTime listener:(OnSwResponse)listener{
    
    [SWSDKLogger logMessage:@"SwRemoteServer | onResponse | key=%@ | successfully=%d | responseCode=%d | cap=%d | iteration=%d | requestTime=%d",
     key, successfully, responseCode, cap, iteration, requestTime];
    
    NSMutableDictionary *responseDict = [self createBaseResponse:key successfully:successfully responseCode:responseCode responseData:responseData cap:cap iteration:iteration requestTime:requestTime];
    
    if(successfully){
        NSString* reponseJsonString = [[NSString alloc] initWithData:responseData encoding:NSASCIIStringEncoding];
        
        [responseDict setValue:reponseJsonString forKey:SW_REQUEST_DATA_STRING];
        [responseDict setValue:@"No error" forKey:SW_REQUEST_ERROR];
    }
    else{
        [responseDict setValue:@"Error" forKey:SW_REQUEST_ERROR];
    }
    
    NSString *responseString = [SwUtils toJsonString:responseDict];
    listener(responseString);
}

-(void) resendRequest:(NSDictionary *)requestDict listener:(OnSwResponse)listener{
    NSInteger iteration = [[requestDict valueForKey:SW_REQUEST_ITERATION] intValue];
    
    double delayInSeconds = pow(SW_REQUEST_WAITING_MULTIPLIER, iteration - 1);
    dispatch_time_t popTime = dispatch_time(DISPATCH_TIME_NOW, (int64_t)(delayInSeconds * NSEC_PER_SEC));

    dispatch_after(popTime, dispatch_get_main_queue(), ^(void){
        [self sendRequest:[SwUtils toJsonString:[[NSDictionary alloc] initWithDictionary:requestDict]] withResponseCallback:listener];
    });
}

-(NSMutableDictionary *) createBaseResponse:(NSString *)key  successfully:(BOOL)successfully responseCode:(NSInteger)responseCode responseData:(NSData *)responseData cap:(NSInteger)cap iteration:(NSInteger)iteration requestTime:(NSTimeInterval)requestTime{
    long requestTimeInMilliseconds = requestTime * 1000;
    
    NSMutableDictionary *requestDict = @
    {
        SW_REQUEST_KEY:key,
        SW_REQUEST_IS_DONE: @true,
        SW_REQUEST_IS_PENDING: @false,
        SW_REQUEST_CODE: @(responseCode),
        SW_REQUEST_ITERATION: [NSNumber numberWithInteger:iteration],
        SW_REQUEST_CAP: [NSNumber numberWithInteger:cap],
        SW_REQUEST_IS_REACHED_CAP: @(cap == iteration),
        SW_REQUEST_TIME: [NSNumber numberWithLong:requestTimeInMilliseconds],
    }.mutableCopy;
    
    return requestDict;
}

-(void) sendAllWaitingForNetwork{
    NSDictionary *networkListeners = [waitingForNetwork copy];
    for (id key in [networkListeners allKeys]) {
        id value = [networkListeners objectForKey:key];
        [waitingForNetwork removeObjectForKey: key];
        if (![value isKindOfClass:[SwRemoteRequest class]]) continue;
        
        SwRemoteRequest *request = (SwRemoteRequest *) value;
        [self sendRequest: [SwUtils toJsonString: [request getRequest]] withResponseCallback:[request getListener]];
    }
}

- (void)onConnectivityStatusChanged:(BOOL)isAvailable {
    if(isAvailable){
        [self sendAllWaitingForNetwork];
    }
}

@end
