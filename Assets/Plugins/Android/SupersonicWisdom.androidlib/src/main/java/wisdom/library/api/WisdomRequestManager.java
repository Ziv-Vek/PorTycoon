package wisdom.library.api;

import static wisdom.library.data.framework.network.core.WisdomNetwork.*;

import wisdom.library.api.listener.IWisdomConnectivityListener;
import wisdom.library.api.listener.IWisdomRequestListener;
import wisdom.library.data.framework.network.api.IInternalRequestListener;
import wisdom.library.data.framework.network.api.INetwork;
import wisdom.library.data.framework.network.request.Method;
import wisdom.library.data.framework.network.request.WisdomRequest;
import wisdom.library.data.framework.network.utils.NetworkUtils;
import wisdom.library.util.SdkLogger;
import wisdom.library.util.SwUtils;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class WisdomRequestManager implements IInternalRequestListener, IWisdomConnectivityListener {
    private static final String KEY = "key";
    private static final String KEY_DATA_STRING = "dataString";
    private static final String KEY_ITERATION = "iteration";
    private static final String KEY_CAP = "cap";
    private static final String KEY_CODE = "code";
    private static final String KEY_ERROR = "error";
    private static final String KEY_IS_DONE = "isDone";
    private static final String KEY_IS_PENDING = "isPending";
    private static final String KEY_IS_REACHED_CAP = "isReachedCap";
    private static final String KEY_TIME = "time";
    private static final int KEY_EXPO_MULTIPLIER = 2;
    private static final int MS_IN_SECOND = 1000;
    private static final int SUCCESS_STATUS_CODE = 200;
    private static final String TAG = WisdomRequestManager.class.getSimpleName();
    
    private final INetwork _network;
    private final NetworkUtils _networkUtils;
    private HashMap<String, WisdomRequest> _inProgressRequests;
    private HashMap<String, WisdomRequest> _waitingForNetworkRequests;
    private List<IWisdomRequestListener> _requestListeners;
    
        public WisdomRequestManager(INetwork network, NetworkUtils networkUtils) {
        _network = network;
        _networkUtils = networkUtils;
        _requestListeners = new ArrayList<>();
        _inProgressRequests =  new HashMap<String, WisdomRequest>();
        _waitingForNetworkRequests = new HashMap<String, WisdomRequest>();
        _networkUtils.registerToNetworkChanges(this);
    }

    public void sendRequest(String request){
        JSONObject requestJson = SwUtils.createJsonOrEmpty(request);

        sendRequest(
        (String) SwUtils.safeGetObject(requestJson, "key"),
            (String) SwUtils.safeGetObject(requestJson, "url"),
            SwUtils.safeGetObject(requestJson, "headers").toString(),
            SwUtils.safeGetObject(requestJson, "body").toString(),
            (int) SwUtils.safeGetObject(requestJson, "connectionTimeout"),
            (int) SwUtils.safeGetObject(requestJson, "readTimeout"),
            (int) SwUtils.safeGetObject(requestJson, "cap"));
    }

    public void sendRequest(String key, String url, String headers, String body, int connectionTimeout, int requestTimeout, int cap) {
        WisdomRequest request = new WisdomRequest(key, url, Method.POST, SwUtils.createJsonOrEmpty(headers), SwUtils.createJsonOrEmpty(body), this, connectionTimeout, requestTimeout, cap);
        
        send(request, 0);
    }

    @Override
    public void onResponseFailed(String key, int iteration, int code, String error) {
        WisdomRequest request = _inProgressRequests.remove(key);

        if (request == null || request.getKey() == null || request.getKey() == "") {
            SdkLogger.log("WisdomRequestManager | could not find listener for request = " + key);
            return;
        }

        if (request.isReachedCap()) {
            invokeCallback(key,"", request.getIteration(), code, error, request.mCap, request.startTime, request.endTime);
        } else {
            invokeCallback(key,"", request.getIteration(), code, error, request.mCap, request.startTime, request.endTime);
            request.increaseIteration();
            send(request, (long)Math.pow(KEY_EXPO_MULTIPLIER, request.getIteration() - 1) * MS_IN_SECOND);
        }
    }

    @Override
    public void onResponseSuccess(String key, int iteration, String body) {
        WisdomRequest request = _inProgressRequests.remove(key);

        if(request == null || request.getKey() == null || request.getKey().isEmpty()) {
            return;
        }
        
        for (IWisdomRequestListener listener : _requestListeners) {
            if (listener != null) {
                removeFromList(key);
                invokeCallback(key, body, request.getIteration(), SUCCESS_STATUS_CODE, "", request.mCap, request.startTime, request.endTime);
            }
        }
    }

    public void registerWebRequestListener(IWisdomRequestListener listener) {
        if(listener == null) return;
        
        _requestListeners.add(listener);
    }

    public void unregisterWebRequestListener(IWisdomRequestListener listener) {
        if(listener == null) return;
        
        _requestListeners.remove(listener);
    }

    @Override
    public void onConnectionStatusChanged(String connectionStatus) {
        JSONObject connectionStatusJson = SwUtils.createJsonOrEmpty(connectionStatus);
        if((boolean) SwUtils.safeGetObject(connectionStatusJson, NetworkUtils.KEY_IS_AVAILABLE)){
            trySendAllPending();
        }

    }

    private void send(final WisdomRequest request, long inMilliseconds) {
        if(!_networkUtils.isAvailable) {
            SdkLogger.log("WisdomRequestManager | send | network unavailable - caching request with key = " + request.getKey());
            _waitingForNetworkRequests.put(request.getKey(), request);
            invokeCallback(request.getKey(), "", request.getIteration(), WISDOM_INTERNAL_NO_INTERNET, WISDOM_NO_INTERNET_CONNECTION_MSG, request.mCap, request.startTime, request.endTime);
            
            return;
        }

        _inProgressRequests.put(request.getKey(), request);
        request.setResponseListener(this);
        sendRequestSchedule(request, inMilliseconds);
    }

    private void sendRequestSchedule(final WisdomRequest request, long inMilliseconds) {
            new java.util.Timer().schedule(
            new java.util.TimerTask() {
                @Override
                public void run() {
                    SdkLogger.log("WisdomRequestManager | send | sending request with key = " + request.getKey());
                    _network.sendAsync(request);
                }
            },
            inMilliseconds
        );
    }

    private void removeFromList(String key) {
        _inProgressRequests.remove(key);
    }

    private void trySendAllPending() {
        if(_waitingForNetworkRequests == null || _waitingForNetworkRequests.isEmpty()) return;
        
        for (String key : _waitingForNetworkRequests.keySet()) {
            WisdomRequest requestStruct = _waitingForNetworkRequests.get(key);
            
            if (requestStruct != null) {
                send(requestStruct, 0);
            }
        }
    }

    private void invokeCallback(String key, String data, int iteration, int code, String error, int cap, long startTime, long endTime){
        if(_requestListeners == null || _requestListeners.isEmpty())  return;
        
        JSONObject responseJson = new JSONObject();
        
        try {
            SdkLogger.log("Network callback | key = " + key + "| code = " + code + "| error = " + error + "| iteration = " + iteration + "| cap = " + cap);
            
            responseJson.put(KEY, key);
            responseJson.put(KEY_ITERATION, iteration);
            responseJson.put(KEY_CAP, cap);
            responseJson.put(KEY_CODE, code);
            responseJson.put(KEY_ERROR, error);
            responseJson.put(KEY_IS_DONE, true);
            responseJson.put(KEY_IS_PENDING, false);
            responseJson.put(KEY_IS_REACHED_CAP, cap == iteration);
            responseJson.put(KEY_TIME, endTime - startTime);
            responseJson.put(KEY_DATA_STRING, data);
            
            for (IWisdomRequestListener listener : _requestListeners) {
                if (listener != null) {
                    listener.onResponse(responseJson.toString());
                }
            }
        } 
        catch (JSONException e) {
            SdkLogger.error(TAG, e);
        }
    }
}
