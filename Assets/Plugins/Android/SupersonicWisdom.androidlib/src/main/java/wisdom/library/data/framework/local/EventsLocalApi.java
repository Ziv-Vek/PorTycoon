package wisdom.library.data.framework.local;

import android.content.ContentValues;

import wisdom.library.data.framework.local.storage.api.BackupEventsContract;
import wisdom.library.data.framework.local.storage.api.IWisdomStorage;
import wisdom.library.data.framework.local.storage.api.TemporaryEventsContract;
import wisdom.library.domain.events.StoredEvent;
import wisdom.library.domain.events.dto.Constants;
import wisdom.library.data.framework.local.mapper.StoredEventMapper;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

public class EventsLocalApi {

    private IWisdomStorage mLocalStorage;
    private StoredEventMapper mStoredEventMapper;

    public EventsLocalApi(IWisdomStorage storage, StoredEventMapper storedMapper) {
        mLocalStorage = storage;
        mStoredEventMapper = storedMapper;
    }

    public synchronized long storeEvent(JSONObject event) {
        ContentValues cv = new ContentValues();
        cv.put(BackupEventsContract.EventEntry.COLUMN_NAME_ATTEMPT, Constants.INITIAL_EVENT_ATTEMPT);
        cv.put(BackupEventsContract.EventEntry.COLUMN_NAME_EVENT, event.toString());
        return mLocalStorage.insert(BackupEventsContract.EventEntry.TABLE_NAME, cv);
    }

    public synchronized long storeTemporaryEvent(JSONObject event) {
        ContentValues cv = new ContentValues();
        cv.put(TemporaryEventsContract.EventEntry.COLUMN_NAME_ATTEMPT, Constants.INITIAL_EVENT_ATTEMPT);
        cv.put(TemporaryEventsContract.EventEntry.COLUMN_NAME_EVENT, event.toString());
        return mLocalStorage.insert(TemporaryEventsContract.EventEntry.TABLE_NAME, cv);
    }

    public synchronized long storeEvents(List<StoredEvent> events) {
        List<ContentValues> cvs = new ArrayList<>(events.size());
        for (StoredEvent event : events) {
            ContentValues cv = new ContentValues();
            cv.put(BackupEventsContract.EventEntry.COLUMN_NAME_ATTEMPT, Constants.INITIAL_EVENT_ATTEMPT);
            cv.put(BackupEventsContract.EventEntry.COLUMN_NAME_EVENT, event.getEventDetails().toString());
            cvs.add(cv);
        }
        return mLocalStorage.insert(BackupEventsContract.EventEntry.TABLE_NAME, cvs);
    }

    public synchronized List<StoredEvent> getEvents(int amount) {
        String sortOrder = BackupEventsContract.EventEntry._ID + " ASC";
        return get(BackupEventsContract.EventEntry.TABLE_NAME, sortOrder, amount);
    }

    public synchronized List<StoredEvent> getTemporaryEvents(int amount) {
        String sortOrder = TemporaryEventsContract.EventEntry._ID + " ASC";
        return get(TemporaryEventsContract.EventEntry.TABLE_NAME, sortOrder, amount);
    }

    private List<StoredEvent> get(String table, String sortOrder, int amount) {
        List<ContentValues> cvs = mLocalStorage.query(table, sortOrder, amount);
        if (cvs == null || cvs.size() == 0) {
            return new ArrayList<>();
        }

        List<StoredEvent> events = new ArrayList<>(cvs.size());
        for (ContentValues cv : cvs) {
            events.add(mStoredEventMapper.reverse(cv)); 
        }

        return events;
    }

    public synchronized int update(List<StoredEvent> events) {
        if (events == null || events.isEmpty()) {
            return 0;
        }

        List<ContentValues> cvs = new ArrayList<>(events.size());
        for (StoredEvent event : events) {
            cvs.add(mStoredEventMapper.map(event));
        }

        return mLocalStorage.update(BackupEventsContract.EventEntry.TABLE_NAME,
                BackupEventsContract.EventEntry._ID + " = ?",
                BackupEventsContract.EventEntry._ID,
                cvs
        );
    }

    public synchronized int deleteEvents(List<StoredEvent> events) {
        return delete(
                BackupEventsContract.EventEntry.TABLE_NAME,
                BackupEventsContract.EventEntry._ID,
                events
        );
    }

    public synchronized int deleteAllEvents() {
        return deleteAll(BackupEventsContract.EventEntry.TABLE_NAME);
    }

    public synchronized int deleteTemporaryEvents(List<StoredEvent> events) {
        return delete(
                TemporaryEventsContract.EventEntry.TABLE_NAME,
                TemporaryEventsContract.EventEntry._ID,
                events
        );
    }

    public synchronized int deleteAllTemporaryEvents() {
        return deleteAll(TemporaryEventsContract.EventEntry.TABLE_NAME);
    }

    private int delete(String fromTable, String byColumn, List<StoredEvent> events) {
        if ( events == null || events.isEmpty()) {
            return 0;
        }

        int lastIndex = events.size() - 1;
        long from = Math.min(events.get(0).getRowId(), events.get(lastIndex).getRowId());
        long to = Math.max(events.get(0).getRowId(), events.get(lastIndex).getRowId());
        String whereClause = byColumn + " BETWEEN ? AND ?";
        String[] whereArgs = new String[] {
                String.valueOf(from),
                String.valueOf(to)
        };

        return mLocalStorage.delete(fromTable, whereClause, whereArgs);
    }

    private int deleteAll(String fromTable) {
        return mLocalStorage.delete(fromTable, null, null);
    }
}