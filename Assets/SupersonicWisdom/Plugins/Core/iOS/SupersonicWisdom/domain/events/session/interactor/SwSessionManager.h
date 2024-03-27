//
//  SwSessionManager.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

#import <Foundation/Foundation.h>
#import "SwEventMetadataDto.h"
#import "SwSessionDelegate.h"
#import "BackgroundWatchdogDelegate.h"
#import "SwSessionManagement.h"
#import "SwEventsRepositoryProtocol.h"
#import "SwEventMetadataManagement.h"
#import "SwConversionDataManagement.h"
#import "SwEventsReporter.h"
#import "SwEventsQueueProtocol.h"
#import "SwConstants.h"

@interface SwSessionManager : NSObject <SwSessionDelegate, SwSessionManagement>

- (id)initWithReporter:(id<SwEventsReporterProtocol>)reporter
            EventsRepo:(id<SwEventsRepositoryProtocol>)eventsRepo
       MetadataManager:(id<SwEventMetadataManagement>)metadataRepo
 ConversionDataManager:(id<SwConversionDataManagement>)conversionDataManager
            EventQueue:(id<SwEventsQueueProtocol>)queue
           UserDefault:(NSUserDefaults *)prefs;

@end
