package wisdom.library.data.framework.local;

import android.content.SharedPreferences;

import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;

public class ConversionDataLocalApi {

    private static final String PREFS_EVENT_CONVERSION_DATA_KEY = "AFConversionData";

    private SharedPreferences mUnityPrefs;

    public ConversionDataLocalApi(SharedPreferences unityPrefs) {
        mUnityPrefs = unityPrefs;
    }

    public String getConversionData() {
        String conversionData = mUnityPrefs.getString(PREFS_EVENT_CONVERSION_DATA_KEY, "");
        try {
            conversionData = URLDecoder.decode(conversionData, "UTF-8");
        } catch (UnsupportedEncodingException e) { }
        return  conversionData;
    }
}
