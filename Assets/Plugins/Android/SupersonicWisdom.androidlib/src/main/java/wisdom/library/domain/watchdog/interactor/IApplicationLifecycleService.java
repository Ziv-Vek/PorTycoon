package wisdom.library.domain.watchdog.interactor;

public interface IApplicationLifecycleService {

    void startWatch();
    void stopWatch();
    void registerWatchdog(IWatchdog listener);
    void unregisterWatchdog(IWatchdog listener);
}
