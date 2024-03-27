package wisdom.library.data.framework.network.api;


public interface IInternalRequestListener {
    void onResponseFailed(String key, int iteration, int code, String error);
    void onResponseSuccess(String key, int iteration, String body);
}