package wisdom.library.data.repository.datasource;

import wisdom.library.data.framework.local.EventMetadataLocalApi;

public class EventMetadataLocalDataSource {

    private EventMetadataLocalApi mApi;

    public EventMetadataLocalDataSource(EventMetadataLocalApi api) {
        mApi = api;
    }

    public void put(String eventMetadataJson) {
        mApi.save(eventMetadataJson);
    }

    public String get() {
        return mApi.get();
    }

    public void update(String eventMetadataJson) {
        mApi.update(eventMetadataJson);
    }
}
