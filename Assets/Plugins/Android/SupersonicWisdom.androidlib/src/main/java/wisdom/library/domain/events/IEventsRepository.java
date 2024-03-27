package wisdom.library.domain.events;

import org.json.JSONObject;

import java.util.List;

public interface IEventsRepository {
    void sendEventAsync(JSONObject details);
    void sendEventAsync(JSONObject details, IEventsRemoteStorageListener listener);
    int sendEvents(List<StoredEvent> events);
    void sendEventsAsync(List<StoredEvent> events, IEventsRemoteStorageListener listener);

    long storeEvent(JSONObject details);
    long storeEvents(List<StoredEvent> events);
    long storeTemporaryEvent(JSONObject details);

    List<StoredEvent> getEvents(int amount);
    List<StoredEvent> getTemporaryEvents(int amount);

    int updateSyncEventAttempts(List<StoredEvent> events);
    int deleteEvents(List<StoredEvent> events);
    int deleteAllEvents();
    int deleteTemporaryEvents(List<StoredEvent> events);
    int deleteAllTemporaryEvents();
}
