//
//  SwWisdomDbHelper.m
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 16/12/2020.
//

#import "SwWisdomDBManager.h"
#import "SwStorageUtils.h"

#define SW_DB_NAME @"wisdom_db.db"
#define SW_ERR_DB_NOT_CREATED -2

#define SW_CREATE_TABLE_SCHEMA_V1 @"CREATE TABLE IF NOT EXISTS %@ (%@ INTEGER PRIMARY KEY AUTOINCREMENT, %@ TEXT, %@ INTEGER)"

static sqlite3 *database = nil;
static sqlite3_stmt *statement = nil;
const static NSInteger SW_CURRENT_DB_VERSION = 3;

@implementation SwWisdomDBManager {
    NSObject *dbLock;
}

- (id)initDatabase {
    if (!(self = [super init])) return nil;
    dbLock = [[NSObject alloc] init];
    isDBCreated = [self createDB];
    
    return self;
}

- (BOOL)createDB {
    // Get the documents directory
    NSArray *dirPaths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
    NSString *docsDir = dirPaths[0];
    // Build the path to the database file
    databasePath = [[NSString alloc] initWithString:[docsDir stringByAppendingPathComponent:SW_DB_NAME]];
    BOOL isNeedToUpdateDatabaseVersion = NO;
    NSFileManager *filemgr = [NSFileManager defaultManager];
    BOOL isDatabaseExists = [filemgr fileExistsAtPath: databasePath];
    
    if (isDatabaseExists == NO) {
        isDatabaseExists = isNeedToUpdateDatabaseVersion = [self createAllTables];
    } else {
        NSInteger oldDatabaseVersion = [self getDatabaseVersion];
        if (oldDatabaseVersion < SW_CURRENT_DB_VERSION) {
            [self upgradeDatabaseVersion:oldDatabaseVersion toVersion:SW_CURRENT_DB_VERSION];
            isNeedToUpdateDatabaseVersion = YES;
        } else if (oldDatabaseVersion > SW_CURRENT_DB_VERSION){
            [self downgradeDatabaseVersion:oldDatabaseVersion toVersion:SW_CURRENT_DB_VERSION];
            isNeedToUpdateDatabaseVersion = YES;
        }
    }
    
    if (isNeedToUpdateDatabaseVersion) {
        [self setDatabaseVersion:SW_CURRENT_DB_VERSION];
    }
    
    return isDatabaseExists;
}

- (BOOL)createAllTables {
    NSInteger resultBackupTable = [self createTable:SW_BACKUP_EVENTS_TABLE withSchema:SW_CREATE_TABLE_SCHEMA_V1];
    NSInteger resultTmpTable = [self createTable:SW_TMP_EVENTS_TABLE withSchema:SW_CREATE_TABLE_SCHEMA_V1];
    
    return resultBackupTable == SQLITE_OK && resultTmpTable == SQLITE_OK;
}

- (NSInteger)createTable:(NSString *)tableName withSchema:(NSString *)schema {
    NSInteger result = sqlite3_open([databasePath UTF8String], &database);
    if (result == SQLITE_OK) {
        char *errMsg;
        NSString *createTableSql = [NSString stringWithFormat: schema, tableName, SW_COLUMN_ID, SW_COLUMN_EVENT, SW_COLUMN_ATTEMPT];
        const char *sql_stmt = [createTableSql UTF8String];
        result = sqlite3_exec(database, sql_stmt, NULL, NULL, &errMsg);
        if (result != SQLITE_OK) {
            //TODO Log error msg in future
        }
    }
    
    sqlite3_close(database);
    return result;
}

- (NSInteger)deleteTable:(NSString *)tableName {
    NSString *dropTable = [NSString stringWithFormat:@"DROP TABLE IF EXISTS %@", tableName];
    NSInteger result = sqlite3_open([databasePath UTF8String], &database);
    if (result == SQLITE_OK) {
        char *errMsg;
        const char *sql_stmt = [dropTable UTF8String];
        result = sqlite3_exec(database, sql_stmt, NULL, NULL, &errMsg);
        if (result != SQLITE_OK) {
            //TODO Log error msg in future
        }
    }
    
    sqlite3_close(database);
    return result;
    
}

