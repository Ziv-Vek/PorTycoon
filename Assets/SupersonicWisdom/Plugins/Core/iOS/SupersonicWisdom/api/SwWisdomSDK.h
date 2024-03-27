//
//  WisdomSDK.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import <Foundation/Foundation.h>
#import "../domain/events/dto/SwEventMetadataDto.h"
#import "SwWisdomSessionDelegate.h"
#import "SwWisdomSessionCallback.h"
#import "SwSDK.h"
#import "SwWisdomConfigurationDto.h"

@interface SwWisdomSDK : NSObject <SwWisdomSessionCallback, SwConnectivityStatusCallback>

+ (void)initSdkWithConfig:(SwWisdomConfigurationDto *)configuration;
+ (BOOL)toggleBlockingLoader:(BOOL)shouldPresent;
+ (void)updateWisdomConfiguration:(SwWisdomConfigurationDto *)configuration;
+ (BOOL)isInitialized;
+ (void)initializeSession:(NSString *)metadataJson;
+ (void)setEventMetadata:(NSString *)metadataJson;
+ (void)updateEventMetadata:(NSString *)metadataJson;
+ (void)trackEvent:(NSString *)eventName customsJson:(NSString *)customsJson extraJson:(NSString *)extraJson;
+ (void)sendRequest:(NSString *)requestJsonString;
+ (NSString *)getConnectionStatus;
+ (void)destroy;
+ (NSString *)getMegaSessionId;

@end


