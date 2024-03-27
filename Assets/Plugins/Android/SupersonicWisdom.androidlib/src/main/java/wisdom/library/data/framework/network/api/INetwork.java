package wisdom.library.data.framework.network.api;

import org.json.JSONObject;

import wisdom.library.data.framework.network.request.WisdomRequest;

public interface INetwork {
    void setConnectTimeout(int timeout);
    void setReadTimeout(int timeout);
    int getConnectTimeout();
    int getReadTimeout();
    void sendAsync(String key, String url, JSONObject body, IInternalRequestListener listener);
    void sendAsync(WisdomRequest request);
    int send(String key, String url, JSONObject body, IInternalRequestListener listener);
    int send(String key, String url, JSONObject body, int connectTimeout, int readTimeout, IInternalRequestListener listener);
}

