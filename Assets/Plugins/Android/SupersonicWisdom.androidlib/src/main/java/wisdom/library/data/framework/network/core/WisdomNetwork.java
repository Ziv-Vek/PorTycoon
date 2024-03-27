package wisdom.library.data.framework.network.core;

import wisdom.library.data.framework.network.api.IInternalRequestListener;
import wisdom.library.data.framework.network.api.INetwork;
import wisdom.library.data.framework.network.request.Method;
import wisdom.library.data.framework.network.request.WisdomRequest;
import wisdom.library.data.framework.network.request.WisdomRequestExecutorTask;
import wisdom.library.data.framework.network.utils.NetworkUtils;

import org.json.JSONObject;

public class WisdomNetwork implements INetwork {

    public static final int WISDOM_INTERNAL_ERROR = -1;
    public static final int WISDOM_INTERNAL_MALFORMED_URL = -2;
    public static final int WISDOM_INTERNAL_UNSUPPORTED_ENCODING = -3;
    public static final int WISDOM_INTERNAL_PROTOCOL_ERROR = -4;
    public static final int WISDOM_INTERNAL_IO_ERROR = -5;
    public static final int WISDOM_INTERNAL_NO_INTERNET = -6;

    public static final int WISDOM_RESPONSE_CODE_BAD_REQUEST = 400;
    public static final int WISDOM_RESPONSE_CODE_OK = 200;

    public static final String WISDOM_REQUEST_METHOD_POST = "POST";
    public static final String WISDOM_REQUEST_METHOD_GET = "GET";

    public static final String WISDOM_REQUEST_HEADER_HOST = "Host";
    public static final String WISDOM_REQUEST_ENCODING = "UTF-8";
    public static final String WISDOM_NO_INTERNET_CONNECTION_MSG = "NO INTERNET CONNECTION";

    public static final int WISDOM_READ_BUFFER_SIZE = 1024;
    /**
     * Default network request timeout in millis
     */
    public static final int DEFAULT_CONNECT_TIMEOUT = 3000;
    public static final int DEFAULT_READ_RESPONSE_TIMEOUT = 3000;

    private WisdomNetworkDispatcher mDispatcher;
    private NetworkUtils mNetworkUtils;
    private int mConnectTimeout;
    private int mReadTimeout;

    public WisdomNetwork(WisdomNetworkDispatcher dispatcher, NetworkUtils networkUtils) {
        this(dispatcher, DEFAULT_CONNECT_TIMEOUT, DEFAULT_READ_RESPONSE_TIMEOUT, networkUtils);
    }

    public WisdomNetwork(WisdomNetworkDispatcher dispatcher, int connectTimeout, int readTimeout,
                         NetworkUtils networkUtils) {
        mDispatcher = dispatcher;
        mNetworkUtils = networkUtils;
        mConnectTimeout = connectTimeout;
        mReadTimeout = readTimeout;
    }

    @Override
    public void setConnectTimeout(int timeout) {
        mConnectTimeout = timeout;
    }

    @Override
    public void setReadTimeout(int timeout) {
        mReadTimeout = timeout;
    }

    @Override
    public int getConnectTimeout() {
        return mConnectTimeout;
    }

    @Override
    public int getReadTimeout() {
        return mReadTimeout;
    }

    public void sendAsync(String key, String url, JSONObject body, IInternalRequestListener listener) {
        WisdomRequest request = new WisdomRequest(key, url, Method.POST, body, listener);
        request.setHeader("Content-Type", "application/json");
        sendAsync(request);
    }
    
    @Override
    public void sendAsync(WisdomRequest request) {
        WisdomRequestExecutorTask requestTask = new WisdomRequestExecutorTask(request, mNetworkUtils);
        mDispatcher.dispatch(requestTask);
    }

    @Override
    public int send(String key, String url, JSONObject body, IInternalRequestListener listener) {
        return send(key, url, body, mConnectTimeout, mReadTimeout, listener);
    }

    @Override
    public int send(String key, String url, JSONObject body, int connectTimeout, int readTimeout, IInternalRequestListener listener) {
        WisdomRequest request = new WisdomRequest(key, url, Method.POST, body, listener);
        request.setHeader("Content-Type", "application/json");
        request.setConnectTimeout(connectTimeout);
        request.setReadTimeout(readTimeout);
        WisdomRequestExecutorTask task = new WisdomRequestExecutorTask(request, mNetworkUtils);
        return task.executeRequest();
    }
}
