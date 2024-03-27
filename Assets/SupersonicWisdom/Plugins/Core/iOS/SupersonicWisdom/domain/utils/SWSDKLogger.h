//
//  SWSDKLogger.h
//  SupersonicWisdom
//
//  Created by Perry Sh on 05/09/2022.
//

#import <Foundation/Foundation.h>

@interface SWSDKLogger : NSObject

+(void) logMessage:(NSString *)message, ...;

+(void) setIsEnabled:(BOOL) isEnabled;

@end
