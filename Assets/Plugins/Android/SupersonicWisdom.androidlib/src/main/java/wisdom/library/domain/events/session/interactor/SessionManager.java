package wisdom.library.domain.events.session.interactor;

import android.content.SharedPreferences;

import wisdom.library.domain.events.dto.Constants;
import wisdom.library.util.SdkLogger;
import wisdom.library.domain.events.IEventsQueue;
import wisdom.library.domain.events.reporter.interactor.IEventsReporter;
import wisdom.library.domain.events.interactor.IConversionDataManager;
import wisdom.library.domain.events.interactor.IEventMetadataManager;
import wisdom.library.domain.events.session.ISessionListener;
import wisdom.library.util.SwUtils;

import org.json.JSONObject;
import org.json.JSONException;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;
import java.util.concurrent.TimeUnit;

public class SessionManager implements ISessionManager, ISessionListener {
    private static final String MEGA_SESSION_ID;
    private static final String SESSION_EVENT_NAME = "Session";
    private static final String START_SESSION_EVENT = "StartSession";
    private static final String END_SESSION_EVENT = "FinishSession";
    private static final String MEGA_SESSION_COUNTER = "megaSessionCounter";
    private static final String SESSION_IN_MEGA_COUNTER = "sessionInMegaCounter";
    private static final String SESSION_COUNTER = "sessionCounter";
    private static final String SW_PREFS_PREFIX = "sw_";

    private String mCurrentSessionId;
    private long mSessionStartTime;
    private long mSessionEndTime;
    private long mSessionDuration;
    private boolean mIsSessionInitialized;

    private final SharedPreferences mSharedPrefs;
    private IEventMetadataManager mEventMetadataManager;
    private IConversionDataManager mConversionDataManager;
    private List<ISessionListener> mSessionListeners;
    private IEventsReporter mEventsReporter;
    private IEventsQueue mEventsQueue;
    
    private long mMegaSessionsCounter;
    private long mSessionsInMegaSessionCounter;
    private long mTotalSessionsCounter;

    static { // Static constructor, will run once in app session lifetime. Will be reset only after app is killed (of course).
        MEGA_SESSION_ID = UUID.randomUUID().toString();
    }

    public SessionManager(IEventsReporter reporter, IEventMetadataManager eventMetadataManager,
                          IConversionDataManager conversionDataManager, IEventsQueue eventsQueue, SharedPreferences sharedPref) {
        mEventsReporter = reporter;
        mEventMetadataManager = eventMetadataManager;
        mConversionDataManager = conversionDataManager;
        mEventsQueue = eventsQueue;
        mIsSessionInitialized = false;
        mSessionListeners = new ArrayList<>(1);
        mSharedPrefs = sharedPref;
        
        loadSessionData();
        mSessionsInMegaSessionCounter = 0;
        mMegaSessionsCounter++;
        saveSessionData();
    }

    private void loadSessionData() {
        mMegaSessionsCounter = mSharedPrefs.getLong(SW_PREFS_PREFIX + MEGA_SESSION_COUNTER, 0);
        mTotalSessionsCounter = mSharedPrefs.getLong(SW_PREFS_PREFIX + SESSION_COUNTER, 0);
    }

    private void saveSessionData() {
        SharedPreferences.Editor editor = mSharedPrefs.edit();
        editor.putLong(SW_PREFS_PREFIX + MEGA_SESSION_COUNTER, mMegaSessionsCounter);
        editor.putLong(SW_PREFS_PREFIX + SESSION_COUNTER, mTotalSessionsCounter);
        editor.apply();
    }

    private void openSession() {
        mCurrentSessionId = UUID.randomUUID().toString();
        mSessionStartTime = currentTimeSeconds();
        mSessionsInMegaSessionCounter++;
        mTotalSessionsCounter++;
        saveSessionData();
    }

    private void closeSession() {
        mSessionEndTime = currentTimeSeconds();
        mSessionDuration = mSessionEndTime - mSessionStartTime;
    }

    private void resetSession() {
        mCurrentSessionId = "";
        mSessionDuration = 0;
        mSessionStartTime = 0;
        mSessionEndTime = 0;
    }

    private boolean isSessionStarted() {
        return mSessionStartTime != 0;
    }

