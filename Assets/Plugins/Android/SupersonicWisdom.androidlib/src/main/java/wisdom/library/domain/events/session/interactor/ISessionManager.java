package wisdom.library.domain.events.session.interactor;

import org.json.JSONObject;

import wisdom.library.domain.events.session.ISessionListener;
import wisdom.library.domain.watchdog.listener.IBackgroundWatchdogListener;

public interface ISessionManager extends IBackgroundWatchdogListener {
    void initializeSession(String metadata);
    void registerSessionListener(ISessionListener listener);
    void unregisterSessionListener(ISessionListener listener);
    void unregisterAllSessionListeners();
    String getMegaSessionId();
    JSONObject getData();
}
