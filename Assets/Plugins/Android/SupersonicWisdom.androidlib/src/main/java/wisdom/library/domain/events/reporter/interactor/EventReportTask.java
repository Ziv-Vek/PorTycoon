package wisdom.library.domain.events.reporter.interactor;


import wisdom.library.data.framework.network.core.WisdomNetwork;
import wisdom.library.domain.events.IEventsRepository;
import wisdom.library.domain.events.StoredEvent;
import wisdom.library.domain.events.dto.Constants;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

public class EventReportTask implements Runnable {

    private IEventsRepository mRepository;
    private JSONObject mEvent;

    public EventReportTask(IEventsRepository repo, JSONObject event) {
        mRepository = repo;
        mEvent = event;
    }

    @Override
    public void run() {
        long rowId = mRepository.storeTemporaryEvent(mEvent);
        if (rowId > -1) {
            StoredEvent se = new StoredEvent(rowId, Constants.INITIAL_EVENT_ATTEMPT, mEvent);
            List<StoredEvent> events = new ArrayList<>(1);
            events.add(se);

            int responseCode = mRepository.sendEvents(events);
            handleResponse(events, responseCode);
        }
    }

    private void handleResponse(List<StoredEvent> events, int responseCode) {
        if (responseCode > 199 && responseCode < 400) {
            mRepository.deleteTemporaryEvents(events);
        } else if (responseCode == WisdomNetwork.WISDOM_RESPONSE_CODE_BAD_REQUEST) { //BAD REQUEST
            mRepository.deleteTemporaryEvents(events);
        } else if (responseCode == WisdomNetwork.WISDOM_INTERNAL_NO_INTERNET) {
            if (mRepository.storeEvent(mEvent) > -1) {
                mRepository.deleteTemporaryEvents(events);
            }
        } else {
            if (mRepository.storeEvent(mEvent) > -1) {
                mRepository.deleteTemporaryEvents(events);
            }
        }
    }
}
