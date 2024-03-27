//
//  WisdomSDK.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import "SwWisdomSDK.h"
#import "SWSDKLogger.h"

__strong SwSDK *sharedInstance;

typedef struct _SwWisdomConfigurationStruct {
    bool isLoggingEnabled;
    const char* subdomain;
    int connectTimeout;
    int readTimeout;
    int initialSyncInterval;
    const char * streamingAssetsFolderPath;
    const char * blockingLoaderResourceRelativePath;
    int blockingLoaderViewportPercentage;
} SwWisdomConfigurationStruct;


extern "C" {
    typedef void (*OnSessionStarted)(const char*);
    typedef void (*OnSessionEnded)(const char*);
    typedef char* (*GetAdditionalDataJsonMethod)();
    typedef void (*OnWebResponse)(const char*);
    typedef void (*OnConnectivityStatusChanged)(const char*);
}

static OnSessionStarted onSessionStartedCallback;
static OnSessionEnded onSessionEndedCallback;
static GetAdditionalDataJsonMethod getAdditionalDataJsonMethod;
static OnWebResponse onWebResponseCallback;
static OnConnectivityStatusChanged onConnectivityStatusChangedCallback;

extern "C" {
    
    void initSdk(SwWisdomConfigurationStruct configuration) {
        SwWisdomConfigurationDto *config = [[SwWisdomConfigurationDto alloc] init];

        [config setIsLoggingEnabled:configuration.isLoggingEnabled];
        [config setSubdomain:[NSString stringWithUTF8String:configuration.subdomain]];
        [config setConnectTimeout:configuration.connectTimeout];
        [config setReadTimeout:configuration.readTimeout];
        [config setInitialSyncInterval:configuration.initialSyncInterval];
        [config setStreamingAssetsFolderPath:[NSString stringWithUTF8String: configuration.streamingAssetsFolderPath]];
        [config setBlockingLoaderResourceRelativePath:[NSString stringWithUTF8String: configuration.blockingLoaderResourceRelativePath]];
        [config setBlockingLoaderViewportPercentage: configuration.blockingLoaderViewportPercentage];

        [SwWisdomSDK initSdkWithConfig:config];
    }
    
    void toggleBlockingLoader(BOOL shouldPresent) {
        [SwWisdomSDK toggleBlockingLoader: shouldPresent];
    }
    
    void updateWisdomConfiguration(SwWisdomConfigurationStruct configuration) {
        SwWisdomConfigurationDto *config = [[SwWisdomConfigurationDto alloc] init];
        [config setSubdomain:[NSString stringWithUTF8String:configuration.subdomain]];
        [config setConnectTimeout:configuration.connectTimeout];
        [config setReadTimeout:configuration.readTimeout];
        [config setInitialSyncInterval:configuration.initialSyncInterval];
        
        [SwWisdomSDK updateWisdomConfiguration:config];
    }

    bool isInitialized() {
        return [SwWisdomSDK isInitialized];
    }

    void initializeSession(const char* metadataJson) {
        NSString *metadataJsonStr;
        if (metadataJson)                 metadataJsonStr = [NSString stringWithUTF8String:metadataJson];
        [SwWisdomSDK initializeSession:metadataJsonStr];
    }

    void setEventMetadata(const char* metadataJson) {
        NSString *metadataJsonStr;
        if (metadataJson)                 metadataJsonStr = [NSString stringWithUTF8String:metadataJson];
        [SwWisdomSDK initializeSession:metadataJsonStr];
    }

    void updateEventMetadata(const char* metadataJson) {
        NSString *metadataJsonStr;
        if (metadataJson)                 metadataJsonStr = [NSString stringWithUTF8String:metadataJson];
        [SwWisdomSDK updateEventMetadata:metadataJsonStr];
    }
    
    void trackEvent(const char* eventName, const char* customsJson, const char* extraJson) {
        NSString *eventNameStr;
        NSString *customsJsonStr;
        NSString *extraJsonStr;
        if (eventName)                 eventNameStr = [NSString stringWithUTF8String:eventName];
        if (customsJson)                   customsJsonStr = [NSString stringWithUTF8String:customsJson];
        if (extraJson)                   extraJsonStr = [NSString stringWithUTF8String:extraJson];
        
        [SwWisdomSDK trackEvent:eventNameStr customsJson:customsJsonStr extraJson:extraJsonStr];
    }
    
    void registerSessionStartedCallback(OnSessionStarted callback) {
        onSessionStartedCallback = callback;
    }
    
    void registerSessionEndedCallback(OnSessionEnded callback) {
        onSessionEndedCallback = callback;
    }

    void registerGetAdditionalDataJsonMethod(GetAdditionalDataJsonMethod method) {
        getAdditionalDataJsonMethod = method;
    }
    
    void registerWebRequestCallback(OnWebResponse callback){
        onWebResponseCallback = callback;
    }

    void registerConnectivityListener(OnConnectivityStatusChanged callback){
        onConnectivityStatusChangedCallback = callback;
    }

    void unregisterSessionStartedCallback() {
        onSessionStartedCallback = NULL;
    }
    
    void unregisterSessionEndedCallback() {
        onSessionEndedCallback = NULL;
    }

    void unregisterGetAdditionalDataJsonMethod() {
        getAdditionalDataJsonMethod = NULL;
    }

    void unregisterWebRequestCallback(OnWebResponse callback){
        onWebResponseCallback = NULL;
    }

    void unregisterConnectivityListener(OnConnectivityStatusChanged callback){
        onConnectivityStatusChangedCallback = NULL;
    }

    void sendRequest(const char* requestJson){
        NSString *requesttJsonStr;
        if (requestJson)                 requesttJsonStr = [NSString stringWithUTF8String:requestJson];
        
        [SwWisdomSDK sendRequest:requesttJsonStr];
    }

    char* getConnectionStatus(){
        NSString *connectionStatus = [SwWisdomSDK getConnectionStatus];
        const char* connectionStatusString = [connectionStatus UTF8String];
        if (connectionStatusString == NULL)
            return NULL;

        char* res = (char*)malloc(strlen(connectionStatusString) + 1);
        strcpy(res, connectionStatusString);

        return res;
    }

    void destroy() {
        [SwWisdomSDK destroy];
    }

    char* getMegaSessionId(){
        NSString *megaSessionId = [SwWisdomSDK getMegaSessionId];
        const char* megaSessionIdString = [megaSessionId UTF8String];
    
        char* res = (char*)malloc(strlen(megaSessionIdString) + 1);
        strcpy(res, megaSessionIdString);
        
        return res;
    }
}

