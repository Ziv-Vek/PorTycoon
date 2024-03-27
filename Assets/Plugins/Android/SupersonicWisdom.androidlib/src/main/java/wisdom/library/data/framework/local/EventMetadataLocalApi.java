package wisdom.library.data.framework.local;

import android.content.SharedPreferences;

import wisdom.library.util.SdkLogger;
import wisdom.library.util.SwUtils;

import org.json.JSONException;
import org.json.JSONObject;

public class EventMetadataLocalApi {

    private static final String PREFS_EVENT_METADATA_KEY = "event_metadata";
    private SharedPreferences mPrefs;
    private SharedPreferences mUnityPrefs;

    public EventMetadataLocalApi(SharedPreferences prefs) {
        mPrefs = prefs;
    }

    public void save(String metadataJson) {
        mPrefs.edit().putString(PREFS_EVENT_METADATA_KEY, metadataJson).commit();
    }

    public String get() {
        return mPrefs.getString(PREFS_EVENT_METADATA_KEY, "");
    }

    public void update(String metadataJson) {
        save(merge(get(), metadataJson));
    }

    private static String merge(String oldMetadata, String newMetadata) {
        JSONObject oldJson, newJson;

        try{
            oldJson = new JSONObject(oldMetadata);
        }
        catch (JSONException e) {
            SdkLogger.error(null, "Error parsing metadata string: oldMetaData is empty or invalid "+ oldMetadata +"\nexception: " + e);
            oldJson = new JSONObject();
        }

        try{
            newJson = new JSONObject(newMetadata);
        }
        catch (JSONException e) {
            SdkLogger.error(null, "Error parsing metadata string: newMetadata is empty or invalid "+ newMetadata +"\nexception: " + e);
            newJson = new JSONObject();
        }

        JSONObject returnJson = new JSONObject();

        try {
            returnJson = SwUtils.merge(oldJson, newJson);
        } catch (JSONException e) {
            returnJson = new JSONObject();
            SdkLogger.error(null, "Error merging metadata strings: "+ oldMetadata + " and " + newMetadata + "\nexception: " + e);
        }
        finally {
            return returnJson.toString();
        }
    }
}
