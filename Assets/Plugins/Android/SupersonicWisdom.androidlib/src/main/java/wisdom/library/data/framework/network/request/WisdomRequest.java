package wisdom.library.data.framework.network.request;

import wisdom.library.data.framework.network.api.IInternalRequestListener;
import wisdom.library.data.framework.network.core.WisdomNetwork;
import wisdom.library.util.SdkLogger;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

public class WisdomRequest{
    private static final String TAG = WisdomRequest.class.getSimpleName();
    
    private String mKey;
    private String mUrl;
    private String mRequestMethod;
    private int mConnectTimeout = WisdomNetwork.DEFAULT_CONNECT_TIMEOUT;
    private int mReadTimeout = WisdomNetwork.DEFAULT_READ_RESPONSE_TIMEOUT;
    private JSONObject mBody;
    private Map<String, String> mHeaders = new HashMap<>();
    public IInternalRequestListener mResponseListener;
    public int mIteration;
    public int mCap;
    public long startTime;
    public long endTime;
    
    public WisdomRequest(String key, String url, Method method, JSONObject body, IInternalRequestListener listener) {
        mResponseListener = listener;
        mKey = key;
        mUrl = url;
        mRequestMethod = method.name().toUpperCase();
        mBody = body;
        mIteration = 0;
        mCap = 0;
    }
    
    public WisdomRequest(String key, String url, Method method, JSONObject headers, JSONObject body, IInternalRequestListener listener, int connectionTimeout, int requestTimeout, int cap) {
        mResponseListener = listener;
        mKey = key;
        mUrl = url;
        mRequestMethod = method.name().toUpperCase();
        mBody = body;
        mIteration = 1;
        mConnectTimeout = connectionTimeout;
        mReadTimeout = requestTimeout;
        mCap = cap;

        parseHeaders(headers);
    }

    private void parseHeaders(JSONObject headers) {
        if(headers == null) return;
        
        Iterator<String> keys = headers.keys();
        while(keys.hasNext()){
            String headerKey = keys.next();
            try {
                setHeader(headerKey, headers.getString(headerKey));
            } catch (JSONException e) {
                SdkLogger.error(TAG, e);
            }
        }
    }

    public boolean isReachedCap(){
        return mIteration >= mCap;
    }

    public void increaseIteration(){
        mIteration++;
    }

    public void setHeader(String name, String value) {
        mHeaders.put(name, value);
    }

    public void setConnectTimeout(int timeout) {
        mConnectTimeout = timeout;
    }

    public void setReadTimeout(int timeout) {
        mReadTimeout = timeout;
    }

    public void setResponseListener(IInternalRequestListener listener) {
        mResponseListener = listener;
    }

    public Map<String, String > getHeaders() {
        return mHeaders;
    }

    public String getUrl() {
        return mUrl;
    }

    public String getRequestMethod() {
        return mRequestMethod;
    }

    public JSONObject getBody() {
        return mBody;
    }
    public String getKey() { return mKey; }

    public int getConnectTimeout() {
        return mConnectTimeout;
    }

    public int getReadTimeout() {
        return mReadTimeout;
    }
    
    public int getIteration() {return mIteration;}

    public void onResponseSuccess(String body){
        mResponseListener.onResponseSuccess(mKey, mIteration, body);
    }
    public void onResponseFailed(int code, String error) {
        mResponseListener.onResponseFailed(mKey, mIteration, code, error);
    }
}
