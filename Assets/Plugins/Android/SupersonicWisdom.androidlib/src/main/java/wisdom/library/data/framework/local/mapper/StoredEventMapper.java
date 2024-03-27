package wisdom.library.data.framework.local.mapper;

import android.content.ContentValues;

import wisdom.library.data.framework.local.storage.api.BackupEventsContract;
import wisdom.library.domain.events.StoredEvent;
import wisdom.library.domain.mapper.Mapper;
import wisdom.library.util.SdkLogger;

import org.json.JSONException;
import org.json.JSONObject;

public class StoredEventMapper extends Mapper<StoredEvent, ContentValues> {


    public StoredEventMapper() {}

    @Override
    public ContentValues map(StoredEvent event) {
        ContentValues cv = new ContentValues();
        cv.put(BackupEventsContract.EventEntry._ID, event.getRowId());
        cv.put(BackupEventsContract.EventEntry.COLUMN_NAME_ATTEMPT, event.getAttempt());
        JSONObject json = event.getEventDetails();
        cv.put(BackupEventsContract.EventEntry.COLUMN_NAME_EVENT, json.toString());
        return cv;
    }

    @Override
    public StoredEvent reverse(ContentValues cv) {
        int attempt = cv.getAsInteger(BackupEventsContract.EventEntry.COLUMN_NAME_ATTEMPT);
        long rowId = cv.getAsLong(BackupEventsContract.EventEntry._ID);
        String eventContent = cv.getAsString(BackupEventsContract.EventEntry.COLUMN_NAME_EVENT);

        StoredEvent event = null;
        
        try {
            JSONObject eventJson = new JSONObject(eventContent);
            return new StoredEvent(rowId, attempt, eventJson);
        } catch (JSONException e) {
            SdkLogger.error(this, "Error reversing stored event\nexception: " + e);
        }

        return event;
    }
}