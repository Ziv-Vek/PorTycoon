-keeppackagenames # This will prevent collisions with other 3rd parties after obfuscation. For example, the same we had with Fyber. Reference: https://supersonicstudio.monday.com/boards/883112163/pulses/2144806504

-keep class wisdom.library.api.listener.IWisdomSessionListener { *; }
-keep class wisdom.library.api.listener.IWisdomInitListener { *; }
-keep class wisdom.library.api.listener.IWisdomRequestListener { *; }
-keep class wisdom.library.api.listener.IWisdomConnectivityListener { *; }
-keep class wisdom.library.api.WisdomSDK { *; }
-keep class wisdom.library.domain.events.dto.EventMetadataDto { *; }
-keep class wisdom.library.api.dto.WisdomConfigurationDto {*;}
-keep class wisdom.library.domain.events.dto.ExtraEventDetailsDto {*;}

#AppSet ID
-keep class com.google.android.gms.appset.* { *; }
-keep class com.google.android.gms.tasks.* { *; }

#GAID
-keep class com.google.android.gms.ads.identifier.* {public *;}

#Google Play Services
-keep class com.google.android.play.** { *; }

##### SupersonicWisdom 3rd Parties Proguard Rules #####

# IronSource
-keepclassmembers class com.ironsource.sdk.controller.IronSourceWebView$JSInterface {
    public *;
}
-keepclassmembers class * implements android.os.Parcelable {
    public static final android.os.Parcelable$Creator *;
}

-keep public class com.google.android.gms.ads.** {
   public *;
}
-keep class com.ironsource.adapters.** { *;}
-keep class com.ironsource.unity.androidbridge.** { *;}
-keep class io.liftoff.** {*;}

-dontwarn com.ironsource.mediationsdk.**
-dontwarn com.ironsource.adapters.**
-dontwarn com.moat.**
-keep class com.moat.** { public protected private *; }

# AppsFlyer
-keep class com.appsflyer.** { *; }
-dontwarn com.android.installreferrer
-dontwarn com.appsflyer.**

# GameAnalytics
-keep class com.gameanalytics.sdk { *; }
-keep class com.gameanalytics.sdk.** { *; }
 
-keep class com.gameanalytics.sdk.GAPlatform { *; }
-keep class com.gameanalytics.sdk.GAPlatform.** { *; }
-keep class android.net.ConnectivityManager.** { *; }
-keep class com.google.android.instantapps.InstantApps { *; }
-keepclassmembers class com.google.android.instantapps.InstantApps { *; }

# FacebookSDK
-keepclassmembers,allowoptimization enum * {
    public static **[] values();
    public static ** valueOf(java.lang.String);
}

-keepclassmembers class * implements java.io.Serializable {
    private static final java.io.ObjectStreamField[] serialPersistentFields;
    private void writeObject(java.io.ObjectOutputStream);
    private void readObject(java.io.ObjectInputStream);
    java.lang.Object writeReplace();
    java.lang.Object readResolve();
}

-keepnames class com.facebook.FacebookActivity
-keepnames class com.facebook.CustomTabActivity

-keepnames class com.android.installreferrer.api.InstallReferrerClient
-keepnames class com.android.installreferrer.api.InstallReferrerStateListener
-keepnames class com.android.installreferrer.api.ReferrerDetails

-keep class com.facebook.core.Core
-keep class com.facebook.unity.* { *; }
-keep class com.facebook.internal.* { *; }

# UniWebView
-keep class com.onevcat.** { *; }

# keep class names and method names used by reflection by InAppPurchaseEventManager
-keep public class com.android.vending.billing.IInAppBillingService {
    public <methods>;
}
-keep public class com.android.vending.billing.IInAppBillingService$Stub {
    public <methods>;
}

# Unity Android notifications
-keep class com.unity.androidnotifications.** { *; }

# GoogleAdMob - UMP
-keep class com.google.android.ump.** { *; }
-keep public class com.google.android.ump.** {
   public *;
}

#Firebase
-keep,includedescriptorclasses public class com.google.firebase.crashlytics.FirebaseCrashlytics { *; }
-keep,includedescriptorclasses public class com.google.firebase.crashlytics.internal.common.CrashlyticsCore { *; }
-keep,includedescriptorclasses public class com.google.firebase.crashlytics.internal.common.DataCollectionArbiter { *; }
-keep,includedescriptorclasses public class com.google.firebase.crashlytics.ndk.FirebaseCrashlyticsNdk { *; }
-keep class com.google.firebase.crashlytics.ndk.** { *; }

# RootBeer
-keepclassmembers class com.scottyab.rootbeer.RootBeer.** { *; }
-keep class com.scottyab.rootbeer.RootBeer.** { *; }
-keep class com.scottyab.rootbeer.** { *; }