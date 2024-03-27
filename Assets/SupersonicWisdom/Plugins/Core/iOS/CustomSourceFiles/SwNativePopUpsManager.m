#import "SwNativePopUpsManager.h"

@implementation SwNativePopUpsManager

static UIAlertController *_currentAlert = nil;

+ (void)unregisterAlertView {
  if (_currentAlert != nil) {
    _currentAlert = nil;
  }
}

+ (void)dismissCurrentAlert {
  if (_currentAlert != nil) {
    [_currentAlert dismissViewControllerAnimated:NO completion:nil];
    _currentAlert = nil;
  }
}

+ (void)showDialog:(NSString *)title
           message:(NSString *)msg
          yesTitle:(NSString *)b1
           noTitle:(NSString *)b2
    preferredStyle:(UIAlertControllerStyle)style {

  UIAlertController *alertController =
      [UIAlertController alertControllerWithTitle:title
                                          message:msg
                                   preferredStyle:style];

  UIAlertAction *yesAction = [UIAlertAction
      actionWithTitle:b1
                style:UIAlertActionStyleDefault
              handler:^(UIAlertAction *_Nonnull action) {
                [SwNativePopUpsManager unregisterAlertView];
                UnitySendMessage("SupersonicWisdom",
                                 "IOSNativeDialogCloseMessage",
                                 [SwDataConvertor NSIntToChar:0]);
              }];

  UIAlertAction *noAction = [UIAlertAction
      actionWithTitle:b2
                style:UIAlertActionStyleDefault
              handler:^(UIAlertAction *_Nonnull action) {
                [SwNativePopUpsManager unregisterAlertView];
                UnitySendMessage("SupersonicWisdom",
                                 "IOSNativeDialogCloseMessage",
                                 [SwDataConvertor NSIntToChar:1]);
              }];
  [alertController addAction:noAction];
  [alertController addAction:yesAction];
  /* 
  if (@available(iOS 9.0, *)) {
    alertController.preferredAction = yesAction;
  }
  */

  [[[[UIApplication sharedApplication] keyWindow] rootViewController]
      presentViewController:alertController
                   animated:YES
                 completion:nil];
  _currentAlert = alertController;
}

+ (void)showMessage:(NSString *)title
            message:(NSString *)msg
            okTitle:(NSString *)b1 {

  UIAlertController *alertController =
      [UIAlertController alertControllerWithTitle:title
                                          message:msg
                                   preferredStyle:UIAlertControllerStyleAlert];

  UIAlertAction *okAction = [UIAlertAction
      actionWithTitle:b1
                style:UIAlertActionStyleDefault
              handler:^(UIAlertAction *_Nonnull action) {
                [SwNativePopUpsManager unregisterAlertView];
                UnitySendMessage("SupersonicWisdom",
                                 "IOSNativeMessageCloseMessage", "");
              }];
  [alertController addAction:okAction];

  [[[[UIApplication sharedApplication] keyWindow] rootViewController]
      presentViewController:alertController
                   animated:YES
                 completion:nil];
  _currentAlert = alertController;
}

@end

#pragma mark - External Methods

void _swShowDialog(char *title, char *message, char *yes, char *no, int style) {
  NSInteger preferredStyle = style;
  if (preferredStyle > 1 || preferredStyle < 0) {
    preferredStyle = 0;
  }
  UIAlertControllerStyle styleEnumValue = (UIAlertControllerStyle)style;

  [SwNativePopUpsManager showDialog:[SwDataConvertor charToNSString:title]
                            message:[SwDataConvertor charToNSString:message]
                           yesTitle:[SwDataConvertor charToNSString:yes]
                            noTitle:[SwDataConvertor charToNSString:no]
                     preferredStyle:styleEnumValue];
}

void _swShowMessage(char *title, char *message, char *ok) {
  [SwNativePopUpsManager showMessage:[SwDataConvertor charToNSString:title]
                             message:[SwDataConvertor charToNSString:message]
                             okTitle:[SwDataConvertor charToNSString:ok]];
}

void _swDismissCurrentAlert() { [SwNativePopUpsManager dismissCurrentAlert]; }
