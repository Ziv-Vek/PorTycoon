//
//  SwEventsReporterProtocol.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 21/12/2020.
//

#import <Foundation/Foundation.h>

@protocol SwEventsReporterProtocol <NSObject>

@required
- (void)reportEvent:(NSDictionary *)event;

@end
