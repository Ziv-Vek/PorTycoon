package wisdom.library.domain.events.reporter.interactor;

import wisdom.library.domain.events.IEventsRepository;

import org.json.JSONObject;

import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

public class EventsReporter implements IEventsReporter {

    private IEventsRepository mEventsRepository;
    private Executor mExecutor;

    public EventsReporter(IEventsRepository eventsRepository) {
        mExecutor = Executors.newFixedThreadPool(3);
        mEventsRepository = eventsRepository;
    }

    @Override
    public void reportEvent(final JSONObject event) {
        EventReportTask task = new EventReportTask(mEventsRepository, event);
        mExecutor.execute(task);
    }
}