    private void startSession() {
        mEventsQueue.startQueue();
        openSession();
        
        try {
            String additionalDataString = getAdditionalDataJsonMethod();
            JSONObject customsJson = createEventCustoms(START_SESSION_EVENT, String.valueOf(0));
            
            if(additionalDataString != null && additionalDataString != "") {
                JSONObject additionalData = new JSONObject(additionalDataString);

                try {
                    customsJson = SwUtils.merge(customsJson, additionalData);
                } catch (JSONException e) {
                    SdkLogger.error(this, "Error merging additional data json\nexception: " + e);
                }
            }
            
            JSONObject event = SwUtils.createEvent(SESSION_EVENT_NAME, getData(), mConversionDataManager.getConversionData(), mEventMetadataManager.get(), customsJson.toString(), "");
            mEventsReporter.reportEvent(event);
            onSessionStarted(mCurrentSessionId);
        }
        catch (Exception e) {
            SdkLogger.error(this, "Start session error\nexception: " + e);
        }
    }

    private JSONObject createEventCustoms(String eventName, String sessionDuration) throws JSONException {
        JSONObject customsJson = new JSONObject();
        customsJson.put(Constants.KEY_CUSTOM_1, eventName);
        customsJson.put(Constants.KEY_CUSTOM_2, sessionDuration);
        customsJson.put(MEGA_SESSION_COUNTER, mMegaSessionsCounter);
        customsJson.put(SESSION_IN_MEGA_COUNTER, mSessionsInMegaSessionCounter);
        customsJson.put(SESSION_COUNTER, mTotalSessionsCounter);
        return customsJson;
    }

    private void endSession() {
        closeSession();
        try {
            String additionalDataString = getAdditionalDataJsonMethod();
            JSONObject customsJson = createEventCustoms(END_SESSION_EVENT, String.valueOf(mSessionDuration));
            SdkLogger.log(this, "##" +  additionalDataString);
            
            if(additionalDataString != null && additionalDataString != "") {
                JSONObject additionalDataJSON = new JSONObject(additionalDataString);

                try {
                    customsJson = SwUtils.merge(customsJson, additionalDataJSON);
                } catch (JSONException e) {
                    SdkLogger.error(this, "Error merging additional data json\nexception: " + e);
                }
            }

            JSONObject event = SwUtils.createEvent(SESSION_EVENT_NAME, getData(), mConversionDataManager.getConversionData(), mEventMetadataManager.get(), customsJson.toString(), "");
            mEventsReporter.reportEvent(event);
            
            onSessionEnded(mCurrentSessionId);
            resetSession();
            mEventsQueue.stopQueue();
        } catch (Exception e) {
            SdkLogger.error(this, "End session error\nexception: " + e);
        }
    }

    @Override
    public void registerSessionListener(ISessionListener listener) {
        mSessionListeners.add(listener);
    }

    @Override
    public void unregisterSessionListener(ISessionListener listener) {
        mSessionListeners.remove(listener);
    }

    @Override
    public void unregisterAllSessionListeners() {
        mSessionListeners.clear();
    }

    @Override
    public String getMegaSessionId() {
        return MEGA_SESSION_ID;
    }

    @Override
    public JSONObject getData() {
        JSONObject data = new JSONObject();
        SwUtils.addToJson(data, Constants.KEY_MEGA_SESSION_ID, MEGA_SESSION_ID);
        SwUtils.addToJson(data, Constants.KEY_SESSION_ID, mCurrentSessionId);
        SwUtils.addToJson(data, Constants.KEY_MEGA_SESSION_COUNTER, mMegaSessionsCounter);
        SwUtils.addToJson(data, Constants.KEY_SESSION_COUNTER, mTotalSessionsCounter);
        SwUtils.addToJson(data, Constants.KEY_SESSION_IN_MEGA_COUNTER, mSessionsInMegaSessionCounter);
        
        return data;
    }

    @Override
    public void initializeSession(String metadata) {
        mIsSessionInitialized = true;
        mEventMetadataManager.set(metadata);
        startSession();
    }

    @Override
    public void onSessionStarted(String sessionId) {
        for (ISessionListener listener : mSessionListeners) {
            if (listener != null) {
                listener.onSessionStarted(sessionId);
            }
        }
    }

    @Override
    public String getAdditionalDataJsonMethod() {
        for (ISessionListener listener : mSessionListeners) {
            if (listener != null) {
                return listener.getAdditionalDataJsonMethod();
            }
        }
        
        return "";
    }

    @Override
    public void onSessionEnded(String sessionId) {
        for (ISessionListener listener : mSessionListeners) {
            if (listener != null) {
                listener.onSessionEnded(sessionId);
            }
        }
    }

    @Override
    public void onAppMovedToForeground() {
        if (mIsSessionInitialized && !isSessionStarted()) {
            startSession();
        }
    }

    @Override
    public void onAppMovedToBackground() {
        if (mIsSessionInitialized && isSessionStarted()) {
            endSession();
        }
    }

    private static long currentTimeSeconds() {
        return TimeUnit.MILLISECONDS.toSeconds(System.currentTimeMillis());
    }
}
