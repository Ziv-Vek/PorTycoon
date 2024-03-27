//
//  SwStoredEventDictMapper.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 20/12/2020.
//

#import "SwStoredEventDictMapper.h"
#import "SwStorageUtils.h"
#import "SwUtils.h"

@implementation SwStoredEventDictMapper {
    NSObject *lockObj;
}

- (id)init{
    if (!(self = [super init])) return nil;
    lockObj = [NSObject new];
    return self;
}

- (NSDictionary *)map:(SwStoredEvent *)storedEvent {
    @synchronized (lockObj) {
        NSDictionary *parsedEvent = [storedEvent eventDetailsDict];
        NSError *jsonError;
        NSData *jsonData = [SwUtils dataWithJSONObject:parsedEvent error:&jsonError];
        NSString *eventContent = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        NSString *rowId = [@(storedEvent.rowId) stringValue];
        NSDictionary *dict = @{
            SW_COLUMN_ID : rowId,
            SW_COLUMN_ATTEMPT : [NSNumber numberWithInteger:[storedEvent attempt]],
            SW_COLUMN_EVENT : eventContent
        };
        
        return dict;
    }
}

- (SwStoredEvent *)reverse:(NSDictionary *)eventDict {
    @synchronized (lockObj) {
        NSInteger rowId = [[eventDict objectForKey:SW_COLUMN_ID] integerValue];
        NSInteger attempt = [[eventDict objectForKey:SW_COLUMN_ATTEMPT] integerValue];
        NSString *eventContent = [eventDict objectForKey:SW_COLUMN_EVENT];
        NSDictionary *dict= [SwUtils jsonStringToDict:eventContent];
        
        return [[SwStoredEvent alloc] initWithId:rowId attempt:attempt event:dict];
    }
}

@end
