//
//  BackgroundWatchdogDelegate.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/11/2020.
//

@protocol BackgroundWatchdogDelegate <NSObject>

@required
- (void)onAppMovedToBackground;
- (void)onAppMovedToForeground;

@end
