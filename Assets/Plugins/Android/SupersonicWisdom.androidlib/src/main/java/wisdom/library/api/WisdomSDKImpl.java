package wisdom.library.api;

import android.app.Activity;
import android.app.Application;
import android.content.Context;
import android.content.SharedPreferences;
import android.os.Handler;

import wisdom.library.IdentifiersGetter;
import wisdom.library.PackageManagement;
import wisdom.library.api.listener.IWisdomConnectivityListener;
import wisdom.library.api.listener.IWisdomInitListener;
import wisdom.library.api.listener.IWisdomRequestListener;
import wisdom.library.api.listener.IWisdomSessionListener;
import wisdom.library.data.framework.events.EventsQueue;
import wisdom.library.data.framework.local.ConversionDataLocalApi;
import wisdom.library.data.framework.local.EventsLocalApi;
import wisdom.library.data.framework.local.storage.api.IWisdomStorage;
import wisdom.library.data.framework.local.storage.api.WisdomDbHelper;
import wisdom.library.data.framework.local.storage.core.WisdomEventsStorage;
import wisdom.library.data.framework.network.core.WisdomNetwork;
import wisdom.library.data.framework.network.core.WisdomNetworkDispatcher;
import wisdom.library.data.framework.network.utils.NetworkUtils;
import wisdom.library.data.framework.watchdog.ApplicationLifecycleService;
import wisdom.library.data.framework.local.EventMetadataLocalApi;
import wisdom.library.data.framework.network.api.INetwork;
import wisdom.library.data.repository.ConversionDataRepository;
import wisdom.library.data.repository.datasource.ConversionDataLocalDataSource;
import wisdom.library.data.repository.datasource.EventsLocalDataSource;
import wisdom.library.domain.events.IEventsQueue;
import wisdom.library.domain.events.reporter.interactor.EventsReporter;
import wisdom.library.domain.events.IConversionDataRepository;
import wisdom.library.domain.events.reporter.interactor.IEventsReporter;
import wisdom.library.domain.events.interactor.ConversionDataManager;
import wisdom.library.domain.events.interactor.IConversionDataManager;
import wisdom.library.data.framework.remote.EventsRemoteApi;
import wisdom.library.data.repository.EventMetadataRepository;
import wisdom.library.data.repository.EventsRepository;
import wisdom.library.data.repository.datasource.EventMetadataLocalDataSource;
import wisdom.library.data.repository.datasource.EventsRemoteDataSource;
import wisdom.library.domain.events.IEventsRepository;
import wisdom.library.domain.events.interactor.EventMetadataManager;
import wisdom.library.domain.events.interactor.IEventMetadataManager;
import wisdom.library.domain.events.session.interactor.SessionManager;
import wisdom.library.domain.events.session.interactor.ISessionManager;
import wisdom.library.domain.mapper.ListStoredEventJsonMapper;
import wisdom.library.data.framework.local.mapper.StoredEventMapper;
import wisdom.library.domain.watchdog.BackgroundWatchdog;
import wisdom.library.api.dto.WisdomConfigurationDto;
import wisdom.library.domain.watchdog.interactor.ApplicationLifecycleServiceRegistrar;
import wisdom.library.domain.watchdog.interactor.BackgroundWatchdogRegistrar;
import wisdom.library.domain.watchdog.interactor.IApplicationLifecycleService;
import wisdom.library.domain.watchdog.interactor.IBackgroundWatchdog;
import wisdom.library.ui.BlockingFullScreenLoader;
import wisdom.library.util.SdkLogger;
import wisdom.library.util.SwAndroidTaskWrapper;
import wisdom.library.util.SwCallback;
import wisdom.library.util.SwUtils;

import org.json.JSONObject;

import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.List;

class WisdomSDKImpl implements IWisdomSDK {

    private static final String WISDOM_PREFS_NAME = "WisdomEventsPreferences";
    private static final String TAG = WisdomSDKImpl.class.getSimpleName();

    private Application mApplication;
    private IApplicationLifecycleService mAppLifecycleService;
    private IBackgroundWatchdog mBgWatchdog;

    private List<IWisdomInitListener> mInitListeners = new ArrayList<>();

    private BackgroundWatchdogRegistrar mBackgroundWatchdogRegistrar;
    private ApplicationLifecycleServiceRegistrar mAppLifecycleServiceRegistrar;

    private WisdomNetworkDispatcher mDispatcher;
    private INetwork mNetwork;
    private NetworkUtils mNetworkUtils;

