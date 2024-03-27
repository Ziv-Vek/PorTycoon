//
//  SupersonicWisdom.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import "SwEventMetadataDto.h"
#import "SwWisdomSessionDelegate.h"
#import "SwApplicationLifecycleService.h"
#import "SwApplicationLifecycleServiceRegistrar.h"
#import "SwBackgroundWatchdogRegistrar.h"
#import "SwEventsRepository.h"
#import "SwWisdomConfigurationDto.h"
#import "SwWisdomSessionCallback.h"

@interface SwSDK : NSObject <SwWisdomSessionDelegate>

- (void)initSdkWithConfig:(SwWisdomConfigurationDto *)configuration;
- (BOOL)toggleBlockingLoader:(BOOL)shouldPresent;
- (void)updateWisdomConfiguration:(SwWisdomConfigurationDto *)configuration;
- (BOOL)isInitialized;
- (void)initializeSession:(NSString *)metadataJson;
- (void)registerSessionDelegate:(id<SwWisdomSessionDelegate>)delegate;
- (void)unregisterSessionDelegate:(id<SwWisdomSessionDelegate>)delegate;
- (void)registerConnectivityDelegate:(id<SwConnectivityStatusCallback>)delegate;
- (void)unregisterConnectivityDelegate:(id<SwConnectivityStatusCallback>)delegate;
- (void)setEventMetadata:(NSString *)metadataJson;
- (void)updateEventMetadata:(NSString *)metadataJson;
- (void)trackEvent:(NSString *)eventName customsJson:(NSString *)customsJson extraJson:(NSString *)extraJson;
- (void)sendRequest:(NSString *)requestJsonString withResponseCallback:(OnSwResponse)callback;
- (NSString *)getConnectionStatus;
- (void)destroy;
- (NSString *)getMegaSessionId;

@end

