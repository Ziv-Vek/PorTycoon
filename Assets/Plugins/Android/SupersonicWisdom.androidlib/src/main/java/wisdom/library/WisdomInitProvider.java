package wisdom.library;

import android.content.ContentProvider;
import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.net.Uri;

import wisdom.library.util.SdkLogger;
import wisdom.library.util.SwCallback;

public class WisdomInitProvider extends ContentProvider {
    static final String TAG = WisdomInitProvider.class.getSimpleName();

    @Override
    public boolean onCreate() {
        Context context = getContext();
        IdentifiersGetter.fetch(context, new SwCallback<IdentifiersGetter.GetterResults>() {
            @Override
            public void onDone(IdentifiersGetter.GetterResults result) {
                if (result == null) {
                    SdkLogger.error(TAG, "Failed to fetch app set ID + advertising ID");
                } else {
                    String appSetId = result.getAppSetIdentifier();
                    String advertisingId = result.getAdvertisingIdentifier();
                    SdkLogger.log("Got app set ID '" + appSetId + "' from calling via " + TAG);
                    SdkLogger.log("Got advertising ID '" + advertisingId + "' from calling via " + TAG);
                }
            }
        });
        return false;
    }

    @Override
    public Cursor query(Uri uri, String[] projection, String selection, String[] selectionArgs, String sortOrder) {
        return null;
    }

    @Override
    public String getType(Uri uri) {
        return null;
    }

    @Override
    public Uri insert(Uri uri, ContentValues values) {
        return null;
    }

    @Override
    public int delete(Uri uri, String selection, String[] selectionArgs) {
        return 0;
    }

    @Override
    public int update(Uri uri, ContentValues values, String selection, String[] selectionArgs) {
        return 0;
    }
}
