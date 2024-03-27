package wisdom.library.data.framework.remote;

import wisdom.library.data.framework.network.api.IInternalRequestListener;
import wisdom.library.data.framework.network.api.INetwork;
import wisdom.library.domain.events.StoredEvent;
import wisdom.library.domain.mapper.ListStoredEventJsonMapper;

import org.json.JSONObject;

import java.util.List;

public class EventsRemoteApi {

    private final String ANALYTICS_URL;
    private final String ANALYTICS_BULK_URL;
    private final String EVENT = "event";
    

    private INetwork mNetwork;
    private ListStoredEventJsonMapper mListJsonMapper;

    public EventsRemoteApi(INetwork network,
                           String subdomain,
                           ListStoredEventJsonMapper listJsonMapper) {

        mNetwork = network;
        mListJsonMapper = listJsonMapper;
        if (subdomain == null || subdomain.isEmpty()) {
            ANALYTICS_BULK_URL = "https://analytics.mobilegamestats.com/events";
            ANALYTICS_URL = "https://analytics.mobilegamestats.com/event";
        } else {
            ANALYTICS_BULK_URL = "https://" + subdomain + ".analytics.mobilegamestats.com/events";
            ANALYTICS_URL = "https://" + subdomain + ".analytics.mobilegamestats.com/event";
        }
    }

    public void sendEventAsync(JSONObject details, IInternalRequestListener listener) {
        mNetwork.sendAsync(EVENT, ANALYTICS_URL, details, listener);
    }

    public int sendEvents(List<StoredEvent> events, IInternalRequestListener listener) {
        JSONObject json = mListJsonMapper.map(events);
        return mNetwork.send(EVENT, ANALYTICS_BULK_URL, json, listener);
    }

    public void sendEventsAsync(List<StoredEvent> events, IInternalRequestListener listener) {
        JSONObject json = mListJsonMapper.map(events);
        mNetwork.sendAsync(EVENT, ANALYTICS_BULK_URL, json, listener);
    }
}
