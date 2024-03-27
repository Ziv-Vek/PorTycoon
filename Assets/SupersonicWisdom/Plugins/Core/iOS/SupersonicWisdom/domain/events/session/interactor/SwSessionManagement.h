//
//  SwSessionManagement.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

@protocol SwSessionManagement <NSObject, BackgroundWatchdogDelegate>

@required
- (void)initializeSessionWith:(SwEventMetadataDto *)metadata;
- (NSDictionary *)getData;
- (void)registerSessionDelegate:(id<SwSessionDelegate>)delegate;
- (void)unregisterSessionDelegate:(id<SwSessionDelegate>)delegate;
- (void)unregisterAllSessionDelegates;
- (NSString *)getMegaSessionId;

@end
