//
//  SwWisdomConfigurationDto.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 22/06/2021.
//

#import <Foundation/Foundation.h>

@interface SwWisdomConfigurationDto : NSObject

@property (nonatomic, assign) BOOL isLoggingEnabled;
@property (strong) NSString* subdomain;
@property NSInteger connectTimeout;
@property NSInteger readTimeout;
@property NSInteger initialSyncInterval;
@property (nonatomic, strong) NSString* streamingAssetsFolderPath;
@property (nonatomic, strong) NSString* blockingLoaderResourceRelativePath;
@property (nonatomic, assign) NSInteger blockingLoaderViewportPercentage;

@end
