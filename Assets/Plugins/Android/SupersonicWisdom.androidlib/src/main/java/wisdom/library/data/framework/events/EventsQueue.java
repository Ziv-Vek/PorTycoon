package wisdom.library.data.framework.events;

import android.os.Build;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.Looper;
import android.os.Message;

import wisdom.library.data.framework.network.core.WisdomNetwork;
import wisdom.library.domain.events.IEventsQueue;
import wisdom.library.domain.events.IEventsRepository;
import wisdom.library.domain.events.StoredEvent;
import wisdom.library.util.SdkLogger;

import java.util.List;

public class EventsQueue implements IEventsQueue {

    private EventsHandler mHandler;
    private IEventsRepository mEventsRepository;
    private boolean mIsHandlerInitialized;
    private int mInitialSyncInterval;

    public EventsQueue(IEventsRepository eventsRepository, int initialSyncInterval) {
        mEventsRepository = eventsRepository;
        mIsHandlerInitialized = false;
        mInitialSyncInterval = initialSyncInterval;
    }

    private void initializeHandler() {
        HandlerThread mHandlerThread = new HandlerThread("EventsHandlerThread");
        mHandlerThread.start();
        Looper looper = mHandlerThread.getLooper();
        mHandler = new EventsHandler(looper, mEventsRepository, mInitialSyncInterval);
        mIsHandlerInitialized = true;
        mHandler.sendEmptyMessage(EventsHandler.MERGE_TABLES_MSG_ID);
    }

    private void destroyHandler() {
        mHandler.cancelAllTasks(EventsHandler.SYNC_MSG_ID);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.JELLY_BEAN_MR2) {
            mHandler.getLooper().quitSafely();
        } else {
            mHandler.getLooper().quit();
        }

        mIsHandlerInitialized = false;
    }

    @Override
    public void setInitialSyncInterval(int interval) {
        mInitialSyncInterval = interval;
        if (mHandler != null) {
            mHandler.mmInitialSyncInterval = interval;
        }
    }

    @Override
    public void startQueue() {
        if (!mIsHandlerInitialized) {
            initializeHandler();
        }
    }

    @Override
    public void stopQueue() {
        if (mIsHandlerInitialized) {
            destroyHandler();
        }
    }

    private static class EventsHandler extends Handler {

        private static final int SYNC_MSG_ID = 1234;
        private static final int MERGE_TABLES_MSG_ID = 1235;
        private static final int CONNECTIVITY_ISSUE_MSG_ID = 1236;
        private static final int MAX_EVENTS = 100;
        private static final int MAX_RETRIES = 10;
        private static final int CONNECTIVITY_ISSUE_DELAY = 10000;

        private IEventsRepository mEventsRepository;
        private int mmInitialSyncInterval;
        private int mmRetry;

        public EventsHandler(Looper looper, IEventsRepository eventsRepository,
                             int initialSyncInterval) {
            super(looper);
            mEventsRepository = eventsRepository;
            mmInitialSyncInterval = initialSyncInterval;
            mmRetry = 1;
        }

        @Override
        public void handleMessage(Message msg) {
            switch (msg.what) {
                case MERGE_TABLES_MSG_ID:
                    handleMergeTables();
                    sendEmptyMessage(SYNC_MSG_ID);
                    break;
                case SYNC_MSG_ID:
                case CONNECTIVITY_ISSUE_MSG_ID:
                    boolean isHandled = handleSync();
                    if (isHandled) {
                        sendEmptyMessageDelayed(SYNC_MSG_ID, getNextSyncInterval());
                    } else {
                        sendEmptyMessageDelayed(CONNECTIVITY_ISSUE_MSG_ID, CONNECTIVITY_ISSUE_DELAY);
                    }
                    break;
            }
        }

        private void handleMergeTables() {
            List<StoredEvent> tmpEvents = mEventsRepository.getTemporaryEvents(MAX_EVENTS);
            while (!tmpEvents.isEmpty()) {
                long result = mEventsRepository.storeEvents(tmpEvents);
                if (result > 0) {
                    mEventsRepository.deleteTemporaryEvents(tmpEvents);
                    tmpEvents = mEventsRepository.getTemporaryEvents(MAX_EVENTS);
                } else {
                    SdkLogger.error(EventsQueue.class.getSimpleName(), "`storeEvents` failed. Tried to store " + tmpEvents.size() + " events but stored nothing.");
                    tmpEvents.clear();
                }
            }
        }

        /**
         * Handles scheduled sync
         * @return false if was unable to handle sync(No internet) otherwise true
         */
        private boolean handleSync() {
            int responseCode = 0;
            List<StoredEvent> events = mEventsRepository.getEvents(MAX_EVENTS);
            if (shouldPerformSync(events)) {
                increaseEventAttempts(events);
                responseCode = mEventsRepository.sendEvents(events);
                handleResponse(events, responseCode);
            }

            return responseCode != WisdomNetwork.WISDOM_INTERNAL_NO_INTERNET;
        }

        /**
         * Increases attempts number of each event which should be sent in current task.
         * @param events which should be sent at the current task.
         */
        private void increaseEventAttempts(List<StoredEvent> events) {
            for (StoredEvent se : events) {
                se.increaseAttempt();
            }
        }

        private boolean shouldPerformSync(List<StoredEvent> events) {
            return (!events.isEmpty());
        }

        private void handleResponse(List<StoredEvent> events, int responseCode) {
            if (responseCode >= WisdomNetwork.WISDOM_RESPONSE_CODE_OK && responseCode <= WisdomNetwork.WISDOM_RESPONSE_CODE_BAD_REQUEST) {
                /*
                 * Will handle BAD REQUEST together with success request to prevent code duplication,
                 * in case logic if SUCCESS REQUEST will be changed check if need to separate logic of BAD REQUEST
                 */
                handleSuccessResponse(events);
            } else if (responseCode != WisdomNetwork.WISDOM_INTERNAL_NO_INTERNET) {
                handleErrorResponse(events);
            }
        }

        private void handleSuccessResponse(List<StoredEvent> events) {
            resetRetries();
            mEventsRepository.deleteEvents(events);
        }

        private void handleErrorResponse(List<StoredEvent> events) {
            if (mmRetry >= MAX_RETRIES) {
                mEventsRepository.deleteAllTemporaryEvents();
                mEventsRepository.deleteAllEvents();
            }

            mmRetry++;
            mEventsRepository.updateSyncEventAttempts(events);
            //TODO in future add log with error
        }

        /**
         * Cancels all tasks with specified id.
         * @param taskId of the pending tasks which should be canceled.
         */
        public void cancelAllTasks(int taskId) {
            removeMessages(taskId);
        }

        private void resetRetries() {
            mmRetry = 1;
        }

        /**
         * Calculates the next sync interval with exponential backoff policy
         * https://developer.android.com/reference/android/app/job/JobInfo#BACKOFF_POLICY_EXPONENTIAL
         * @return sync interval
         */
        private int getNextSyncInterval() {
            int numFailures = Math.min(mmRetry, MAX_RETRIES);
            int exponent = numFailures - 1;
            int power = (int)Math.pow(2, exponent);

            return (mmInitialSyncInterval * power);
        }
    }
}
