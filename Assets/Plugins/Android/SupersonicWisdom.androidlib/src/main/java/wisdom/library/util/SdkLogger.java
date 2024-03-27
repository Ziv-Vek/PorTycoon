package wisdom.library.util;

import android.util.Log;
import wisdom.library.BuildConfig;

public class SdkLogger {
    private static final boolean isDebug;
    private static boolean isLoggingEnabled;
    private static boolean shouldLog() {
        return isDebug || isLoggingEnabled;
    }

    static {
        isDebug = BuildConfig.DEBUG;
    }

    public static void setup(boolean isLoggingEnabled) {
        SdkLogger.isLoggingEnabled = isLoggingEnabled;
    }

    public static void error(Object reporter, Object errorMessage) {
        if (!shouldLog()) return;
        if (errorMessage == null) return;

        if (reporter == null) {
            reporter = "Anonymous";
        }

        if (errorMessage instanceof Throwable) {
            error(reporter, "", (Throwable) errorMessage);
        } else {
            Log.e(reporter.toString(), "[ERROR] SW Android native SDK: '" + errorMessage.toString() + "'");
        }
    }

    public static void error(Object tag, String errorMessage, Throwable throwable) {
        if (!shouldLog()) return;

        error(tag, errorMessage);
        if (throwable != null) {
            error(tag, throwable.toString());
        }
    }

    public static void log(Object reporter, Object logMessage) {
        if (!shouldLog()) return;
        if (logMessage == null) return;

        if (reporter == null) {
            reporter = "Anonymous";
        }

        Log.d(reporter.toString(), "SW Android native SDK: " + logMessage.toString());
    }

    public static void log(Object logMessage) {
        log(null, logMessage);
    }
}
