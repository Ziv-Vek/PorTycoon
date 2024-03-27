//
//  SwWisdomStorageProtocol.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/12/2020.
//

#import <Foundation/Foundation.h>

@protocol SwWisdomStorageProtocol <NSObject>

@required
- (NSInteger)insertInto:(NSString *)table values:(NSDictionary *)values;
- (NSInteger)insertInto:(NSString *)table listWithValues:(NSArray *)list;
- (NSArray *)queryFrom:(NSString *)table orderBy:(NSString *)orderBy limit:(NSInteger)limit;
- (NSInteger)updateIn:(NSString *)table whereClause:(NSString *)whereClause whereArgs:(NSArray *)whereArgs listWithValues:(NSArray *)list;
- (NSInteger)deleteFrom:(NSString *)table whereClause:(NSString *)whereClause whereArgs:(NSArray *)whereArgs;
- (NSInteger)deleteAllFrom:(NSString *)table;
@end
