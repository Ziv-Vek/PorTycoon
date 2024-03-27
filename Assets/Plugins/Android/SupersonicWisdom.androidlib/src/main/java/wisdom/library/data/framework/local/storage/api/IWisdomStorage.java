package wisdom.library.data.framework.local.storage.api;

import android.content.ContentValues;

import java.util.List;

public interface IWisdomStorage {
    long insert(String table, ContentValues cv);
    long insert(String table, List<ContentValues> cvs);
    List<ContentValues> query(String table, String sortOrder, int amount);
    int update(String table, String whereClause, String whereArg, List<ContentValues> cvs);
    int delete(String table, String whereClause, String[] whereArgs);
}
