#import <Foundation/Foundation.h>
#import "SwTypeUtil.h"

char* _swGetCountry() {
    NSString *language = [[NSLocale currentLocale] objectForKey:NSLocaleCountryCode];
    return swStringCopy([language UTF8String]);
}
