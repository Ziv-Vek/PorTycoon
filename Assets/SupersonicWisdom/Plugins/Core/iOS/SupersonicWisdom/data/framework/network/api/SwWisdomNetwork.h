//
//  SwWisdomNetwork.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 18/11/2020.
//

#import "SwWisdomResponseDelegate.h"
#import "SwNetworkCallbacks.h"

@protocol SwWisdomNetwork <NSObject>

@required
- (void)sendAsync:(NSString *)key
              url:(NSString *)url
         withBody:(NSData *)body
         callback:(OnNetworkResponse)callback;

- (void)sendAsync:(NSString *)key
              url:(NSString *)url
         withBody:(NSData *)body
   connectTimeout:(NSTimeInterval)connectTimeout
      readTimeout:(NSTimeInterval)readTimeout
         callback:(OnNetworkResponse)callback;

@optional
- (void)setConnectTimeout:(NSTimeInterval)timeout;
- (void)setReadTimeout:(NSTimeInterval)timeout;

@end