    private EventsRemoteApi mEventsRemoteApi;
    private EventsRemoteDataSource mEventsRemoteDataSource;
    private IEventsRepository mEventsRepository;

    private EventMetadataLocalApi mEventMetadataLocalApi;
    private EventMetadataLocalDataSource mEventMetadataLocalDataSource;
    private EventMetadataRepository mEventMetadataRepository;

    private ConversionDataLocalApi mConversionDataLocalApi;
    private ConversionDataLocalDataSource mConversionDataLocalDataSource;
    private IConversionDataRepository mConversionDataRepository;

    private ISessionManager mSessionManager;
    private IEventMetadataManager mEventMetadataManager;
    private IConversionDataManager mConversionDatManager;

    private SharedPreferences mPrefs;
    private SharedPreferences mUnityPrefs;

    private boolean mIsInitialized = false;

    private WisdomDbHelper mDbHelper;
    private IWisdomStorage mWisdomEventsStorage;
    private EventsLocalApi mEventsLocalApi;
    private EventsLocalDataSource mEventsLocalDatSource;
    private IEventsReporter mEventsReporter;
    private IEventsQueue mEventsQueue;

    private BlockingFullScreenLoader fullScreenLoader;
    private Handler unityMainThreadHandler;
    private WisdomRequestManager mRequestManager;
    
    private Activity mActivity;
    private PackageManagement mPackageManagement;

    @Override
    public void init(Activity initialActivity, WisdomConfigurationDto config) {
        SdkLogger.setup(config.isLoggingEnabled);

        SharedPreferences sharedPref = initialActivity.getPreferences(Context.MODE_PRIVATE);
        unityMainThreadHandler = new Handler();
        mApplication = initialActivity.getApplication();
        mPrefs = initialActivity.getSharedPreferences(WISDOM_PREFS_NAME, Context.MODE_PRIVATE);
        mUnityPrefs = initialActivity.getSharedPreferences(initialActivity.getPackageName() + ".v2.playerprefs", Context.MODE_PRIVATE);

        mAppLifecycleService = new ApplicationLifecycleService(mApplication);
        mAppLifecycleServiceRegistrar = new ApplicationLifecycleServiceRegistrar(mAppLifecycleService);

        mBgWatchdog = new BackgroundWatchdog(initialActivity.getLocalClassName());
        mBackgroundWatchdogRegistrar = new BackgroundWatchdogRegistrar(mBgWatchdog);

        mAppLifecycleServiceRegistrar.registerWatchdog(mBgWatchdog);
        mAppLifecycleServiceRegistrar.startService();

        mNetworkUtils = new NetworkUtils(mApplication);
        mDispatcher = new WisdomNetworkDispatcher();
        mNetwork = new WisdomNetwork(mDispatcher, config.connectTimeout, config.readTimeout, mNetworkUtils);

        ListStoredEventJsonMapper listJsonMapper = new ListStoredEventJsonMapper();

        mEventsRemoteApi = new EventsRemoteApi(mNetwork, config.subdomain, listJsonMapper);
        mEventsRemoteDataSource = new EventsRemoteDataSource(mEventsRemoteApi);

        StoredEventMapper storedEventMapper = new StoredEventMapper();
        mDbHelper = new WisdomDbHelper(mApplication.getApplicationContext());
        mWisdomEventsStorage = new WisdomEventsStorage(mDbHelper);
        mEventsLocalApi = new EventsLocalApi(mWisdomEventsStorage, storedEventMapper);
        mEventsLocalDatSource = new EventsLocalDataSource(mEventsLocalApi);

        mEventsRepository = new EventsRepository(mEventsRemoteDataSource, mEventsLocalDatSource);

        mEventsQueue = new EventsQueue(mEventsRepository, config.initialSyncInterval);
        mEventsQueue.startQueue();

        mEventMetadataLocalApi = new EventMetadataLocalApi(mPrefs);
        mEventMetadataLocalDataSource = new EventMetadataLocalDataSource(mEventMetadataLocalApi);

        mEventMetadataRepository = new EventMetadataRepository(mEventMetadataLocalDataSource);
        mEventMetadataManager = new EventMetadataManager(mEventMetadataRepository);

        mConversionDataLocalApi = new ConversionDataLocalApi(mUnityPrefs);
        mConversionDataLocalDataSource = new ConversionDataLocalDataSource(mConversionDataLocalApi);
        mConversionDataRepository = new ConversionDataRepository(mConversionDataLocalDataSource);
        mConversionDatManager = new ConversionDataManager(mConversionDataRepository);

        fullScreenLoader = new BlockingFullScreenLoader(initialActivity, config.streamingAssetsFolderPath + "/" + config.blockingLoaderResourceRelativePath, config.blockingLoaderViewportPercentage);
        
        mPackageManagement = new PackageManagement(initialActivity.getApplicationContext());
        
        mEventsReporter = new EventsReporter(mEventsRepository);
        mSessionManager = new SessionManager(mEventsReporter, mEventMetadataManager, mConversionDatManager, mEventsQueue, sharedPref);
        
        mRequestManager = new WisdomRequestManager(mNetwork, mNetworkUtils);


        IdentifiersGetter.fetch(initialActivity, new SwCallback<IdentifiersGetter.GetterResults>() {
            @Override
            public void onDone(IdentifiersGetter.GetterResults result) {
                if (result == null) {
                    SdkLogger.error(TAG, "Failed to fetch app set ID + advertising ID");
                } else {
                    String appSetId = result.getAppSetIdentifier();
                    String advertisingId = result.getAdvertisingIdentifier();
                    SdkLogger.log("Got app set ID '" + appSetId + "' from calling via " + TAG);
                    SdkLogger.log("Got advertising ID '" + advertisingId + "' from calling via " + TAG);
                }

                mIsInitialized = true;

                for (IWisdomInitListener initListener : mInitListeners) {
                    initListener.onInitEnded();
                }
            }
        });
        
        mActivity = initialActivity;
    }

