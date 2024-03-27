//
//  SwWisdomRequestExecutorTask.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 19/11/2020.
//

#define STATUS_CODE_NO_INTERNET -6
#import "SwWisdomRequestExecutorTask.h"
#import "SwConnectivityManager.h"

@implementation SwWisdomRequestExecutorTask {
    SwWisdomRequest *wisdomRequest;
    NSURLSessionConfiguration *sessionConfig;
    NSURLSession *session;
    NSString *requestId;
    NSString *cachedRequestBody;
    NSData *responseData;
    SwConnectivityManager *connectivity;
}

@synthesize bgTaskId;

- (id)initWithRequest:(SwWisdomRequest *)request andWithNetworkUtils:(SwConnectivityManager *)connectivityManager {
    if (!(self = [super init])) return nil;
    bgTaskId = UIBackgroundTaskInvalid;
    wisdomRequest = request;
    requestId = [[NSUUID UUID] UUIDString];
    sessionConfig = [NSURLSessionConfiguration backgroundSessionConfigurationWithIdentifier:requestId];
    [sessionConfig setTimeoutIntervalForRequest:[request getConnectTimeout]];
    [sessionConfig setTimeoutIntervalForResource:[request getReadTimeout]];
    session = [NSURLSession sessionWithConfiguration:sessionConfig delegate:self delegateQueue:nil];
    connectivity = connectivityManager;
    
    return self;
}

- (void)executeRequestAsync {
    
    if (![connectivity isNetworkAvailable]) {
        [wisdomRequest onResponseFailedWithError:@"NO_INTERNET" statusCode:STATUS_CODE_NO_INTERNET response:nil];
        return;
    }
    
    if (wisdomRequest.body) {
        [self cacheRequestBody];
    }
    NSMutableURLRequest *urlRequest = [[NSMutableURLRequest alloc] initWithURL:[wisdomRequest url]];
    HttpMethod method = [wisdomRequest httpMethod];
    [urlRequest setHTTPMethod:[self httpMethod:method]];
    [urlRequest setAllHTTPHeaderFields:[wisdomRequest headers]];
    [urlRequest setTimeoutInterval:[wisdomRequest getConnectTimeout]];
    NSURLSessionTask *task;
    if (wisdomRequest.body) {
        task = [session uploadTaskWithRequest:urlRequest fromFile:[NSURL fileURLWithPath:cachedRequestBody]];
    } else {
        task = [session dataTaskWithRequest:urlRequest];
    }
    
    
    self.bgTaskId = [[UIApplication sharedApplication] beginBackgroundTaskWithExpirationHandler:^{
        [task cancel];
     }];
    [task resume];
}

- (NSInteger)executeRequest {
    if (![connectivity isNetworkAvailable]) {
        [wisdomRequest onResponseFailedWithError:@"NO_INTERNET" statusCode:STATUS_CODE_NO_INTERNET response:nil];
        return STATUS_CODE_NO_INTERNET;
    }
    [self cacheRequestBody];
    NSMutableURLRequest *urlRequest = [[NSMutableURLRequest alloc] initWithURL:[wisdomRequest url]];
    HttpMethod method = [wisdomRequest httpMethod];
    [urlRequest setHTTPMethod:[self httpMethod:method]];
    [urlRequest setAllHTTPHeaderFields:[wisdomRequest headers]];
    [urlRequest setTimeoutInterval:[wisdomRequest getConnectTimeout]];
    
    NSURLSessionUploadTask *task = [session uploadTaskWithRequest:urlRequest fromFile:[NSURL fileURLWithPath:cachedRequestBody]];
    [task resume];
    if ([[task response] isKindOfClass:[NSHTTPURLResponse class]]) {
        NSHTTPURLResponse *httpResponse = (NSHTTPURLResponse *)[task response];
        return httpResponse.statusCode;
    }
    
    return  -220;
}

- (void)cacheRequestBody {
    NSArray *paths = NSSearchPathForDirectoriesInDomains(NSCachesDirectory, NSUserDomainMask, YES);
    NSString *documentsDirectory = [paths objectAtIndex:0];
    cachedRequestBody = [documentsDirectory stringByAppendingPathComponent:requestId];
    [[wisdomRequest body] writeToFile:cachedRequestBody atomically:YES];
}

- (void)removeCachedRequestBody {
    if (cachedRequestBody) {
        NSError *error;
        [[NSFileManager defaultManager] removeItemAtPath:cachedRequestBody error:&error];
        //TODO add debug logger in future
    }
}

- (NSString *)httpMethod:(HttpMethod)method {
    switch (method) {
        case POST:
            return @"POST";
            
        case GET:
        default:
            return @"GET";
    }
}

- (void)URLSession:(NSURLSession *)session task:(NSURLSessionTask *)task didCompleteWithError:(NSError *)error {
    if(error) {
        [wisdomRequest onResponseFailedWithError:[error description] statusCode:error.code response:responseData];
    }
    
    if ([[task response] isKindOfClass:[NSHTTPURLResponse class]]) {
        NSHTTPURLResponse *httpResponse = (NSHTTPURLResponse *)[task response];
        if (httpResponse.statusCode >= 200 && httpResponse.statusCode <= 299) {
            [wisdomRequest onResponseSucceededWithStatusCode:httpResponse.statusCode response:responseData];
        } else {
            [wisdomRequest onResponseFailedWithError:@"HTTP_ERROR" statusCode:httpResponse.statusCode response:responseData];
        }
    }
    
    [self removeCachedRequestBody];
    [[UIApplication sharedApplication] endBackgroundTask:bgTaskId];
    [session finishTasksAndInvalidate];
}

- (void)URLSession:(NSURLSession *)session dataTask:(NSURLSessionDataTask *)dataTask didReceiveData:(NSData *)data {
    responseData = data;
}

@end
