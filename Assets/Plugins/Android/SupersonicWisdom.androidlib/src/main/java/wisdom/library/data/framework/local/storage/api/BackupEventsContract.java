package wisdom.library.data.framework.local.storage.api;

public class BackupEventsContract {

    private BackupEventsContract() {}

    public static class EventEntry implements ISwBaseColumns {
        public static final String TABLE_NAME = "backup_wisdom_events";
    }
}
