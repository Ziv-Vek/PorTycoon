//
//  SwFullScreenLoader.m
//  SupersonicWisdom
//
//  Created by Perry Shalom on 10/03/2022.
//

#import <UIKit/UIKit.h>

#import "SwFullScreenLoader.h"
#import "SwUtils.h"

#define kScreenWidthDefaultValue 50

@interface SwAnimatedGifView : UIImageView

@property (nonatomic, strong) NSArray *frameImageFilePaths;
@property (nonatomic, assign) NSInteger percentageFromScreenWidth;

- (instancetype) initWithGifFrames:(NSArray *) frames;

@end

@implementation SwAnimatedGifView

- (instancetype)initWithGifFrames:(NSArray *) frames {
    self = [super init];
    if (self) {
        _frameImageFilePaths = frames;
        if (_frameImageFilePaths == nil) {
            _frameImageFilePaths = @[];
        }
    }

    return self;
}

- (void)didMoveToSuperview {
    [super didMoveToSuperview];
    if ([self superview] == nil) {
        // Removed
        [self stopAnimating];
        return;
    }
    
    if ([[self animationImages] count]) return;

    [SwUtils runBlock: ^{
        NSMutableArray *imageFrames = [NSMutableArray array];
        [[[NSOperationQueue alloc] init] addOperationWithBlock:^{
            for (NSString *frameImagePath in self.frameImageFilePaths) {
                UIImage *frame = [UIImage imageWithContentsOfFile: frameImagePath];
                CGFloat percentageFromScreen;
                if (self.percentageFromScreenWidth > 0) {
                    percentageFromScreen = ((CGFloat) self.percentageFromScreenWidth) / 100;
                } else {
                    percentageFromScreen = kScreenWidthDefaultValue;
                }

                CGFloat squareDimension = [UIScreen mainScreen].bounds.size.width * percentageFromScreen;
                frame = [SwAnimatedGifView resizedImage:frame scaledToSize: CGSizeMake(squareDimension, squareDimension)];

                if (frame) {
                    [imageFrames addObject: frame];
                } else {
                    NSLog(@"Failed to find image frame at path: %@", frameImagePath);
                }
            }
            
            [[NSOperationQueue mainQueue] addOperationWithBlock:^{
                self.animationImages = imageFrames;
                NSTimeInterval duration = ((CGFloat) imageFrames.count) / ((CGFloat)60); // 60 fps
                self.animationDuration = duration;
                [self startAnimating];
            }];
        }];
    } afterDelayInSeconds: 0.5];
}

- (void)removeFromSuperview {
    [super removeFromSuperview];

    [self stopAnimating];
}

+ (UIImage *)resizedImage:(UIImage *)image scaledToSize:(CGSize)newSize {
    if (!image) return image;
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 0.0);
    [image drawInRect: CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *resized = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();

    return resized;
}

@end

@interface SwFullScreenLoader()

@property (nonatomic, strong) UIButton *blockingButton;
@property (nonatomic, strong) SwAnimatedGifView *animatedGifView;
@property (nonatomic, assign) NSInteger percentageFromScreenWidth;
@property (nonatomic, strong) NSString *framesFolderPath;

@end

@implementation SwFullScreenLoader

static NSArray* frameImagePaths = nil;

- (UIViewController *)containerViewController {
    return [SwFullScreenLoader topViewController];
}

- (instancetype) init {
    self = [super init];
    if (self)
    {
        // init stuff
    }
    return self;
}

- (instancetype)initWithFramesFolderPath:(NSString *) framesFolderPath withPercentageFromScreenWidth:(NSInteger)percentageFromScreenWidth {
    self = [super init];
    // initialize
    self.blockingButton = [UIButton buttonWithType:UIButtonTypeCustom];
    
    self.blockingButton.backgroundColor = [UIColor.blackColor colorWithAlphaComponent: 0.5];
    
    self.blockingButton.userInteractionEnabled = true;
    [self.blockingButton addTarget: self action: @selector(onClick:) forControlEvents: UIControlEventTouchUpInside];
    
    self.percentageFromScreenWidth = percentageFromScreenWidth;
    self.framesFolderPath = framesFolderPath;

    return self;
}

