package wisdom.library.api;

import android.app.Activity;

import wisdom.library.api.listener.IWisdomConnectivityListener;
import wisdom.library.api.listener.IWisdomInitListener;
import wisdom.library.api.listener.IWisdomRequestListener;
import wisdom.library.api.listener.IWisdomSessionListener;
import wisdom.library.api.dto.WisdomConfigurationDto;

interface IWisdomSDK {

    void init(Activity initialActivity, WisdomConfigurationDto config);
    boolean isInitialized();
    void initializeSession(String metadata);
    boolean toggleBlockingLoader(boolean shouldPresent);
    void updateWisdomConfiguration(WisdomConfigurationDto config);
    void registerInitListener(IWisdomInitListener listener);
    void unregisterInitListener(IWisdomInitListener listener);
    void registerSessionListener(IWisdomSessionListener listener);
    void unregisterSessionListener(IWisdomSessionListener listener);
    void setEventMetadata(String metadata);
    void updateEventMetadata(String metadata);
    void trackEvent(String eventName, String customsJson, String extraJson);
    String getAdvertisingIdentifier();
    String getAppSetIdentifier();
    String getMegaSessionId();
    String getAppInstallSource();
    void destroy();
    void sendRequest(String request);
    void registerWebRequestListener(IWisdomRequestListener listener);
    void unregisterWebRequestListener(IWisdomRequestListener listener);
    void requestRateUsPopup();
    void registerConnectivityListener(IWisdomConnectivityListener listener);
    void unregisterConnectivityListener(IWisdomConnectivityListener listener);
    String getConnectionStatus();
}
