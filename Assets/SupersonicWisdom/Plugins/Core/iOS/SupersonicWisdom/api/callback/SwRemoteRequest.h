//
//  SwRemoteRequest.h
//  SupersonicWisdom
//
//  Created by Omer Bentov on 02/02/2023.
//

#import <Foundation/Foundation.h>
#import "SwNetworkCallbacks.h"

@interface SwRemoteRequest : NSObject

-(id)initWith:(OnSwResponse)listener request:(NSDictionary *)request;
-(OnSwResponse)getListener;
-(NSDictionary *)getRequest;
@end
