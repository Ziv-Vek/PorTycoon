# Add project specific ProGuard rules here.
# You can control the set of applied configuration files using the
# proguardFiles setting in build.gradle.
#
# For more details, see
#   http://developer.android.com/guide/developing/tools/proguard.html

# If your project uses WebView with JS, uncomment the following
# and specify the fully qualified class name to the JavaScript interface
# class:
#-keepclassmembers class fqcn.of.javascript.interface.for.webview {
#   public *;
#}

# Uncomment this to preserve the line number information for
# debugging stack traces.
#-keepattributes SourceFile,LineNumberTable

# If you keep the line number information, uncomment this to
# hide the original source file name.
#-renamesourcefileattribute SourceFile

-keeppackagenames # This will prevent collisions with other 3rd parties after obfuscation. For example, the same we had with Fyber. Reference: https://supersonicstudio.monday.com/boards/883112163/pulses/2144806504

# Unity Android notifications
-keep class com.unity.androidnotifications.** { *; }

# GoogleAdMob - UMP
-keep class com.google.android.ump.** { *; }
-keep public class com.google.android.ump.** {
   public *;
}

-keep class wisdom.library.api.listener.IWisdomSessionListener { *; }
-keep class wisdom.library.api.listener.IWisdomInitListener { *; }
-keep class wisdom.library.api.listener.IWisdomRequestListener { *; }
-keep class wisdom.library.api.listener.IWisdomConnectivityListener { *; }
-keep class wisdom.library.api.WisdomSDK { *; }
-keep class wisdom.library.WisdomInitProvider { *; }
-keep class wisdom.library.api.dto.WisdomConfigurationDto { *; }
