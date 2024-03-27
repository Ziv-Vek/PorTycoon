package wisdom.library.domain.watchdog.listener;

import wisdom.library.domain.watchdog.dto.ActivityDto;
import wisdom.library.domain.watchdog.dto.BundleDto;

public interface IAppLifecycleListener {
    void onActivityCreated(ActivityDto activity, BundleDto bundle);
    void onActivityStarted(ActivityDto activity);
    void onActivityResumed(ActivityDto activity);
    void onActivityPaused(ActivityDto activity);
    void onActivityStopped(ActivityDto activity);
    void onActivitySaveInstanceState(ActivityDto activity, BundleDto bundle);
    void onActivityDestroyed(ActivityDto activity);
}
