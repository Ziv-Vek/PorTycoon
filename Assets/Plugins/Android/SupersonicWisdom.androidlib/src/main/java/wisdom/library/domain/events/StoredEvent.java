package wisdom.library.domain.events;

import org.json.JSONObject;

public class StoredEvent {

    private long mRowId;
    private int mAttempt;
    private JSONObject mEvent;

    public StoredEvent(long rowId, int attempt, JSONObject event) {
        mRowId = rowId;
        mAttempt = attempt;
        mEvent = event;
    }

    public long getRowId() {
        return mRowId;
    }

    public JSONObject getEventDetails() {
        return mEvent;
    }

    public int getAttempt() {
        return mAttempt;
    }

    public void increaseAttempt() {
        mAttempt++;
    }
}
