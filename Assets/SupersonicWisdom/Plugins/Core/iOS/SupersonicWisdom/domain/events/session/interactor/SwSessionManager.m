//
//  SwSessionManager.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import "SwSessionManager.h"
#import "SwNetworkCallbacks.h"
#import "../../../../domain/utils/SwUtils.h"

#define SESSION_EVENT_NAME @"Session"
#define START_SESSION_EVENT @"StartSession"
#define END_SESSION_EVENT @"FinishSession"
#define MEGA_SESSION_COUNTER @"megaSessionCounter"
#define SESSION_IN_MEGA_COUNTER @"sessionInMegaCounter"
#define SESSION_COUNTER @"sessionCounter"
#define SW_PREFS_PREFIX @"sw_"
#define CUSTOM1 @"custom1"
#define CUSTOM2 @"custom2"

@implementation SwSessionManager {
    NSString *currentSessionId;
    NSTimeInterval sessionStartTime;
    NSTimeInterval sessionEndTime;
    NSNumber *sessionDurationTime;
    BOOL isSessionInitialized;
    NSInteger mMegaSessionsCounter;
    NSInteger mSessionsInMegaSessionCounter;
    NSInteger mTotalSessionsCounter;
    
    id<SwEventsRepositoryProtocol> eventRepository;
    id<SwEventMetadataManagement> eventMetadataManager;
    id<SwConversionDataManagement> eventConversionDataManager;
    id<SwEventsReporterProtocol> eventsReporter;
    NSMutableArray *sessionDelegates;
    id<SwEventsQueueProtocol> syncEventQueue;
    NSUserDefaults *mUserDefaults;
}

static NSString *megaSessionId;

+ (void)load { // This event occurs once, it will be fired again only after the app is killed.
    megaSessionId = [[NSUUID UUID] UUIDString];
}

- (id)initWithReporter:(id<SwEventsReporterProtocol>)repoter
            EventsRepo:(id<SwEventsRepositoryProtocol>)eventsRepo
       MetadataManager:(id<SwEventMetadataManagement>)metadataManager
 ConversionDataManager:(id<SwConversionDataManagement>)conversionDataManager
            EventQueue:(id<SwEventsQueueProtocol>)queue
           UserDefault:(NSUserDefaults *)prefs {
    if (!(self = [super init])) return nil;
    sessionDelegates = [NSMutableArray array];
    eventsReporter = repoter;
    eventRepository = eventsRepo;
    eventMetadataManager = metadataManager;
    eventConversionDataManager = conversionDataManager;
    syncEventQueue = queue;
    isSessionInitialized = NO;
    mUserDefaults = prefs;
    [self loadSessionData];
    mSessionsInMegaSessionCounter = 0;
    mMegaSessionsCounter++;
    [self saveSessionData];

    return self;
}

- (void)loadSessionData {
    mMegaSessionsCounter = [mUserDefaults integerForKey:[NSString stringWithFormat:@"%@%@", SW_PREFS_PREFIX, MEGA_SESSION_COUNTER]];
    mTotalSessionsCounter = [mUserDefaults integerForKey:[NSString stringWithFormat:@"%@%@", SW_PREFS_PREFIX, SESSION_COUNTER]];
}

- (void)saveSessionData {
    [mUserDefaults setInteger:mMegaSessionsCounter forKey:[NSString stringWithFormat:@"%@%@", SW_PREFS_PREFIX, MEGA_SESSION_COUNTER]];
    [mUserDefaults setInteger:mTotalSessionsCounter forKey:[NSString stringWithFormat:@"%@%@", SW_PREFS_PREFIX, SESSION_COUNTER]];
}

- (NSDictionary *)getData {
     return @{
                KEY_MEGA_SESSION_ID : megaSessionId,
                KEY_SESSION : currentSessionId,
                MEGA_SESSION_COUNTER : [NSNumber numberWithInteger:mMegaSessionsCounter],
                SESSION_IN_MEGA_COUNTER : [NSNumber numberWithInteger:mSessionsInMegaSessionCounter],
                SESSION_COUNTER : [NSNumber numberWithInteger:mTotalSessionsCounter],
            };
}

- (void)openSession {
    currentSessionId = [[NSUUID UUID] UUIDString];
    sessionStartTime = [[NSDate date] timeIntervalSince1970];
    mSessionsInMegaSessionCounter++;
    mTotalSessionsCounter++;
    
    [self saveSessionData];
}

- (void)closeSession {
    NSDate *endDate = [NSDate date];
    NSDate *startDate = [NSDate dateWithTimeIntervalSince1970:sessionStartTime];
    sessionEndTime = [endDate  timeIntervalSince1970];
    NSTimeInterval interval = [endDate timeIntervalSinceDate:startDate];
    sessionDurationTime = [NSNumber numberWithLong:interval];
}