- (NSInteger)insertInto:(NSString *)table forColumns:(NSString *)columns withValues:(NSString *)values {
    @synchronized (dbLock) {
        if (!isDBCreated) {
            return SW_ERR_DB_NOT_CREATED;
        }
        
        NSInteger rowId = -1;
        int result = sqlite3_open([databasePath UTF8String], &database);
        if (result == SQLITE_OK) {
            NSString *sql = [NSString stringWithFormat:@"INSERT INTO %@ (%@) values(%@)", table, columns, values];
            const char *sql_stmt = [sql UTF8String];
            result = sqlite3_prepare_v2(database, sql_stmt, -1, &statement, NULL);
            if (result == SQLITE_OK) {
                result = sqlite3_step(statement);
                if (result == SQLITE_DONE) {
                    rowId = sqlite3_last_insert_rowid(database);
                }
            }
            
            sqlite3_finalize(statement);
        }
        
        sqlite3_close(database);
        [self handleDatabaseResult:result];
        
        return rowId;
    }
}

- (NSInteger)updateIn:(NSString *)table set:(NSString *)params where:(NSString *)condition {
    @synchronized (dbLock) {
        if (!isDBCreated) {
            return SW_ERR_DB_NOT_CREATED;
        }
        
        NSInteger updatedRows = -1;
        int result = sqlite3_open([databasePath UTF8String], &database);
        if (result == SQLITE_OK) {
            NSString *sql = [NSString stringWithFormat:@"UPDATE %@ SET %@ WHERE %@", table, params, condition];
            const char *sql_stmt = [sql UTF8String];
            result = sqlite3_prepare_v2(database, sql_stmt, -1, &statement, NULL);
            if (result == SQLITE_OK) {
                result = sqlite3_step(statement);
                if (result == SQLITE_DONE) {
                    updatedRows = sqlite3_changes(database);
                }
            }
            
            sqlite3_finalize(statement);
        }
        
        sqlite3_close(database);
        [self handleDatabaseResult:result];
        
        return updatedRows;
    }
}

- (NSArray *)selectFrom:(NSString *)table
                columns:(NSString *)columns
                  where:(NSString *)condition
                orderBy:(NSString *)orderBy
                  limit:(NSInteger)limit {
    
    @synchronized (dbLock) {
        NSMutableArray *events = [NSMutableArray array];
        if (!isDBCreated) {
            return events;
        }
        NSString *sqlQuery = [SwWisdomDBManager buildSqlQueryFrom:table columns:columns where:condition orderBy:orderBy limit:limit];
        int result = sqlite3_open([databasePath UTF8String], &database);
        if (result == SQLITE_OK) {
            const char *sql_stmt = [sqlQuery UTF8String];
            result = sqlite3_prepare_v2(database, sql_stmt, -1, &statement, NULL);
            if (result == SQLITE_OK) {
                while ((result = sqlite3_step(statement)) == SQLITE_ROW) {
                    NSDictionary *parsedRow = [SwWisdomDBManager parseRowForStatement:statement];
                    [events addObject:parsedRow];
                }
            }
            
            sqlite3_finalize(statement);
        }
        
        sqlite3_close(database);
        [self handleDatabaseResult:result];
        
        return events;
    }
}

+ (NSString *)buildSqlQueryFrom:(NSString *)table
                        columns:(NSString *)columns
                          where:(NSString *)condition
                        orderBy:(NSString *)orderBy
                          limit:(NSInteger)limit {
    NSString *sqlQuery = [NSString stringWithFormat:@"SELECT %@ FROM %@ %@ ", columns, table, condition];
    if (orderBy) {
        sqlQuery = [sqlQuery stringByAppendingString:[NSString stringWithFormat:@"ORDER BY %@ ", orderBy]];
    }
    
    if (limit > 0) {
        sqlQuery = [sqlQuery stringByAppendingString:[NSString stringWithFormat:@"LIMIT %ld", limit]];
    }
    
    return sqlQuery;
}

+ (NSDictionary *)parseRowForStatement:(sqlite3_stmt *)stmt {
    NSMutableDictionary *dict = [NSMutableDictionary dictionary];
    NSInteger columns = sqlite3_column_count(statement);
    for (int column = 0; column < columns; column++) {
        const char *columnName = sqlite3_column_name(stmt, column);
        NSString *key = [NSString stringWithCString:columnName encoding:NSUTF8StringEncoding];
        NSObject *value = [SwWisdomDBManager parseColumn:column ForStatement:stmt];
        [dict setObject:value forKey:key];
    }
    
    return dict;
}

