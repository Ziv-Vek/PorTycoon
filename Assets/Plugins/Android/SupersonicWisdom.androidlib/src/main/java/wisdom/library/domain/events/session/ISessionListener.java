package wisdom.library.domain.events.session;

public interface ISessionListener {
    void onSessionStarted(String sessionId);
    void onSessionEnded(String sessionId);
    String getAdditionalDataJsonMethod();
}
