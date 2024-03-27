package wisdom.library.domain.watchdog.interactor;

public class ApplicationLifecycleServiceRegistrar {

    IApplicationLifecycleService mService;
    public ApplicationLifecycleServiceRegistrar(IApplicationLifecycleService watchdog) {
        mService = watchdog;
    }

    public void startService() {
        mService.startWatch();
    }

    public void stopService() {
        mService.stopWatch();
    }

    public void registerWatchdog(IWatchdog watchdog) {
        mService.registerWatchdog(watchdog);
    }

    public void unregisterWatchdog(IWatchdog watchdog) {
        mService.unregisterWatchdog(watchdog);
    }
}
