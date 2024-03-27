package wisdom.library.domain.watchdog.interactor;

import wisdom.library.domain.watchdog.listener.IBackgroundWatchdogListener;

public abstract class IBackgroundWatchdog implements IWatchdog {

    protected abstract void registerListener(IBackgroundWatchdogListener listener);
    protected abstract void unregisterListener(IBackgroundWatchdogListener listener);
    protected abstract void unregisterAllListeners();
}
