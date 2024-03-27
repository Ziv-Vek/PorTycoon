package wisdom.library.domain.mapper;

import wisdom.library.domain.events.StoredEvent;
import wisdom.library.util.SwUtils;

import org.json.JSONArray;
import org.json.JSONObject;

import java.util.List;

public class ListStoredEventJsonMapper extends Mapper<List<StoredEvent>, JSONObject> {

    private static final String KEY_ATTEMPT = "attempt";
    private static final String KEY_EVENTS = "events";
    
    public ListStoredEventJsonMapper() {}

    @Override
    public JSONObject map(List<StoredEvent> events) {
        JSONObject json = new JSONObject();
        if (events == null || events.isEmpty()) {
            return json;
        }

        JSONArray eventsJson = new JSONArray();
        for (StoredEvent event : events) {
            JSONObject eventJson = event.getEventDetails();
            SwUtils.addToJson(eventJson, KEY_ATTEMPT, event.getAttempt());
            eventsJson.put(eventJson);
        }

        SwUtils.addToJson(json, KEY_EVENTS, eventsJson);
        return json;
    }

    @Override
    public List<StoredEvent> reverse(JSONObject value) {
        throw new UnsupportedOperationException("Unsupported method");
    }
}