+ (NSObject *)parseColumn:(int)column ForStatement:(sqlite3_stmt *)stmt {
    int type = sqlite3_column_type(stmt, column);
    return [SwWisdomDBManager getValueForStatement:stmt FromColumn:column withType:type];
}

+ (NSObject *)getValueForStatement:(sqlite3_stmt *)stmt FromColumn:(int)column withType:(NSInteger)type {
    switch (type) {
        case SQLITE_TEXT:
            return [NSString stringWithCString:(char *)sqlite3_column_text(stmt, column) encoding:NSUTF8StringEncoding];
        case SQLITE_INTEGER:
            return [NSNumber numberWithInt:sqlite3_column_int(stmt, column)];
        default:
            return [[NSObject alloc] init];
    }
}

- (NSInteger)deleteFrom:(NSString *)table where:(NSString *)condition {
    @synchronized (dbLock) {
        if (!isDBCreated) {
            return SW_ERR_DB_NOT_CREATED;
        }
        
        NSInteger deletedRows = -1;
        int result = sqlite3_open([databasePath UTF8String], &database);
        if (result == SQLITE_OK) {
            NSString *sql;
            if (condition && condition.length > 0) {
                sql = [NSString stringWithFormat:@"DELETE FROM %@ WHERE %@", table, condition];
            } else {
                sql = [NSString stringWithFormat:@"DELETE FROM %@", table];
            }
            
            const char *sql_stmt = [sql UTF8String];
            result = sqlite3_prepare_v2(database, sql_stmt, -1, &statement, NULL);
            if (result == SQLITE_OK) {
                result = sqlite3_step(statement);
                if (result == SQLITE_DONE) {
                    deletedRows = sqlite3_changes(database);
                }
            }
            
            sqlite3_finalize(statement);
        }
        
        sqlite3_close(database);
        [self handleDatabaseResult:result];
        
        return deletedRows;
    }
}

- (void)setDatabaseVersion:(NSInteger)version {
    int result = sqlite3_open([databasePath UTF8String], &database);
    if (result == SQLITE_OK) {
        const char *sql_stmt = [[NSString stringWithFormat:@"PRAGMA user_version=%ld;", version] UTF8String];
        char *errMsg;
        result = sqlite3_exec(database, sql_stmt, NULL, NULL, &errMsg);
    }

    sqlite3_close(database);
    [self handleDatabaseResult:result];
}

- (NSInteger)getDatabaseVersion {
    NSInteger databaseVersion = -1;
    int result = sqlite3_open([databasePath UTF8String], &database);
    if (result == SQLITE_OK) {
        const char *sql_stmt = [@"PRAGMA user_version;" UTF8String];
        result = sqlite3_prepare_v2(database, sql_stmt, -1, &statement, NULL);
        if (result == SQLITE_OK) {
            while ((result = sqlite3_step(statement)) == SQLITE_ROW) {
                databaseVersion = sqlite3_column_int(statement, 0);
            }
        }
        
        sqlite3_finalize(statement);
    }
    
    sqlite3_close(database);
    [self handleDatabaseResult:result];
    
    return databaseVersion;
}

- (void)upgradeDatabaseVersion:(NSInteger)oldVersion toVersion:(NSInteger)newVersion {
    //Here will perform upgrade of database table tables to support new db schema defined by new version
}

- (void)downgradeDatabaseVersion:(NSInteger)oldVersion toVersion:(NSInteger)newVersion {
    //Here will perform downgrade of database while need to revert data tables to previous schema defined by old version
}

- (void)handleDatabaseResult:(NSInteger) result {
    if (result != SQLITE_OK && result != SQLITE_DONE) {
        const char *errorMessage;
        const char *extErrorMsg;
        switch (result) {
            case SQLITE_CORRUPT:
            case SQLITE_FULL:
                [self deleteTable:SW_BACKUP_EVENTS_TABLE];
                [self deleteTable:SW_TMP_EVENTS_TABLE];
                [self createTable:SW_BACKUP_EVENTS_TABLE withSchema:SW_CREATE_TABLE_SCHEMA_V1];
                [self createTable:SW_TMP_EVENTS_TABLE withSchema:SW_CREATE_TABLE_SCHEMA_V1];
            default:
                errorMessage = sqlite3_errstr((int)result);
                extErrorMsg = sqlite3_errmsg(database);
        }
    }
}

@end
