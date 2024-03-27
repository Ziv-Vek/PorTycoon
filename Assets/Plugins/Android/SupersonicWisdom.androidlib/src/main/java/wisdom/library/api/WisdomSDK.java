package wisdom.library.api;

import android.app.Activity;

import wisdom.library.api.listener.IWisdomConnectivityListener;
import wisdom.library.api.listener.IWisdomInitListener;
import wisdom.library.api.listener.IWisdomRequestListener;
import wisdom.library.api.listener.IWisdomSessionListener;
import wisdom.library.api.dto.WisdomConfigurationDto;

public class WisdomSDK {

    private static IWisdomSDK sInstance;

    private WisdomSDK() {}

    private static IWisdomSDK getInstance() {
        if (sInstance == null) {
            synchronized (WisdomSDK.class) {
                if (sInstance == null) {
                    sInstance = new WisdomSDKImpl();
                }
            }
        }

        return sInstance;
    }

    public static void init(Activity initialActivity, WisdomConfigurationDto config) {
        getInstance().init(initialActivity, config);
    }

    /**
     * Will create and send first session start
     * @param metadata
     */
    public static void initializeSession(String metadata) {
        if (getInstance().isInitialized()) {
            getInstance().initializeSession(metadata);
        }
    }

    /**
     * Will show or hide a loader view, that will block all user interaction.
     *
     * @param shouldPresent Determines whether the SDK should show / hide the loader
     */
    public static boolean toggleBlockingLoader(boolean shouldPresent) {
        if (getInstance().isInitialized()) {
            return getInstance().toggleBlockingLoader(shouldPresent);
        }

        return false;
    }

    public static void updateWisdomConfiguration(WisdomConfigurationDto config) {
        if (getInstance().isInitialized()) {
            getInstance().updateWisdomConfiguration(config);
        }
    }

    public static boolean isInitialized() {
        return getInstance().isInitialized();
    }

    public static void setEventMetadata(String metadata) {
        if (getInstance().isInitialized()) {
            getInstance().setEventMetadata(metadata);
        }
    }

    public static void updateEventsMetadata(String metadata) {
        if (getInstance().isInitialized()) {
            getInstance().updateEventMetadata(metadata);
        }
    }

    public static String getAdvertisingIdentifier() {
        String advertisingIdentifier = "";
        if (getInstance().isInitialized()) {
            advertisingIdentifier = getInstance().getAdvertisingIdentifier();
        }

        return advertisingIdentifier;
    }
    
    public static String getAppSetIdentifier() {
        String appSetIdentifier = "";
        if (getInstance().isInitialized()) {
            appSetIdentifier = getInstance().getAppSetIdentifier();
        }

        return appSetIdentifier;
    }

    public static String getMegaSessionId() {
        String megaSessionId = "";
        if (getInstance().isInitialized()) {
            megaSessionId = getInstance().getMegaSessionId();
        }

        return megaSessionId;
    }
    
    public static String getAppInstallSource() {
        String appInstallSource = "";
        if (getInstance().isInitialized()) {
            appInstallSource = getInstance().getAppInstallSource();
        }

        return appInstallSource;
    }

    public static void trackEvent(String eventName, String customsJson, String extraJson) {
        if (getInstance().isInitialized()) {
            getInstance().trackEvent(eventName, customsJson, extraJson);
        }
    }

    public static void registerInitListener(IWisdomInitListener listener) {
        getInstance().registerInitListener(listener);
    }

    public static void unregisterInitListener(IWisdomInitListener listener) {
        getInstance().unregisterInitListener(listener);
    }

    public static void registerSessionListener(IWisdomSessionListener listener) {
        if (getInstance().isInitialized()) {
            getInstance().registerSessionListener(listener);
        }
    }

    public static void unregisterSessionListener(IWisdomSessionListener listener) {
        if (getInstance().isInitialized()) {
            getInstance().unregisterSessionListener(listener);
        }
    }

    public static void registerWebRequestListener(IWisdomRequestListener listener) {
        if (getInstance().isInitialized()) {
            getInstance().registerWebRequestListener(listener);
        }
    }
    
    public static void unregisterWebRequestListener(IWisdomRequestListener listener) {
        if (getInstance().isInitialized()) {
            getInstance().unregisterWebRequestListener(listener);
        }
    }

    public static void destroy() {
        if (getInstance().isInitialized()) {
            getInstance().destroy();
        }
    }

    public static void sendRequest(String requestJsonString) {
        if (getInstance().isInitialized()) {
            getInstance().sendRequest(requestJsonString);
        }
    }
    
    public static void requestRateUsPopup() {
        if (getInstance().isInitialized()) {
            getInstance().requestRateUsPopup();
        }
    }

    public static void registerConnectivityListener(IWisdomConnectivityListener listener) {
        if (getInstance().isInitialized()) {
            getInstance().registerConnectivityListener(listener);
        }
    }

    public static void unregisterConnectivityListener(IWisdomConnectivityListener listener){
        if (getInstance().isInitialized()) {
            getInstance().unregisterConnectivityListener(listener);
        }
    }
    
    public static String getConnectionStatus(){
        String networkStatusJson = "";
        
        if (getInstance().isInitialized()) {
             networkStatusJson = getInstance().getConnectionStatus();
        }
        
        return networkStatusJson;
    }
}
