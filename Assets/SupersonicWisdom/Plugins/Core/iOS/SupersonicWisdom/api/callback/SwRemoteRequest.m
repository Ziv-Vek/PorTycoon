//
//  SwRemoteRequest.m
//  SupersonicWisdomTestApp
//
//  Created by Omer Bentov on 02/02/2023.
//

#import <Foundation/Foundation.h>
#import "SwRemoteRequest.h"

@implementation SwRemoteRequest{
    OnSwResponse _listener;
    NSDictionary *_request;
}

-(id)initWith:(OnSwResponse)listener request:(NSDictionary *)request{
    _listener = listener;
    _request = request;
    
    return self;
}

-(OnSwResponse)getListener{
    return _listener;
}
-(NSDictionary *)getRequest{
    return _request;
}

@end