    @Override
    public boolean isInitialized() {
        return mIsInitialized;
    }

    @Override
    public void initializeSession(String metadata) {
        mBackgroundWatchdogRegistrar.registerWatchdogListener(mSessionManager);
        mSessionManager.initializeSession(metadata);
        setEventMetadata(metadata);
    }

    @Override
    public boolean toggleBlockingLoader(boolean shouldPresent) {
        boolean didToggle;
        if (shouldPresent) {
            didToggle = fullScreenLoader.show(unityMainThreadHandler);
        } else {
            didToggle = fullScreenLoader.hide();
        }

        return didToggle;
    }

    @Override
    public void updateWisdomConfiguration(WisdomConfigurationDto config) {
        mNetwork.setConnectTimeout(config.connectTimeout);
        mNetwork.setReadTimeout(config.readTimeout);
        mEventsQueue.setInitialSyncInterval(config.initialSyncInterval);
    }

    @Override
    public void setEventMetadata(String metadataJson) {
        mEventMetadataManager.set(metadataJson);
    }

    @Override
    public void updateEventMetadata(String metadataJson) {
        mEventMetadataManager.update(metadataJson);
    }

    @Override
    public void trackEvent(String eventName, String customsJson, String extraJson) {
        String metaDataJson = (mEventMetadataManager.get());

        JSONObject eventJson = SwUtils.createEvent(eventName, mSessionManager.getData(), mConversionDatManager.getConversionData(), metaDataJson, customsJson, extraJson);

        mEventsReporter.reportEvent(eventJson);
    }

    @Override
    public void registerInitListener(IWisdomInitListener listener) {
        mInitListeners.add(listener);
    }

    @Override
    public void unregisterInitListener(IWisdomInitListener listener) {
        mInitListeners.remove(listener);
    }

    @Override
    public void registerSessionListener(IWisdomSessionListener listener) {
        mSessionManager.registerSessionListener(listener);
    }

    @Override
    public void unregisterSessionListener(IWisdomSessionListener listener) {
        mSessionManager.unregisterSessionListener(listener);
    }
    
    @Override
    public void sendRequest(String request) {
        mRequestManager.sendRequest(request);
    }

    @Override
    public void registerWebRequestListener(IWisdomRequestListener listener) {
        mRequestManager.registerWebRequestListener(listener);
    }
    
    @Override
    public void unregisterWebRequestListener(IWisdomRequestListener listener) {
        mRequestManager.unregisterWebRequestListener(listener);
    }

    @Override
    public void registerConnectivityListener(IWisdomConnectivityListener listener) {
        mNetworkUtils.registerToNetworkChanges(listener);
    }

    @Override
    public void unregisterConnectivityListener(IWisdomConnectivityListener listener) {
        mNetworkUtils.unregisterToNetworkChanges(listener);
    }

    @Override
    public String getConnectionStatus() {
        JSONObject connectionStatusJson = new JSONObject();
        SwUtils.addToJson(connectionStatusJson, NetworkUtils.KEY_IS_AVAILABLE, mNetworkUtils.isAvailable);
        SwUtils.addToJson(connectionStatusJson, NetworkUtils.KEY_IS_FLIGHT_MODE, mNetworkUtils.isFlightMode());
        
        return connectionStatusJson.toString();
    }

