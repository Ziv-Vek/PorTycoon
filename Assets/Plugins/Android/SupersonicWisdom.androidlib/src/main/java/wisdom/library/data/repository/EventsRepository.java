package wisdom.library.data.repository;

import wisdom.library.data.repository.datasource.EventsLocalDataSource;
import wisdom.library.data.repository.datasource.EventsRemoteDataSource;
import wisdom.library.domain.events.IEventsRepository;
import wisdom.library.domain.events.IEventsRemoteStorageListener;
import wisdom.library.domain.events.StoredEvent;

import org.json.JSONObject;

import java.util.List;

public class EventsRepository implements IEventsRepository {

    private EventsRemoteDataSource mRemoteSource;
    private EventsLocalDataSource mLocalSource;

    public EventsRepository(EventsRemoteDataSource remote, EventsLocalDataSource local) {
        mRemoteSource = remote;
        mLocalSource = local;
    }

    @Override
    public synchronized void sendEventAsync(JSONObject details) {
        sendEventAsync(details, null);
    }

    @Override
    public synchronized void sendEventAsync(JSONObject details, IEventsRemoteStorageListener listener) {
        mRemoteSource.sendEventAsync(details, listener);
    }

    @Override
    public synchronized int sendEvents(List<StoredEvent> events) {
        return mRemoteSource.sendEvents(events);
    }

    @Override
    public synchronized void sendEventsAsync(List<StoredEvent> events, IEventsRemoteStorageListener listener) {
        mRemoteSource.sendEventsAsync(events, listener);
    }

    @Override
    public synchronized long storeEvent(JSONObject details) {
        return mLocalSource.storeEvent(details);
    }

    @Override
    public synchronized long storeTemporaryEvent(JSONObject details) {
        return mLocalSource.storeTemporaryEvent(details);
    }

    @Override
    public synchronized long storeEvents(List<StoredEvent> events) {
        return mLocalSource.storeEvents(events);
    }

    @Override
    public synchronized List<StoredEvent> getEvents(int amount) {
        return mLocalSource.getEvents(amount);
    }

    @Override
    public synchronized List<StoredEvent> getTemporaryEvents(int amount) {
        return mLocalSource.getTemporaryEvents(amount);
    }

    @Override
    public synchronized int updateSyncEventAttempts(List<StoredEvent> events) {
        return mLocalSource.update(events);
    }

    @Override
    public synchronized int deleteEvents(List<StoredEvent> events) {
        return mLocalSource.deleteEvents(events);
    }

    @Override
    public int deleteAllEvents() {
        return mLocalSource.deleteAllEvents();
    }

    @Override
    public synchronized int deleteTemporaryEvents(List<StoredEvent> events) {
        return mLocalSource.deleteTemporaryEvents(events);
    }

    @Override
    public int deleteAllTemporaryEvents() {
        return mLocalSource.deleteAllTemporaryEvents();
    }
}