#import "SwUtils.h"
@implementation SwWisdomSDK

+ (void)load {
    sharedInstance = [[SwSDK alloc] init];
}

+ (void)initSdkWithConfig:(SwWisdomConfigurationDto *)configuration {
    [sharedInstance initSdkWithConfig:configuration];
    [sharedInstance registerSessionDelegate:[SwWisdomSDK self]];
    [sharedInstance registerConnectivityDelegate:[SwWisdomSDK self]];
}

+ (BOOL)toggleBlockingLoader:(BOOL)shouldPresent {
    if (isInitialized()) {
        return [sharedInstance toggleBlockingLoader: shouldPresent];
    }
    
    return NO;
}

+ (void)updateWisdomConfiguration:(SwWisdomConfigurationDto *)configuration {
    [sharedInstance updateWisdomConfiguration:configuration];
}

+ (BOOL)isInitialized {
    return [sharedInstance isInitialized];
}

+ (void)initializeSession:(NSString *)metadataJson {
    if (isInitialized()) {
        SwEventMetadataDto *data = [[SwEventMetadataDto alloc] initFromString:metadataJson];
        NSString *metaDataStr = [data get];
        [sharedInstance initializeSession:metaDataStr];
    }
}
 
+ (void)setEventMetadata:(NSString *)metadataJson {
    if (isInitialized()) {
        SwEventMetadataDto *data = [[SwEventMetadataDto alloc] initFromString:metadataJson];
        [sharedInstance setEventMetadata:[data get]];
    }
}

+ (void)updateEventMetadata:(NSString *)metadataJson {
    if (isInitialized()) {
        SwEventMetadataDto *data = [[SwEventMetadataDto alloc] initFromString:metadataJson];
        [sharedInstance updateEventMetadata:[data get]];
    }
}

+ (void)trackEvent:(NSString *)eventName customsJson:(NSString *)customsJson extraJson:(NSString *)extraJson{
    if (isInitialized()) {
        [sharedInstance trackEvent:eventName customsJson:customsJson extraJson:extraJson];
    }
}

+ (void)destroy {
    if (isInitialized()) {
        [sharedInstance unregisterSessionDelegate:[SwWisdomSDK self]];
        [sharedInstance destroy];
    }
}

+ (void)onSessionStarted:(NSString *)sessionId {
    if (onSessionStartedCallback) {
        onSessionStartedCallback([sessionId cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

+ (void)onSessionEnded:(NSString *)sessionId {
    if (onSessionEndedCallback) {
        onSessionEndedCallback([sessionId cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

+ (NSString *)getAdditionalDataJsonMethod {
    if (getAdditionalDataJsonMethod) {
        char* data = getAdditionalDataJsonMethod();
        return [NSString stringWithUTF8String:data];
    }
    
    return @"";
}

+ (void)onConnectivityStatusChanged:(BOOL)isAvailable {
    if (onConnectivityStatusChangedCallback) {
        onConnectivityStatusChangedCallback([[sharedInstance getConnectionStatus] cStringUsingEncoding:NSUTF8StringEncoding]);
    }
}

+ (void)sendRequest:(NSString *)requestJsonString {
    if (isInitialized()) {
        [sharedInstance sendRequest:requestJsonString withResponseCallback:^(NSString *response) {
            if(onWebResponseCallback){
                onWebResponseCallback([response cStringUsingEncoding:NSUTF8StringEncoding]);
            }
        }];
    }
}

+ (NSString *)getConnectionStatus {
    return [sharedInstance getConnectionStatus];
}

+ (NSString *)getMegaSessionId {
    if(isInitialized()){
        return [sharedInstance getMegaSessionId];
    }
    return @"";
}

@end