    @Override
    public String getAdvertisingIdentifier() {
        return IdentifiersGetter.getAdvertisingId();
    }

    @Override
    public String getAppSetIdentifier() {
        return IdentifiersGetter.getAppSetId();
    }

    @Override
    public String getMegaSessionId() {
        return mSessionManager.getMegaSessionId();
    }
    
    @Override
    public String getAppInstallSource() { 
        return mPackageManagement.getAppInstallerSource(); 
    }

    @Override
    public void destroy() {
        mIsInitialized = false;
        mEventsQueue.stopQueue();

        mAppLifecycleServiceRegistrar.stopService();
        mBackgroundWatchdogRegistrar.unregisterAllWatchdogs();
        mSessionManager.unregisterAllSessionListeners();

        mAppLifecycleServiceRegistrar = null;
        mBackgroundWatchdogRegistrar = null;
        mEventsRemoteApi = null;
        mEventsRemoteDataSource = null;
        mEventsRepository = null;
        mEventMetadataLocalApi = null;
        mEventMetadataLocalDataSource = null;
        mEventMetadataRepository = null;
        mEventMetadataManager = null;
        mSessionManager = null;
        mBgWatchdog = null;
    }

    public void requestRateUsPopup() {
        try {
            final Class<?> reviewManagerFactoryClass = Class.forName("com.google.android.play.core.review.ReviewManagerFactory");
            final Method createMethod = reviewManagerFactoryClass.getMethod("create", Context.class);
            final Object reviewManager = createMethod.invoke(null, mApplication.getApplicationContext());

            final Class<?> reviewManagerClass = Class.forName("com.google.android.play.core.review.ReviewManager");
            final Method requestMethod = reviewManagerClass.getMethod("requestReviewFlow");
            final Object reviewInfoRequest = requestMethod.invoke(reviewManager);

            final Class<?> taskClass = Class.forName("com.google.android.gms.tasks.Task");
            final Method isSuccessfulMethod = taskClass.getMethod("isSuccessful");

            // Wait for completion
            new SwAndroidTaskWrapper<Object>(reviewInfoRequest, new SwAndroidTaskWrapper.IResultExtractor<Object>() {
                @Override
                public Object getResult(Object taskResultObject) throws ReflectiveOperationException {
                    final Boolean isSuccessful = (Boolean) isSuccessfulMethod.invoke(reviewInfoRequest);
                    
                    if (isSuccessful != null && isSuccessful) {
                        final Method getResultMethod = taskClass.getMethod("getResult");
                        return getResultMethod.invoke(reviewInfoRequest);
                    }
                    
                    return null;
                }
            }, new SwCallback<Object>() {
                @Override
                public void onDone(Object reviewInfo) {
                    try {
                        if (reviewInfo != null) {
                            // After this line is called - we should see the popup ...
                            final Method launchReviewFlowMethod = reviewManagerClass.getMethod("launchReviewFlow", Activity.class, Class.forName("com.google.android.play.core.review.ReviewInfo"));
                            // ... Rate us popup should appear
                            
                            final Object launchReviewFlowTask = launchReviewFlowMethod.invoke(reviewManager, mActivity, reviewInfo);
                            
                            // Wait for completion
                            new SwAndroidTaskWrapper<Boolean>(launchReviewFlowTask, new SwAndroidTaskWrapper.IResultExtractor<Boolean>() {
                                @Override
                                public Boolean getResult(Object taskResultObject) throws ReflectiveOperationException {
                                    final Boolean isSuccessful = (Boolean) isSuccessfulMethod.invoke(launchReviewFlowTask);

                                    return isSuccessful != null && isSuccessful;
                                }
                            }, new SwCallback<Boolean>() {
                                @Override
                                public void onDone(Boolean result) {
                                    SdkLogger.log("Request RateUs (in-app review) has " + ((result != null && result) ? "succeeded" : "failed"));
                                }
                            });
                            
                            SdkLogger.log("Request RateUs (in-app review) has launched");
                        } else {
                            SdkLogger.error(TAG, "Request RateUs (in-app review) has failed");
                        }
                    } catch (Exception e) {
                        SdkLogger.error(TAG, "Failed to invoke `launchReviewFlow`", e);
                    }
                }
            });
        } catch (Exception e) {
            SdkLogger.error(TAG, "Error using reflection: ", e);
        }
    }
}

