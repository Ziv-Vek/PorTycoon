package wisdom.library.data.framework.network.utils;

import android.content.Context;
import android.net.ConnectivityManager;
import android.net.Network;
import android.net.NetworkCapabilities;
import android.net.NetworkInfo;
import android.net.NetworkRequest;
import android.os.Build;
import android.provider.Settings;

import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import wisdom.library.api.listener.IWisdomConnectivityListener;
import wisdom.library.util.SdkLogger;
import wisdom.library.util.SwUtils;

public class NetworkUtils {
    public static final String KEY_IS_AVAILABLE = "isAvailable";
    public static final String KEY_IS_FLIGHT_MODE = "isFlightMode";
    
    private Context _context;

    public boolean isAvailable;
    private List<IWisdomConnectivityListener> mConnectivityListeners = Collections.synchronizedList(new ArrayList<IWisdomConnectivityListener>());
    
    private ConnectivityManager.NetworkCallback networkCallback = new ConnectivityManager.NetworkCallback() {
        @Override
        public void onAvailable(Network network) {
            super.onAvailable(network);
            isAvailable = true;
            notifyConnectivityListeners();
            SdkLogger.log("Network is available!");
        }

        @Override
        public void onLost( Network network) {
            super.onLost(network);
            isAvailable = false;
            notifyConnectivityListeners();
            
            SdkLogger.log("Network is unavailable!");
        }
    };

    NetworkRequest networkRequest = new NetworkRequest.Builder()
        .addCapability(NetworkCapabilities.NET_CAPABILITY_INTERNET)
        .addTransportType(NetworkCapabilities.TRANSPORT_WIFI)
        .addTransportType(NetworkCapabilities.TRANSPORT_CELLULAR)
        .build();

    public boolean isFlightMode(){
        return Settings.System.getInt(_context.getContentResolver(),
            Settings.Global.AIRPLANE_MODE_ON, 0) != 0;
    }

    public NetworkUtils(Context context) {
        this._context = context.getApplicationContext();
        
        mConnectivityListeners = new ArrayList<IWisdomConnectivityListener>();
        registerToConnectivityManager();
    }

    private void registerToConnectivityManager() {
        ConnectivityManager connectivityManager = (ConnectivityManager) _context.getSystemService(Context.CONNECTIVITY_SERVICE);
        connectivityManager.registerNetworkCallback(networkRequest, networkCallback);
    }

    public boolean isNetworkAvailable() {
        ConnectivityManager connectivityManager = (ConnectivityManager) _context.getSystemService(Context.CONNECTIVITY_SERVICE);
        
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            Network network = connectivityManager.getActiveNetwork();
            if (network == null) {
                return false;
            }

            NetworkCapabilities capabilities = connectivityManager.getNetworkCapabilities(network);
            if (capabilities == null) {
                return false;
            }

            boolean isWifiNetwork = capabilities.hasTransport(NetworkCapabilities.TRANSPORT_WIFI) ;
            boolean isCellular = capabilities.hasTransport(NetworkCapabilities.TRANSPORT_CELLULAR);
            boolean isEthernet = capabilities.hasTransport(NetworkCapabilities.TRANSPORT_ETHERNET);
            boolean internetReachable = capabilities.hasCapability(NetworkCapabilities.NET_CAPABILITY_INTERNET);

            return ((isWifiNetwork || isCellular || isEthernet) && internetReachable);
        } else {
            NetworkInfo networkInfo = connectivityManager.getActiveNetworkInfo();
            return (networkInfo != null && networkInfo.isConnected());
        }
    }

    public void registerToNetworkChanges(IWisdomConnectivityListener listener) {
        synchronized(mConnectivityListeners) {
            mConnectivityListeners.add(listener);
        }
    }

    public void unregisterToNetworkChanges(IWisdomConnectivityListener listener) {
        synchronized(mConnectivityListeners) {
            mConnectivityListeners.remove(listener);
        }
    }
    
    private void notifyConnectivityListeners() {
        if (mConnectivityListeners == null) return;
        
        synchronized(mConnectivityListeners) {
            for (IWisdomConnectivityListener listener : mConnectivityListeners) {
                if (listener != null) {
                    JSONObject connection = new JSONObject();
                    SwUtils.addToJson(connection, NetworkUtils.KEY_IS_AVAILABLE, isAvailable);
                    SwUtils.addToJson(connection, NetworkUtils.KEY_IS_FLIGHT_MODE, isFlightMode());
                    listener.onConnectionStatusChanged(connection.toString());
                }
            }
        }
    }
}
