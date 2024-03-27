//
//  SupersonicWisdom.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import "SwSDK.h"
#import "SwSessionManager.h"
#import "SwConstants.h"
#import "SwUtils.h"
#import "SwSessionManagement.h"
#import "SwEventsRepositoryProtocol.h"
#import "SwEventMetadataRepository.h"
#import "SwEventMetadataManagement.h"
#import "SwEventMetadataManager.h"
#import "SwConversionDataManagement.h"
#import "SwConversionDataManager.h"
#import "SwConversionDataRepositoryProtocol.h"
#import "SwConversionDataRepository.h"
#import "SwWisdomNetworkManager.h"
#import "SwListStoredEventJsonMapper.h"
#import "SwWisdomEventsStorage.h"
#import "SwEventsReporter.h"
#import "SwEventsQueue.h"
#import "SdkVersion.h"
#import "../data/framework/remote/SwRequestManager.h"
#import "../domain/events/dto/SwEventMetadataDto.h"

#import "SWSDKLogger.h"
#import "SwFullScreenLoader.h"

@implementation SwSDK {
    //App Lifecycle
    SwApplicationLifecycleService *appLifecycleService;
    SwApplicationLifecycleServiceRegistrar *appLifecycleRegistrar;
    
    SwFullScreenLoader *blockingLoader;
    
    //Background watchdog
    SwBackgroundWatchdogService *bgWatchdog;
    SwBackgroundWatchdogRegistrar *bgWatchdogRegistrar;
    
    //Events Repository
    id<SwEventsRepositoryProtocol> eventsRepository;
    SwEventsRemoteApi *eventsRemoteApi;
    SwEventsLocalApi *eventsLocalApi;
    SwEventsRemoteDataSource *eventsRemoteDataSrc;
    SwEventsLocalDataSource *eventsLocalDataSrc;
    
    //Metadata
    id<SwEventMetadataRepositoryProtocol> metadataRepository;
    SwEventMetadataLocalApi *metadataLocalApi;
    SwEventMetadataLocalDataSource *metadataLocalDataSource;
    id<SwEventMetadataManagement> metadataManger;
    SwConnectivityManager *connectivityManager;
    
    //Conversion Data
    id<SwConversionDataRepositoryProtocol> conversionDataRepository;
    SwConversionDataLocalApi *conversionDataApi;
    SwConversionDataLocalDataSource *conversionDataDataSource;
    id<SwConversionDataManagement> conversionDataManager;
    
    id<SwEventsReporterProtocol> eventsReporter;
    id<SwEventsQueueProtocol> syncEventsQueue;
    id<SwSessionManagement> sessionManager;
    SwRequestManager *requestManager;
    
    BOOL isInitialized;
    NSMutableArray *delegates;
    
    id<SwWisdomNetwork>network;
}

-(BOOL) toggleBlockingLoader:(BOOL)shouldPresent {
    if (shouldPresent) {
        return [blockingLoader show];
    } else {
        return [blockingLoader hide];
    }
}

