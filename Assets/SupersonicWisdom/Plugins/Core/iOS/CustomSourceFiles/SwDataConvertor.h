#import <Foundation/Foundation.h>

@interface SwDataConvertor : NSObject

+ (NSString *)charToNSString:(char *)value;
+ (const char *)NSIntToChar:(NSInteger)value;
+ (const char *)NSStringToChar:(NSString *)value;

@end
