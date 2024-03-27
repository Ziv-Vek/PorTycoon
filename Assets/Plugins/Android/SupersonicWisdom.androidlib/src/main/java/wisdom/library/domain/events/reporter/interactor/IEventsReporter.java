package wisdom.library.domain.events.reporter.interactor;

import org.json.JSONObject;

public interface IEventsReporter {

    void reportEvent(JSONObject event);

}
