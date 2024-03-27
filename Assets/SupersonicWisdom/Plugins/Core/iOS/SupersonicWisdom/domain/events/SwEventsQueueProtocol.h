//
//  SwEventsQueueProtocol.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/12/2020.
//

@protocol SwEventsQueueProtocol <NSObject>

@required
- (void)startQueue;
- (void)stopQueue;
- (void)updateSyncInterval:(NSTimeInterval)interval;

@end
