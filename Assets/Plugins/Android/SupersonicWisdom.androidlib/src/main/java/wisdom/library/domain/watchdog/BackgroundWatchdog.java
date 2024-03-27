package wisdom.library.domain.watchdog;

import wisdom.library.domain.watchdog.dto.ActivityDto;
import wisdom.library.domain.watchdog.dto.BundleDto;
import wisdom.library.domain.watchdog.interactor.IBackgroundWatchdog;
import wisdom.library.domain.watchdog.listener.IBackgroundWatchdogListener;

import java.util.ArrayList;
import java.util.List;

public class BackgroundWatchdog extends IBackgroundWatchdog {

    /**
     * Describes application state(background/foreground)
     */
    private enum AppState {
        /**
         * App moved to Background
         */
        MOVE_TO_BG,

        /**
         * App moved to Foreground
         */
        MOVE_TO_FG
    }

    private List<IBackgroundWatchdogListener> mListeners;
    private String mInitialActivity;
    private boolean mAppIsForeground;

    boolean mUsedStopped = false;
    boolean mUsedResumed = false;

    public BackgroundWatchdog(String initialActivity) {
        mInitialActivity = initialActivity;
        mListeners = new ArrayList<>(1);
        mAppIsForeground = true;
    }

    @Override
    public void registerListener(IBackgroundWatchdogListener listener) {
        mListeners.add(listener);
    }

    @Override
    public void unregisterListener(IBackgroundWatchdogListener listener) {
        mListeners.remove(listener);
    }

    @Override
    protected void unregisterAllListeners() {
        mListeners.clear();
    }

    @Override
    public void onActivityCreated(ActivityDto activity, BundleDto bundle) { }

    @Override
    public void onActivityStarted(ActivityDto activity) { }

    @Override
    public void onActivityResumed(ActivityDto activity) {
        mUsedResumed = true;
        if (mUsedStopped) {
            handleApplicationState(AppState.MOVE_TO_FG);
        }
        mUsedStopped = false;
    }

    @Override
    public void onActivityPaused(ActivityDto activity) {
        mUsedResumed = false;
        mUsedStopped = false;
    }

    @Override
    public void onActivityStopped(ActivityDto activity) {
        mUsedStopped = true;
        if (!mUsedResumed) {
            handleApplicationState(AppState.MOVE_TO_BG);
        }
    }

    @Override
    public void onActivitySaveInstanceState(ActivityDto activity, BundleDto bundle) { }

    @Override
    public void onActivityDestroyed(ActivityDto activity) { }

    /**
     * Handles Application State to notify listeners.
     * @param state indicate to which state an application will enter
     */
    private void handleApplicationState(AppState state) {
        mAppIsForeground = (AppState.MOVE_TO_FG == state);
        if (mAppIsForeground) {
            for (IBackgroundWatchdogListener listener : mListeners) {
                listener.onAppMovedToForeground();
            }
        } else {
            for (IBackgroundWatchdogListener listener : mListeners) {
                listener.onAppMovedToBackground();
            }
        }
    }
}
