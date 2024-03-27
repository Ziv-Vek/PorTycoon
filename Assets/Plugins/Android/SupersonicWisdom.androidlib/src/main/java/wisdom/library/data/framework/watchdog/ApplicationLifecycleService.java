package wisdom.library.data.framework.watchdog;

import android.app.Activity;
import android.app.Application;
import android.os.Bundle;

import wisdom.library.domain.watchdog.dto.BundleDto;
import wisdom.library.domain.watchdog.interactor.IApplicationLifecycleService;
import wisdom.library.domain.watchdog.interactor.IWatchdog;

import java.util.ArrayList;
import java.util.List;

public class ApplicationLifecycleService implements IApplicationLifecycleService, Application.ActivityLifecycleCallbacks {

    private List<IWatchdog> mWatchdogs;
    private Application mApplication;

    public ApplicationLifecycleService(Application application) {
        mWatchdogs = new ArrayList<>(1);
        mApplication = application;
    }

    @Override
    public void startWatch() {
        if (mApplication != null) {
            mApplication.registerActivityLifecycleCallbacks(this);
        }
    }

    public void stopWatch() {
        unregisterAllWatchdogs();
        if (mApplication != null) {
            mApplication.unregisterActivityLifecycleCallbacks(this);
        }
    }

    @Override
    public void registerWatchdog(IWatchdog watchdog) {
        mWatchdogs.add(watchdog);
    }

    @Override
    public void unregisterWatchdog(IWatchdog watchdog) {
        mWatchdogs.remove(watchdog);
    }

    private void unregisterAllWatchdogs() {
        mWatchdogs.clear();
    }

    @Override
    public void onActivityCreated(Activity activity, Bundle bundle) {
        for (IWatchdog watchdog : mWatchdogs) {
            if (watchdog != null) {
                watchdog.onActivityCreated(ActivityDtoMapper.map(activity), new BundleDto());
            }
        }
    }

    @Override
    public void onActivityStarted(Activity activity) {
        for (IWatchdog watchdog : mWatchdogs) {
            if (watchdog != null) {
                watchdog.onActivityStarted(ActivityDtoMapper.map(activity));
            }
        }
    }

    @Override
    public void onActivityResumed(Activity activity) {
        for (IWatchdog watchdog : mWatchdogs) {
            if (watchdog != null) {
                watchdog.onActivityResumed(ActivityDtoMapper.map(activity));
            }
        }
    }

    @Override
    public void onActivityPaused(Activity activity) {
        for (IWatchdog watchdog : mWatchdogs) {
            if (watchdog != null) {
                watchdog.onActivityPaused(ActivityDtoMapper.map(activity));
            }
        }
    }

    @Override
    public void onActivityStopped(Activity activity) {
        for (IWatchdog watchdog : mWatchdogs) {
            if (watchdog != null) {
                watchdog.onActivityStopped(ActivityDtoMapper.map(activity));
            }
        }
    }

    @Override
    public void onActivitySaveInstanceState(Activity activity, Bundle bundle) {
        for (IWatchdog watchdog : mWatchdogs) {
            if (watchdog != null) {
                watchdog.onActivitySaveInstanceState(ActivityDtoMapper.map(activity), new BundleDto());
            }
        }
    }

    @Override
    public void onActivityDestroyed(Activity activity) {
        for (IWatchdog watchdog : mWatchdogs) {
            if (watchdog != null) {
                watchdog.onActivityDestroyed(ActivityDtoMapper.map(activity));
            }
        }
    }

}
