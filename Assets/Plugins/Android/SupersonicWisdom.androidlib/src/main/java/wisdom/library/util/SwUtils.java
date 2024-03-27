package wisdom.library.util;

import android.os.Handler;
import android.os.HandlerThread;
import android.os.Looper;

import wisdom.library.domain.events.dto.Constants;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.Iterator;
import java.util.UUID;

public class SwUtils {
    private static final Handler bgThreadHandler;
    private static final Handler mainThreadHandler;
    private static final String TAG = SwUtils.class.getSimpleName();

    static {
        mainThreadHandler = new Handler(Looper.getMainLooper());
        HandlerThread handlerThread = new HandlerThread(SwUtils.class.getSimpleName() + "_HandlerThread");
        handlerThread.start();
        bgThreadHandler = new Handler(handlerThread.getLooper());
    }

    public static Handler bgThreadHandler() {
        return bgThreadHandler;
    }

    public static Handler mainThreadHandler() {
        return mainThreadHandler;
    }

    public static JSONObject createEvent(String eventName, JSONObject sessionData,
            String conversionData, String metadataStr, String customsStr, String extraStr) {
        JSONObject json = new JSONObject();

        String eventId = UUID.randomUUID().toString();
        long clientTs = System.currentTimeMillis();

        addToJson(json, Constants.KEY_EVENT_NAME, eventName);
        addToJson(json, Constants.KEY_EVENT_ID, eventId);
        addToJson(json, Constants.KEY_CONVERSION_DATA, conversionData);
        addToJson(json, Constants.KEY_CLIENT_TS, clientTs);
        
        try {
            json = merge(json, sessionData);
        } catch (JSONException e) {
            SdkLogger.log("Could not parse sessionData - " + sessionData);
        }

        if (extraStr != "") {
            try {
                JSONObject extraJson = new JSONObject(extraStr);
                addToJson(json, Constants.KEY_EXTRA, extraJson);
            } catch (JSONException e) {
                SdkLogger.log("Could not parse extraData - " + extraStr);
            }
        }

        if (metadataStr != "") {
            try {
                JSONObject metaJson = new JSONObject(metadataStr);
                json = merge(json, metaJson);
            } catch (JSONException e) {
                SdkLogger.log("Could not parse metadata - " + metadataStr);
            }
        }

        if (customsStr != "") {
            try {
                JSONObject customsJson = new JSONObject(customsStr);
                json = merge(json, customsJson);
            } catch (JSONException e) {
                SdkLogger.log("Could not parse customs - " + customsStr);
            }
        }

        return json;
    }

    public static JSONObject merge(JSONObject... jsonObjects) throws JSONException {

        JSONObject jsonObject = new JSONObject();

        for (JSONObject temp : jsonObjects) {
            Iterator<String> keys = temp.keys();
            while (keys.hasNext()) {
                String key = keys.next();
                jsonObject.putOpt(key, temp.get(key));
            }

        }
        return jsonObject;
    }

    public static <T> T safeGetObject(JSONObject jsonObject, String key) {
        Object value;
        T typedValue = null;
        try {
            value = jsonObject.get(key);
            typedValue = (T) value;
        } catch (ClassCastException | JSONException e) {
            SdkLogger.error(TAG, "Error getting key - " + key + " from json - " + jsonObject + "\nexception: " + e);
        }

        return typedValue;
    }

    public static void addToJson(JSONObject json, String key, Object value) {
        try {
            json.putOpt(key, value);
        } catch (JSONException e) {
            SdkLogger.error(null, "Error adding key - " + key + " to json - " + json + "\nexception: " + e);
        }
    }

    public static JSONObject createJsonOrEmpty(String jsonStr) {
        try {
            return new JSONObject(jsonStr);
        } catch (JSONException e) {
            return new JSONObject();
        }
    }
}
