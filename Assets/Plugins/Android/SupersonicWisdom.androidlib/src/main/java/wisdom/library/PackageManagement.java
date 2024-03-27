package wisdom.library;

import android.content.Context;
import android.content.pm.PackageManager;

import wisdom.library.util.SdkLogger;

public class PackageManagement {
    
    private static final String TAG = PackageManagement.class.getSimpleName();
    private final Context applicationContext;
    private String appInstallerSource;
    public PackageManagement(final Context context){
        applicationContext = context;
    }
    
    public String getAppInstallerSource() {
        if (appInstallerSource == null) {
            appInstallerSource = getInstallSource();
        }
        return appInstallerSource;
    }
    
    private String getInstallSource() {
        String installSource = "unknown";
        try {
            PackageManager packageManager = applicationContext.getPackageManager();
            if (packageManager != null) {
                // if the app was installed from Google Play, the installer package name will be "com.android.vending"
                String installerPackageName = packageManager.getInstallerPackageName(applicationContext.getPackageName());
                if (installerPackageName != null) {
                    appInstallerSource = installerPackageName;
                    installSource = installerPackageName;
                    SdkLogger.log("Got install source '" + appInstallerSource + "' from calling via " + TAG);
                }
            }
        } catch (Exception e) {
            SdkLogger.error(TAG, "Failed to get install source", e);
        }
        return installSource;
    }
}