- (void)resetSession {
    currentSessionId = @"";
    sessionDurationTime = 0;
    sessionStartTime = 0;
    sessionEndTime = 0;
}

- (BOOL)isSessionStarted {
    return sessionStartTime != 0;
}

- (void)startSession {
    [syncEventQueue startQueue];
    [self openSession];
    
    NSDictionary *additionalDataJson = [self getAdditionalDataJson];
    NSDictionary *customs = [self createCustomsDictionray:START_SESSION_EVENT duration:@"0"];
    
    if (additionalDataJson != nil) {
        customs = [[SwUtils mergeDictionaries:customs other:additionalDataJson] mutableCopy];
    }
    
    NSDictionary *event = [SwUtils createEvent:SESSION_EVENT_NAME sessionData:[self getData] conversionData:[eventConversionDataManager conversionData] metdadata:[[eventMetadataManager get] get] customs:[SwUtils toJsonString:customs] extra:@"{}"];
    
    [eventsReporter reportEvent:event];
    [self onSessionStarted:currentSessionId];
}

- (void)endSession {
    [self closeSession];
    NSDictionary *additionalDataJson = [self getAdditionalDataJson];
    NSString *duration = [sessionDurationTime stringValue];
    NSDictionary *customs = [self createCustomsDictionray:END_SESSION_EVENT duration:duration];
    
    if (additionalDataJson != nil) {
        customs = [[SwUtils mergeDictionaries:customs other:additionalDataJson] mutableCopy];
    }
    
    NSDictionary *event = [SwUtils createEvent:SESSION_EVENT_NAME sessionData:[self getData] conversionData:[eventConversionDataManager conversionData] metdadata:[[eventMetadataManager get] get] customs:[SwUtils toJsonString:customs] extra:@"{}"];
    
    [eventsReporter reportEvent:event];
    [self onSessionEnded:currentSessionId];
    [self resetSession];
    [syncEventQueue stopQueue];
}

- (NSDictionary *)createCustomsDictionray:(NSString *)eventName duration:(NSString *)duration{
    NSDictionary *customs =@{ CUSTOM1     : eventName,
                              CUSTOM2    : duration,
                              MEGA_SESSION_COUNTER : [NSNumber numberWithInteger:mMegaSessionsCounter],
                              SESSION_IN_MEGA_COUNTER : [NSNumber numberWithInteger:mSessionsInMegaSessionCounter],
                              SESSION_COUNTER : [NSNumber numberWithInteger:mTotalSessionsCounter],
                            };
    
    return customs;
}

- (void)initializeSessionWith:(SwEventMetadataDto *)metadata {
    isSessionInitialized = YES;
    [eventMetadataManager set:metadata];
    [self startSession];
}

- (void)registerSessionDelegate:(id<SwSessionDelegate>)delegate {
    [sessionDelegates addObject:delegate];
}

- (void)unregisterSessionDelegate:(id<SwSessionDelegate>)delegate {
    [sessionDelegates removeObject:delegate];
}

- (void)unregisterAllSessionDelegates {
    [sessionDelegates removeAllObjects];
}

- (void)onSessionStarted:(NSString *)sessionId {
    for (id<SwSessionDelegate> delegate in sessionDelegates) {
        if (delegate != nil && [delegate respondsToSelector:@selector(onSessionStarted:)]) {
            [delegate onSessionStarted:sessionId];
        }
    }
}

- (void)onSessionEnded:(NSString *)sessionId {
    for (id<SwSessionDelegate> delegate in sessionDelegates) {
        if (delegate != nil && [delegate respondsToSelector:@selector(onSessionEnded:)]) {
            [delegate onSessionEnded:sessionId];
        }
    }
}

- (void)onAppMovedToForeground {
    if (isSessionInitialized && ![self isSessionStarted]) {
        [self startSession];
    }
}

- (void)onAppMovedToBackground {
    if (isSessionInitialized && [self isSessionStarted]) {
        [self endSession];
    }
}

- (NSString *)getMegaSessionId {
    return megaSessionId;
}

- (NSDictionary *)getAdditionalDataJson{
    NSString *additionalDataString = @"";
    
    for (id<SwSessionDelegate> delegate in sessionDelegates) {
        if (delegate != nil && [delegate respondsToSelector:@selector(getAdditionalDataJsonMethod)]){
            additionalDataString = [delegate getAdditionalDataJsonMethod];
        }
    }
    
    NSDictionary *additionalDataJson;
    NSData *data = [additionalDataString dataUsingEncoding:NSUTF8StringEncoding];
    id json = [NSJSONSerialization JSONObjectWithData:data options:0 error:nil];
    
    if ([json isKindOfClass:[NSDictionary class]]) {
        additionalDataJson = json;
    }
    
    return additionalDataJson;
}

@end
