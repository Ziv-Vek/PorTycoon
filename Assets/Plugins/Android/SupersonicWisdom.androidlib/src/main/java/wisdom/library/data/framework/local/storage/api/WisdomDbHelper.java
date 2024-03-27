package wisdom.library.data.framework.local.storage.api;

import android.content.Context;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteDatabaseCorruptException;
import android.database.sqlite.SQLiteFullException;
import android.database.sqlite.SQLiteOpenHelper;

import wisdom.library.util.SdkLogger;

import java.util.concurrent.Semaphore;

public class WisdomDbHelper extends SQLiteOpenHelper {

    private static final int DB_VERSION = 1;
    private static final String DB_NAME = "wisdom_db";
    private static final String TAG = WisdomDbHelper.class.getSimpleName();

    /**
     * This semaphore is intended to guard our tables only in `WisdomEventsStorage.query`, `WisdomDbHelper.onDelete` and `WisdomDbHelper.onCreate`.
     * That's because we experienced issues specifically in `WisdomEventsStorage.query` after (probably) `WisdomDbHelper.onDatabaseException` was called simultaneously.
     */
    private final Semaphore tableChangesSemaphore = new Semaphore(1);

    private static final String SQL_CREATE_BACKUP_TABLE = SQL_CREATE_TABLE(BackupEventsContract.EventEntry.TABLE_NAME);

    private static final String SQL_CREATE_TMP_TABLE = SQL_CREATE_TABLE(TemporaryEventsContract.EventEntry.TABLE_NAME);

    private static String SQL_CREATE_TABLE(String tableName) {
        return "CREATE TABLE " + tableName + " (" +
                ISwBaseColumns._ID + " INTEGER PRIMARY KEY AUTOINCREMENT," +
                ISwBaseColumns.COLUMN_NAME_EVENT + " TEXT," +
                ISwBaseColumns.COLUMN_NAME_ATTEMPT + " INTEGER)";
    }

    private static final String SQL_DROP_BACKUP_EVENTS_TABLE =
            "DROP TABLE IF EXISTS " + BackupEventsContract.EventEntry.TABLE_NAME;

    private static final String SQL_DROP_TMP_EVENTS_TABLE =
            "DROP TABLE IF EXISTS " + TemporaryEventsContract.EventEntry.TABLE_NAME;

    public WisdomDbHelper(Context context) {
        super(context, DB_NAME, null, DB_VERSION);
    }

    @Override
    public void onCreate(SQLiteDatabase sqLiteDatabase) {
        createTables(sqLiteDatabase);
    }

    @Override
    public void onUpgrade(SQLiteDatabase sqLiteDatabase, int oldVersion, int newVersion) {
        deleteTables(sqLiteDatabase);
        createTables(sqLiteDatabase);
    }

    @Override
    public void onDowngrade(SQLiteDatabase sqLiteDatabase, int oldVersion, int newVersion) {
        onUpgrade(sqLiteDatabase, oldVersion, newVersion);
    }

    public void onDatabaseException(SQLiteDatabase sqLiteDatabase, SQLException ex) {
        if (ex instanceof SQLiteFullException || ex instanceof SQLiteDatabaseCorruptException) {
            deleteTables(sqLiteDatabase);
            createTables(sqLiteDatabase);
        }
    }

    public void createTables(SQLiteDatabase sqLiteDatabase) {
        createTables(sqLiteDatabase, null);
    }

    public void createTables(SQLiteDatabase sqLiteDatabase, String tableName) {
        try {
            lock();

            if (tableName == null) {
                sqLiteDatabase.execSQL(SQL_CREATE_BACKUP_TABLE);
                sqLiteDatabase.execSQL(SQL_CREATE_TMP_TABLE);
            } else {
                sqLiteDatabase.execSQL(SQL_CREATE_TABLE(tableName));
            }
        } catch (Exception e) {
            SdkLogger.error(WisdomDbHelper.class.getSimpleName(), e);
        } finally {
            releaseIfLocked();
        }
    }

    public void deleteTables(SQLiteDatabase sqLiteDatabase) {
        try {
            lock();

            sqLiteDatabase.execSQL(SQL_DROP_BACKUP_EVENTS_TABLE);
            sqLiteDatabase.execSQL(SQL_DROP_TMP_EVENTS_TABLE);
        } catch (Exception e) {
            SdkLogger.error(WisdomDbHelper.class.getSimpleName(), e);
        } finally {
            releaseIfLocked();
        }
    }

    public void lock() throws InterruptedException {
        tableChangesSemaphore.acquire();
    }

    public void releaseIfLocked() {
        tableChangesSemaphore.release();
    }
}
