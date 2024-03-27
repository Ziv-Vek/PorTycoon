
@interface SwAppTrackingManager : NSObject

+ (void)_showTrackingConsentDialog;
+ (int)_getTrackingConsentValue;
+ (void)_sendUnityResponse:(NSUInteger) num;
+ (void)openSettingsPage;

@end


NS_ASSUME_NONNULL_BEGIN

typedef NS_ENUM(NSUInteger, ATTrackingManagerAuthorizationStatus) {
    ATTrackingManagerAuthorizationStatusNotDetermined = 0,
    ATTrackingManagerAuthorizationStatusRestricted,
    ATTrackingManagerAuthorizationStatusDenied,
    ATTrackingManagerAuthorizationStatusAuthorized
} NS_SWIFT_NAME(ATTrackingManager.AuthorizationStatus) API_AVAILABLE(ios(14), macosx(11.0), tvos(14));
NS_ASSUME_NONNULL_END

API_AVAILABLE(ios(14), macosx(11.0), tvos(14))

@interface ATTrackingManager : NSObject
@property (class, nonatomic, readonly, assign) ATTrackingManagerAuthorizationStatus trackingAuthorizationStatus;
+ (void)requestTrackingAuthorizationWithCompletionHandler:(void(^)(ATTrackingManagerAuthorizationStatus status))completion;
+ (void)openSettingsPage;

@end



