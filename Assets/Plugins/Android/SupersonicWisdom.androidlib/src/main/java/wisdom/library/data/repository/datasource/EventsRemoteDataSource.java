package wisdom.library.data.repository.datasource;

import wisdom.library.data.framework.network.api.IInternalRequestListener;
import wisdom.library.data.framework.remote.EventsRemoteApi;
import wisdom.library.domain.events.IEventsRemoteStorageListener;
import wisdom.library.domain.events.StoredEvent;

import org.json.JSONObject;

import java.util.List;

public class EventsRemoteDataSource {

    private EventsRemoteApi mApi;

    public EventsRemoteDataSource(EventsRemoteApi api) {
        mApi = api;
    }

    public void sendEventAsync(JSONObject details, final IEventsRemoteStorageListener listener) {
        mApi.sendEventAsync(details, new IInternalRequestListener() {
            @Override
            public void onResponseFailed(String key, int iteration, int code, String error) {
                if (listener != null) {
                    listener.onEventsStoredRemotely(false);
                }
            }

            @Override
            public void onResponseSuccess(String key, int iteration, String body) {
                if (listener != null) {
                    listener.onEventsStoredRemotely(true);
                }
            }
        });
    }

    public int sendEvents(List<StoredEvent> events) {
        return mApi.sendEvents(events, null);
    }

    public void sendEventsAsync(List<StoredEvent> events, final IEventsRemoteStorageListener listener) {
        mApi.sendEventsAsync(events, new IInternalRequestListener() {
            @Override
            public void onResponseFailed(String key, int iteration, int code, String error) {
                if (listener != null) {
                    listener.onEventsStoredRemotely(false);
                }
            }

            @Override
            public void onResponseSuccess(String key, int iteration, String body) {
                if (listener != null) {
                    listener.onEventsStoredRemotely(true);
                }
            }
        });
    }
}
