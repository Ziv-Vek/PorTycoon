#import "SwDataConvertor.h"

@implementation SwDataConvertor

+ (NSString *)charToNSString:(char *)value {
  if (value != NULL) {
    return [[NSString alloc] initWithUTF8String:value];
  } else {
    return [[NSString alloc] initWithUTF8String:""];
  }
}

+ (const char *)NSIntToChar:(NSInteger)value {
  NSString *tmp = [NSString stringWithFormat:@"%ld", (long)value];
  return [tmp UTF8String];
}

+ (const char *)NSStringToChar:(NSString *)value {
  return [value UTF8String];
}

@end