- (void)initSdkWithConfig:(SwWisdomConfigurationDto *)configuration {
    bgWatchdog = [[SwBackgroundWatchdogService alloc] init];
    bgWatchdogRegistrar = [[SwBackgroundWatchdogRegistrar alloc] initWithWatchdog:bgWatchdog];

    NSString *framesLocationPath = [[configuration streamingAssetsFolderPath] stringByAppendingPathComponent: [configuration blockingLoaderResourceRelativePath]];
    blockingLoader = [[SwFullScreenLoader alloc] initWithFramesFolderPath: framesLocationPath withPercentageFromScreenWidth: [configuration blockingLoaderViewportPercentage]];
    
    appLifecycleService = [[SwApplicationLifecycleService alloc] init];
    appLifecycleRegistrar = [[SwApplicationLifecycleServiceRegistrar alloc] initWithService: appLifecycleService];
    [appLifecycleRegistrar registerBackgroundWatchdog:bgWatchdog];
    [appLifecycleRegistrar startService];
    
    SwListStoredEventJsonMapper *storedEventListMapper = [SwListStoredEventJsonMapper alloc];
    
    connectivityManager = [SwConnectivityManager internetConnectivity];
    
    network = [[SwWisdomNetworkManager alloc] initWithNetworkUtils:connectivityManager];
    [network setConnectTimeout:configuration.connectTimeout];
    [network setReadTimeout:configuration.readTimeout];
    
    eventsRemoteApi = [[SwEventsRemoteApi alloc] initWithNetwork:network
                                                       subdomain:configuration.subdomain
                                              storedEventsMapper:storedEventListMapper];
    
    eventsRemoteDataSrc = [[SwEventsRemoteDataSource alloc] initWithApi:eventsRemoteApi];
    
    SwWisdomDBManager *dbHelper = [[SwWisdomDBManager alloc] initDatabase];
    SwStoredEventDictMapper *storedEventDictMapper = [SwStoredEventDictMapper alloc];
    
    id<SwWisdomStorageProtocol> storage = [[SwWisdomEventsStorage alloc] initWithDatabase:dbHelper];
    eventsLocalApi = [[SwEventsLocalApi alloc] initWithStorage:storage
                                            storedEventsMapper:storedEventDictMapper];
    eventsLocalDataSrc = [[SwEventsLocalDataSource alloc] initWithLocalApi:eventsLocalApi];
    
    eventsRepository = [[SwEventsRepository alloc] initWithRemoteDataSource:eventsRemoteDataSrc localDataSource:eventsLocalDataSrc];
    
    NSUserDefaults *prefs = [NSUserDefaults standardUserDefaults];
    metadataLocalApi = [[SwEventMetadataLocalApi alloc] initWithLocalStorage:prefs];
    metadataLocalDataSource = [[SwEventMetadataLocalDataSource alloc] initWithApi:metadataLocalApi];
    metadataRepository = [[SwEventMetadataRepository alloc] initWith:metadataLocalDataSource];
    metadataManger = [[SwEventMetadataManager alloc] initWithRepository:metadataRepository];
    
    conversionDataApi = [[SwConversionDataLocalApi alloc] initWithLocalStorage:prefs];
    conversionDataDataSource = [[SwConversionDataLocalDataSource alloc] initWithApi:conversionDataApi];
    conversionDataRepository = [[SwConversionDataRepository alloc] initWith:conversionDataDataSource];
    conversionDataManager = [[SwConversionDataManager alloc] initWith:conversionDataRepository];
    
    eventsReporter = [[SwEventsReporter alloc] initWithRepository:eventsRepository];
    syncEventsQueue = [[SwEventsQueue alloc] initWith:eventsRepository withMapper:storedEventDictMapper andInterval:configuration.initialSyncInterval];
    sessionManager = [[SwSessionManager alloc] initWithReporter:eventsReporter
                                                      EventsRepo:eventsRepository
                                                 MetadataManager:metadataManger
                                           ConversionDataManager:conversionDataManager
                                                      EventQueue:syncEventsQueue
                                                     UserDefault:prefs];
    
    requestManager = [[SwRequestManager alloc] initWithNetwork:network connectivityManager:connectivityManager];
    delegates = [NSMutableArray array];
    isInitialized = YES;
    [syncEventsQueue startQueue];
}

- (void)updateWisdomConfiguration:(SwWisdomConfigurationDto *)configuration {
    [SWSDKLogger setIsEnabled:configuration.isLoggingEnabled];
    [network setConnectTimeout:configuration.connectTimeout];
    [network setReadTimeout:configuration.readTimeout];
    [syncEventsQueue updateSyncInterval:configuration.initialSyncInterval];
}

- (BOOL)isInitialized {
    return isInitialized;
}

- (void)initializeSession:(NSString *)metadataJson {
    SwEventMetadataDto *metadata = [[SwEventMetadataDto alloc] initFromString:metadataJson];
    [sessionManager initializeSessionWith:metadata];
    [bgWatchdogRegistrar registerWatchdogDelegate:sessionManager];
}

