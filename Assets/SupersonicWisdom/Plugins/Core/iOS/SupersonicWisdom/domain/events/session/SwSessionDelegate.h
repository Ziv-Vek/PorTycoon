//
//  SwSessionDelegate.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

#import <Foundation/Foundation.h>

@protocol SwSessionDelegate <NSObject>

@required
- (void)onSessionStarted:(NSString *)sessionId;
- (void)onSessionEnded:(NSString *)sessionId;
- (NSString *)getAdditionalDataJsonMethod;

@end
