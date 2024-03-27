package wisdom.library.domain.watchdog.interactor;

import wisdom.library.domain.watchdog.listener.IBackgroundWatchdogListener;

public class BackgroundWatchdogRegistrar {

    private IBackgroundWatchdog mWatchdog;

    public BackgroundWatchdogRegistrar(IBackgroundWatchdog watchdog) {
        mWatchdog = watchdog;
    }

    public void registerWatchdogListener(IBackgroundWatchdogListener listener) {
        mWatchdog.registerListener(listener);
    }

    public void unregisterWatchdogListener(IBackgroundWatchdogListener listener) {
        mWatchdog.unregisterListener(listener);
    }

    public void unregisterAllWatchdogs() {
        mWatchdog.unregisterAllListeners();
    }
}
