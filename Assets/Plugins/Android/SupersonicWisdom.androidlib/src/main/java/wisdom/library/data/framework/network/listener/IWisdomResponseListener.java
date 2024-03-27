package wisdom.library.data.framework.network.listener;

public interface IWisdomResponseListener {
    void onResponseFailed(int code, String error);
    void onResponseSuccess();
}
