//
//  SwWisdomResponseDelegate.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 19/11/2020.
//

@protocol SwWisdomResponseDelegate <NSObject>

- (void)onResponseFailedWithError:(NSString *)error statusCode:(NSInteger)code response:(NSData *)data;
- (void)onResponseSucceededWithStatusCode:(NSInteger)code response:(NSData *)data;

@end