- (BOOL)show {
    if (self.animatedGifView == nil) {
        if (![SwUtils isEmpty: self.framesFolderPath]) {
            NSArray *frameFilesList = [[NSFileManager defaultManager] contentsOfDirectoryAtPath: self.framesFolderPath error: nil];
            if (frameImagePaths == nil) {
                // Lazy loading file names list, because we access the file system (files count) on UI thread.
                NSMutableArray *frames = [[NSMutableArray alloc] init];
                for (NSString *frameFilePath in frameFilesList) {
                    [frames addObject: [self.framesFolderPath stringByAppendingPathComponent: frameFilePath]];
                }
                
                frameImagePaths = [[NSArray arrayWithArray: frames] sortedArrayUsingComparator:^NSComparisonResult(id  _Nonnull obj1, id  _Nonnull obj2) {
                    return [obj1 compare: obj2] == NSOrderedDescending;
                }];
            }
        }

        self.animatedGifView = [[SwAnimatedGifView alloc] initWithGifFrames: frameImagePaths];
        
        if (self.percentageFromScreenWidth >= 0 && self.percentageFromScreenWidth <= 100) {
            self.animatedGifView.percentageFromScreenWidth = self.percentageFromScreenWidth;
        } else {
            self.animatedGifView.percentageFromScreenWidth = kScreenWidthDefaultValue;
        }
        
        [self.blockingButton addSubview: self.animatedGifView];
    }

    UIView *containerView = [[self containerViewController] view];
    if (containerView == nil) return NO;

    [self addSubview: self.blockingButton];
    [containerView addSubview: self];

    self.blockingButton.alpha = 0;

    [SwFullScreenLoader strechToSuperView: self];
    [SwFullScreenLoader strechToSuperView: self.blockingButton];
    [SwFullScreenLoader centerInSuperView: self.animatedGifView];

    
    [UIView animateWithDuration: 0.1 delay:0 options: UIViewAnimationOptionCurveLinear animations:^{
        self.blockingButton.alpha = 1;
    } completion: nil];

    return YES;
}

- (BOOL)hide {
    [UIView animateWithDuration: 0.1 delay:0 options: UIViewAnimationOptionCurveLinear animations:^{
        self.blockingButton.alpha = 0;
    } completion:^(BOOL finished) {
        [self removeFromSuperview];
        [self.blockingButton removeFromSuperview];
    }];
    
    return YES;
}

+(void) centerInSuperView: (UIView *) subview {
    UIView *superview = [subview superview];
    if (superview == nil) [NSException
                           raise: @"Cannot add constraints"
                           format:@"subview must have a superview"];

    subview.translatesAutoresizingMaskIntoConstraints = NO;

    [superview addConstraints:({
        @[ [NSLayoutConstraint
           constraintWithItem: subview
           attribute:NSLayoutAttributeCenterX
           relatedBy:NSLayoutRelationEqual
           toItem: superview
           attribute:NSLayoutAttributeCenterX
           multiplier:1.f constant:0.f],

           [NSLayoutConstraint
            constraintWithItem: subview
            attribute:NSLayoutAttributeCenterY
            relatedBy:NSLayoutRelationEqual
            toItem: superview
            attribute:NSLayoutAttributeCenterY
            multiplier:1.f constant:0.f] ];
    })];
}

+(void) strechToSuperView: (UIView *) subview {
    UIView *superview = [subview superview];
    if (superview == nil) [NSException
                           raise: @"Cannot add constraints"
                           format:@"subview must have a superview"];

    subview.translatesAutoresizingMaskIntoConstraints = NO;

    NSLayoutConstraint *width =[NSLayoutConstraint
                                        constraintWithItem: subview
                                        attribute:NSLayoutAttributeWidth
                                        relatedBy:0
                                        toItem: superview
                                        attribute:NSLayoutAttributeWidth
                                        multiplier:1.0
                                        constant:0];
    NSLayoutConstraint *height =[NSLayoutConstraint
                                         constraintWithItem: subview
                                         attribute:NSLayoutAttributeHeight
                                         relatedBy:0
                                         toItem: superview
                                         attribute:NSLayoutAttributeHeight
                                         multiplier:1.0
                                         constant:0];
    NSLayoutConstraint *top = [NSLayoutConstraint
                                       constraintWithItem: subview
                                       attribute:NSLayoutAttributeTop
                                       relatedBy:NSLayoutRelationEqual
                                       toItem: superview
                                       attribute:NSLayoutAttributeTop
                                       multiplier:1.0f
                                       constant:0.f];
    NSLayoutConstraint *leading = [NSLayoutConstraint
                                           constraintWithItem: subview
                                           attribute:NSLayoutAttributeLeading
                                           relatedBy:NSLayoutRelationEqual
                                           toItem: superview
                                           attribute:NSLayoutAttributeLeading
                                           multiplier:1.0f
                                           constant:0.f];
    [superview addConstraint:width];
    [superview addConstraint:height];
    [superview addConstraint:top];
    [superview addConstraint:leading];
}

-(void) onClick: (UIButton *) sender {
    // Ignore taps...
}

+ (UIViewController *)topViewController {
    return [self topViewControllerFromViewController:
            [UIApplication sharedApplication].keyWindow.rootViewController];
}

+ (UIViewController *)topViewControllerFromViewController:(UIViewController *)viewController {

    if ([viewController isKindOfClass:[UINavigationController class]]) {
        UINavigationController *navigationController = (UINavigationController *)viewController;
        return [self
                topViewControllerFromViewController:[navigationController.viewControllers lastObject]];
    }

    if ([viewController isKindOfClass:[UITabBarController class]]) {
        UITabBarController *tabController = (UITabBarController *)viewController;
        return [self topViewControllerFromViewController:tabController.selectedViewController];
    }

    if (viewController.presentedViewController) {
        return [self topViewControllerFromViewController:viewController.presentedViewController];
    }

    return viewController;
}

@end
