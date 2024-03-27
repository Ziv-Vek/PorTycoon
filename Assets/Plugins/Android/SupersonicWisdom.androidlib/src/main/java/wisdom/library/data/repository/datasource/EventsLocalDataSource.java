package wisdom.library.data.repository.datasource;

import wisdom.library.data.framework.local.EventsLocalApi;
import wisdom.library.domain.events.StoredEvent;

import org.json.JSONObject;

import java.util.List;

public class EventsLocalDataSource {

    private EventsLocalApi mApi;

    public EventsLocalDataSource(EventsLocalApi api) {
        mApi = api;
    }

    public long storeEvent(JSONObject event) {
        return mApi.storeEvent(event);
    }

    public long storeEvents(List<StoredEvent> events) {
        return mApi.storeEvents(events);
    }

    public long storeTemporaryEvent(JSONObject event) {
        return mApi.storeTemporaryEvent(event);
    }

    public List<StoredEvent> getEvents(int amount) {
        return mApi.getEvents(amount);
    }

    public List<StoredEvent> getTemporaryEvents(int amount) {
        return mApi.getTemporaryEvents(amount);
    }

    public int update(List<StoredEvent> events) {
        return mApi.update(events);
    }

    public int deleteEvents(List<StoredEvent> events) {
        return mApi.deleteEvents(events);
    }

    public int deleteAllEvents() {
        return mApi.deleteAllEvents();
    }

    public int deleteTemporaryEvents(List<StoredEvent> events) {
        return mApi.deleteTemporaryEvents(events);
    }

    public int deleteAllTemporaryEvents() {
        return mApi.deleteAllTemporaryEvents();
    }
}
