//
//  SwFullScreenLoader.h
//  SupersonicWisdom
//
//  Created by Perry Shalom on 10/03/2022.
//

#import <UIKit/UIKit.h>

@interface SwFullScreenLoader : UIView

- (instancetype)initWithFramesFolderPath:(NSString *) framesFolderPath withPercentageFromScreenWidth:(NSInteger) percentageFromScreenWidth;
- (BOOL) hide;
- (BOOL) show;

@end
