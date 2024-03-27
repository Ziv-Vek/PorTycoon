package wisdom.library.domain.events;

public interface IEventsQueue {
    void setInitialSyncInterval(int interval);
    void startQueue();
    void stopQueue();
}
