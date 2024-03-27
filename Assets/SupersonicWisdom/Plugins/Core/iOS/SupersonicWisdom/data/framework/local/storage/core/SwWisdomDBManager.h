//
//  SwWisdomDbHelper.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/12/2020.
//

#import <Foundation/Foundation.h>
#import <sqlite3.h>

@interface SwWisdomDBManager : NSObject {
    NSString *databasePath;
    BOOL isDBCreated;
}

- (id)initDatabase;
- (BOOL)createDB;
- (NSInteger)insertInto:(NSString *)table forColumns:(NSString *)columns withValues:(NSString *)values;
- (NSInteger)updateIn:(NSString *)table set:(NSString *)params where:(NSString *)condition;
- (NSArray *)selectFrom:(NSString *)table
                columns:(NSString *)columns
                  where:(NSString *)condition
                orderBy:(NSString *)orderBy
                  limit:(NSInteger)limit;
- (NSInteger)deleteFrom:(NSString *)table where:(NSString *)condition;

@end
