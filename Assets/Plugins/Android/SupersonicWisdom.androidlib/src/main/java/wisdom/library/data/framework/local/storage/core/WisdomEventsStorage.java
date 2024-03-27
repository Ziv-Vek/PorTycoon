package wisdom.library.data.framework.local.storage.core;

import android.content.ContentValues;
import android.database.Cursor;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;
import android.text.TextUtils;

import wisdom.library.data.framework.local.storage.api.BackupEventsContract;
import wisdom.library.data.framework.local.storage.api.WisdomDbHelper;
import wisdom.library.data.framework.local.storage.api.IWisdomStorage;
import wisdom.library.util.SdkLogger;

import java.util.ArrayList;
import java.util.List;

public class WisdomEventsStorage implements IWisdomStorage {

    public static final String[] ALL_COLUMNS = null;
    public static final String NO_WHERE_CLAUSE = null;
    public static final String[] NO_WHERE_ARGS = null;
    public static final String NO_GROUP_BY = null;
    public static final String NO_HAVING = null;
    public static final String NO_SORT_ORDER = null;
    private static final String TAG = WisdomEventsStorage.class.getSimpleName();

    private WisdomDbHelper mDbHelper;

    public WisdomEventsStorage(WisdomDbHelper dbHelper) {
        mDbHelper = dbHelper;
    }

    @Override
    public synchronized long insert(String table, ContentValues cv) {
        SQLiteDatabase db = mDbHelper.getWritableDatabase();
        long result = -1;
        try {
            result = db.insertOrThrow(table, null, cv);
        } catch (SQLException e) {
            SdkLogger.error(TAG, e);
            mDbHelper.onDatabaseException(db, e);
        } finally {
            db.close();
        }

        return result;
    }

    @Override
    public synchronized long insert(String table, List<ContentValues> cvs) {
        if (cvs == null || cvs.size() == 0) {
            return 0;
        }

        long insertedEvents = 0;
        SQLiteDatabase db = mDbHelper.getWritableDatabase();
        db.beginTransaction();

        try {
            for (ContentValues cv : cvs) {
                long result = db.insert(table, null, cv);
                if (result > -1) {
                    insertedEvents++;
                }
            }

            db.setTransactionSuccessful();
            db.endTransaction();
        } catch (SQLException e) {
            SdkLogger.error(TAG, e);
            db.endTransaction();
            mDbHelper.onDatabaseException(db, e);
        } finally {
            db.close();
        }

        return insertedEvents;
    }

    @Override
    public synchronized List<ContentValues> query(String table, String sortOrder, int amount) {
        List<ContentValues> cvs = new ArrayList<>(amount);

        try {

            SQLiteDatabase db = mDbHelper.getReadableDatabase();
            String limit = String.valueOf(amount);

            mDbHelper.lock();

            Cursor cursor = null;
            try {
                cursor = db.query(
                        table,
                        ALL_COLUMNS,
                        NO_WHERE_CLAUSE,
                        NO_WHERE_ARGS,
                        NO_GROUP_BY,
                        NO_HAVING,
                        sortOrder,
                        limit
                );

                if (!isNullOrEmpty(cursor)) {
                    while (cursor.moveToNext()) {
                        ContentValues cv = parseCursor(cursor);
                        cvs.add(cv);
                    }
                }
            } catch (Throwable throwable) {
                mDbHelper.releaseIfLocked();
                String errorMessage = throwable.getMessage();
                if (errorMessage != null && errorMessage.contains("no such table")) {
                    // This means that the table named {table} is somehow deleted, it's already should be fixed by calling `mDbHelper.lock();`
                    // but we're handling it anyway just in case this error will occur again.
                    mDbHelper.createTables(mDbHelper.getWritableDatabase(), table);
                }
            } finally {
                if (cursor != null) {
                    cursor.close();
                }
                db.close();
                mDbHelper.releaseIfLocked();
            }
        } catch (InterruptedException e) {
            SdkLogger.error(TAG, "The calling to `mDbHelper.lock();` failed!", e);
        } catch (Exception e) {
            SdkLogger.error(TAG, "Error occurred before querying.", e);
        }

        return cvs;
    }

    @Override
    public synchronized int update(String table, String whereClause, String whereCvField, List<ContentValues> cvs) {
        if (cvs == null || cvs.size() == 0) {
            return 0;
        }

        int updatedRows = 0;
        SQLiteDatabase db = mDbHelper.getWritableDatabase();
        db.beginTransaction();

        try {
            for (ContentValues cv : cvs) {
                String[] whereArgs = new String[]{
                        cv.getAsString(whereCvField)
                };
                updatedRows += db.update(table, cv, whereClause, whereArgs);
            }

            db.setTransactionSuccessful();
            db.endTransaction();
        } catch (SQLException e) {
            SdkLogger.error(TAG, e);
            db.endTransaction();
            mDbHelper.onDatabaseException(db, e);
        } finally {
            db.close();
        }

        return updatedRows;
    }

    @Override
    public synchronized int delete(String table, String whereClause, String[] whereArgs) {
        SQLiteDatabase db = mDbHelper.getWritableDatabase();
        int deleted = 0;

        try {
            deleted = db.delete(table, whereClause, whereArgs);
        } catch (SQLException e) {
            SdkLogger.error(TAG, e);
            mDbHelper.onDatabaseException(db, e);
        } finally {
            db.close();
        }

        return deleted;
    }

    private boolean isNullOrEmpty(Cursor cursor) {
        return (cursor == null || cursor.getCount() == 0);
    }

    private ContentValues parseCursor(Cursor cursor) {
        ContentValues cv = new ContentValues();
        if (cursor == null) {
            return cv;
        }

        int columnIndex;

        columnIndex = cursor.getColumnIndex(BackupEventsContract.EventEntry._ID);
        if(columnIndex >= 0) {
            long rowId = cursor.getLong(columnIndex);
            cv.put(BackupEventsContract.EventEntry._ID, rowId);
        }

        columnIndex = cursor.getColumnIndex(BackupEventsContract.EventEntry.COLUMN_NAME_EVENT);
        if(columnIndex >= 0) {
            String eventContent = cursor.getString(columnIndex);
            if (!TextUtils.isEmpty(eventContent)) {
                cv.put(BackupEventsContract.EventEntry.COLUMN_NAME_EVENT, eventContent);
            }
        }

        columnIndex = cursor.getColumnIndex(BackupEventsContract.EventEntry.COLUMN_NAME_ATTEMPT);
        if(columnIndex >= 0) {
            int attempt = cursor.getInt(columnIndex);
            cv.put(BackupEventsContract.EventEntry.COLUMN_NAME_ATTEMPT, attempt);
        }

        return cv;
    }
}