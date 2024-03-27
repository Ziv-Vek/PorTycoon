//
//  SwUtils.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 19/11/2020.
//

#import "SwUtils.h"
#import "SwConstants.h"

@implementation SwUtils

+ (BOOL) isRunningReleaseVersion {
#if DEBUG
    return NO;
#else
    return YES;
#endif
}

+ (BOOL)isEmpty:(NSString *)string {
    return ([string isKindOfClass: [NSNull class]] || string == nil || [string length] == 0);
}

+ (NSDictionary *)cleanDictionary:(NSDictionary *)dict {
    NSMutableDictionary *mutDict = [NSMutableDictionary dictionaryWithDictionary:dict];
    NSArray *keys = [mutDict allKeys];
    for (NSString *key in keys) {
        if (![dict[key] isKindOfClass:[NSString class]]) {
            continue;
        }
        
        if ([self isEmpty:dict[key]]) {
            [mutDict removeObjectForKey:key];
        }
    }
    
    return mutDict;
}

+ (NSString *)replaceNil:(NSString *)string {
    return string ?: @"";
}

+ (NSDictionary *)replaceNilDict:(NSDictionary *)dict {
    return dict ?: [[NSDictionary alloc] init];
}

+ (NSData *)dataWithJSONObject:(id)obj error:(NSError *__autoreleasing *)error {
    if (@available(iOS 11.0, *)) {
        return [NSJSONSerialization dataWithJSONObject:obj options:NSJSONWritingSortedKeys error:error];
    }
    return [NSJSONSerialization dataWithJSONObject:obj options:kNilOptions error:error];
}

+ (NSString *)stringWithJSONObject:(id)obj error:(NSError *__autoreleasing *)error {
    NSData *nsData = [self dataWithJSONObject:obj error:error];
    NSString *str = [[NSString alloc] initWithData:nsData encoding:NSASCIIStringEncoding];
    return str;
}


+ (void)runBlock:(dispatch_block_t) block afterDelayInSeconds:(NSTimeInterval) delayInSeconds {
    if (!block) return;

    dispatch_time_t popTime = dispatch_time(DISPATCH_TIME_NOW, delayInSeconds * NSEC_PER_SEC);
    dispatch_after(popTime, dispatch_get_main_queue(), block);
}

+ (NSDictionary *)jsonStringToDict:(NSString *) jsonString {
    if ([jsonString isKindOfClass: [NSDictionary class]]) return jsonString;

    NSDictionary *dict;
    
    if (![jsonString length]) {
        return @{};
    }
    
    NSData *jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSError *error;
    
    if(!jsonData) {
        return @{};
    }
    
    id jsonObject;
    
    jsonObject = [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:&error];
    if(error){
        error = [NSError errorWithDomain:@"Sw" code:1 userInfo:@{@"exception":error}];
        [SWSDKLogger logMessage:@"Error parsing JSON: %@", jsonString];
    }
    else{
        if ([jsonObject isKindOfClass:[NSDictionary class]]) {
            dict = (NSDictionary *)jsonObject;
        }
        else{
            NSLog(@"Error parsing JSON: %@", jsonString);
        }
    }
    
    return dict ?: @{};
}

+ (NSDictionary *)dataToDictionary:(NSData *) data {
    if ([data isKindOfClass: [NSDictionary class]]) return (NSDictionary *) data;

    NSDictionary *dict;
    
    if(!data) {
        return @{};
    }
    
    NSError *error;
    id jsonObject;
    
    jsonObject = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if(error){
        error = [NSError errorWithDomain:@"Sw" code:1 userInfo:@{@"exception":error}];
        [SWSDKLogger logMessage:@"Error parsing JSON: %@", data];
    }
    else{
        if ([jsonObject isKindOfClass:[NSDictionary class]]) {
            dict = (NSDictionary *)jsonObject;
        }
        else{
            [SWSDKLogger logMessage:@"Error parsing JSON: %@", jsonObject];
        }
    }
    
    return dict ?: @{};
}

+ (NSDictionary *)mergeDictionaries:(NSDictionary *)dict1 other:(NSDictionary *)dict2{
    if(dict1 == nil && dict2 != nil){
        return dict2;
    }
    else if(dict1 != nil && dict2 == nil){
        return dict1;
    }
    else if(dict1 != nil && dict2 != nil){
        NSMutableDictionary *mutDict = [dict1 mutableCopy];
        [mutDict addEntriesFromDictionary:dict2];
    
        return mutDict;
    }
    else{
        return [[NSDictionary alloc] init];
    }
}

+(NSString *) toJsonString:(NSDictionary *) jsonDictionary {
    NSError *error;
    NSData *jsonData = [SwUtils toJsonData: jsonDictionary];

    if (![jsonData length]) {
        return @"{}";
    }

    NSString* jsonString = [[NSString alloc] initWithData: jsonData encoding: NSUTF8StringEncoding];
    [SWSDKLogger logMessage:@"jsonString: %@", jsonString];
    
    return error ? @"{}" : jsonString;
}

+(NSData *) toJsonData:(NSDictionary *) jsonDictionary {
    if (![jsonDictionary isKindOfClass: [NSDictionary class]] || ![jsonDictionary count]) {
        // Avoiding 'NSInvalidArgumentException'
        return nil;
    }

    NSMutableDictionary *mutableDictionary = [NSMutableDictionary dictionaryWithDictionary: jsonDictionary];
    for (id key in [jsonDictionary allKeys]) {
        if (!jsonDictionary[key]) {
            [mutableDictionary removeObjectForKey: key];
        }
    }

    NSData *jsonData;
    NSError *error;

    // Instead of NSJSONWritingPrettyPrinted, we're not using any option.
    jsonData = [NSJSONSerialization dataWithJSONObject: mutableDictionary options: kNilOptions error: &error];
    if(error){
        [SWSDKLogger logMessage:@"Error parsing JSON string: %@\n exception: %@", jsonDictionary, error];
    }

    return jsonData;
}

+ (NSDictionary *)createEvent:(NSString *)eventName sessionData:(NSDictionary *)sessionData conversionData:(NSString *)conversionData metdadata:(NSString *)metadata customs:(NSString *)customs extra:(NSString *)extra {
    NSNumber *clientTs = [NSNumber numberWithLong:[[NSDate date] timeIntervalSince1970]];
    
    NSMutableDictionary *mutDict = [@{
        KEY_EVENT_NAME: [SwUtils replaceNil:eventName],
        KEY_CONVERSION_DATA: [SwUtils replaceNil:conversionData],
        KEY_CLIENT_TS: clientTs,
        KEY_EVENT_ID: [[NSUUID UUID] UUIDString]
    } mutableCopy];
    
    mutDict = [[SwUtils mergeDictionaries:mutDict other:sessionData] mutableCopy];
    NSDictionary *extraDict = [SwUtils replaceNilDict:[SwUtils jsonStringToDict:extra]];
    
    if([extraDict count] > 0){
        [mutDict setValue:extraDict forKey:KEY_EXTRA];
    }
    
    NSDictionary *dict = [mutDict copy];
    
    NSDictionary *metadataDict = [SwUtils jsonStringToDict:metadata];
    NSDictionary *customsDict = [SwUtils jsonStringToDict:customs];
    
    dict = [SwUtils mergeDictionaries:dict other:metadataDict];
    dict = [SwUtils mergeDictionaries:dict other:customsDict];
    
    return dict;
}


@end