- (void)registerSessionDelegate:(id<SwWisdomSessionDelegate>)delegate {
    [sessionManager registerSessionDelegate:delegate];
}

- (void)unregisterSessionDelegate:(id<SwWisdomSessionDelegate>)delegate {
    [sessionManager unregisterSessionDelegate:delegate];
}

- (void)registerConnectivityDelegate:(id<SwConnectivityStatusCallback>)delegate {
    [connectivityManager registerConnectivityDelegate:delegate];
}

- (void)unregisterConnectivityDelegate:(id<SwConnectivityStatusCallback>)delegate {
    [connectivityManager unregisterConnectivityDelegate:delegate];
}

- (void)setEventMetadata:(NSString *)metadataJson {
    SwEventMetadataDto *metadata = [[SwEventMetadataDto alloc] initFromString:metadataJson];
    [metadataManger set:metadata];
}

- (void)updateEventMetadata:(NSString *)metadataJson {
    SwEventMetadataDto *metadata = [[SwEventMetadataDto alloc] initFromString:metadataJson];
    [metadataManger update:metadata];
}

- (void)trackEvent:(NSString *)eventName customsJson:(NSString *)customsJson extraJson:(NSString *)extraJson{
    NSDictionary *event =
    [SwUtils
     createEvent:eventName
     sessionData:[sessionManager getData]
     conversionData:[conversionDataManager conversionData]
     metdadata:[[metadataManger get] get]
     customs:customsJson
     extra:extraJson];
    
    [eventsReporter reportEvent:event];
}

- (void)sendRequest:(NSString *)requestJsonString withResponseCallback:(OnSwResponse)callback{
    [requestManager sendRequest:requestJsonString withResponseCallback:callback];
}

- (NSString *)getConnectionStatus {
    NSDictionary *status = @{
        @"isAvailable": @([connectivityManager isNetworkAvailable])
    };
    
    return [SwUtils toJsonString:status];
}

- (NSString *)getMegaSessionId {
    return [sessionManager getMegaSessionId];
}

- (NSString *)getVersion {    
    return [NSString stringWithFormat:@"%@-%@", VERSION, COMMIT_SHA];
}

- (void)destroy {
    isInitialized = NO;
    
    [appLifecycleRegistrar stopService];
    [appLifecycleRegistrar unregisterAllWatchdogs];
    [bgWatchdogRegistrar unregisterAllDelegates];
    [sessionManager unregisterAllSessionDelegates];
    
    [syncEventsQueue stopQueue];
    [sessionManager unregisterAllSessionDelegates];
    
    appLifecycleRegistrar = nil;
    bgWatchdogRegistrar = nil;
    syncEventsQueue = nil;
    sessionManager = nil;
    
    conversionDataApi = nil;
    conversionDataDataSource = nil;
    conversionDataRepository = nil;
    conversionDataManager = nil;
    
    metadataLocalApi = nil;
    metadataLocalDataSource = nil;
    metadataRepository = nil;
    metadataManger = nil;
    
    eventsRemoteApi = nil;
    eventsRemoteDataSrc = nil;
    eventsRepository = nil;
}

- (void)onSessionStarted:(NSString *)sessionId {
    for (id sessionDelegate in delegates) {
        if (sessionDelegate != nil && [sessionDelegate respondsToSelector:@selector(onSessionStarted:)]) {
            [sessionDelegate onSessionStarted:sessionId];
        }
    }
}

- (void)onSessionEnded:(NSString *)sessionId {
    for (id sessionDelegate in delegates) {
        if (sessionDelegate != nil && [sessionDelegate respondsToSelector:@selector(onSessionEnded:)]) {
            [sessionDelegate onSessionEnded:sessionId];
        }
    }
}

- (NSString *)getAdditionalDataJsonMethod {
    for (id sessionDelegate in delegates) {
        if (sessionDelegate != nil && [sessionDelegate respondsToSelector:@selector(getAdditionalDataJsonMethod)]) {
            return [sessionDelegate getAdditionalDataJsonMethod];
        }
    }
    
    return @"";
}

@end
