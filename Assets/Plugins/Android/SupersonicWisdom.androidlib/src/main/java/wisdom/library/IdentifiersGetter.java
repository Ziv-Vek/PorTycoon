package wisdom.library;

import android.content.Context;

import wisdom.library.util.SdkLogger;
import wisdom.library.util.SwAndroidTaskWrapper;
import wisdom.library.util.SwCallback;
import wisdom.library.util.SwUtils;

import java.lang.reflect.Method;

public class IdentifiersGetter {
    public static class GetterResults {
        private final String advertisingIdentifier;
        private final String appSetIdentifier;

        private GetterResults(String advertisingId, String appSetId) {
            advertisingIdentifier = advertisingId;
            appSetIdentifier = appSetId;
        }

        private static GetterResults with(String advertisingId, String appSetId) {
            return new GetterResults(advertisingId, appSetId);
        }

        public String getAdvertisingIdentifier() {
            return advertisingIdentifier;
        }

        public String getAppSetIdentifier() {
            return appSetIdentifier;
        }
        
    }

    private static final String TAG = IdentifiersGetter.class.getSimpleName();

    private enum IdentifiersGetterController {
        instance;

        private String advertisingIdentifier;
        private String appSetIdentifier;

        private void fetch(final Context context, final SwCallback<GetterResults> callback) {
            if (didFinishBoth()) {
                callback.onDone(GetterResults.with(getAdvertisingId(), getAppSetId()));
            } else {
                // Unlike C# and JS, the variable's value (int or Integer) will be copied to the Runnable even if it was declared with the Runnable at the same method.
                // That's because the Runnable is not a closure, it's an object of an anonymous class that holds a copy of the variable.
                final int[] counter = {2};

                final Runnable onDone = new Runnable() {
                    @Override
                    public void run() {
                        SwUtils.mainThreadHandler().post(new Runnable() {
                            @Override
                            public void run() {
                                callback.onDone(GetterResults.with(getAdvertisingId(), getAppSetId()));
                            }
                        });
                    }
                };

                SwUtils.bgThreadHandler().post(new Runnable() {
                    @Override
                    public void run() {
                        String advertisingId = "";
                        Object idGetterTaskObject = null;

                        try {
                            Class<?> appSetClass = Class.forName("com.google.android.gms.appset.AppSet");
                            Method appSetIdClientStaticGetter = appSetClass.getMethod("getClient", Context.class);
                            Object appSetIdClient = appSetIdClientStaticGetter.invoke(null, context);
                            Class<?> appSetIdClientClass = Class.forName("com.google.android.gms.appset.AppSetIdClient");
                            Method infoGetterMethod = appSetIdClientClass.getMethod("getAppSetIdInfo");
                            idGetterTaskObject = infoGetterMethod.invoke(appSetIdClient);

                            Class<?> advertisingIdClientClass = Class.forName("com.google.android.gms.ads.identifier.AdvertisingIdClient");
                            Method advertisingInfoStaticGetter = advertisingIdClientClass.getMethod("getAdvertisingIdInfo", Context.class);
                            Object adInfo = advertisingInfoStaticGetter.invoke(null, context);
                            Class<?> advertisingInfoInnerClass = Class.forName("com.google.android.gms.ads.identifier.AdvertisingIdClient$Info");
                            Method idGetterMethod = advertisingInfoInnerClass.getMethod("getId");
                            Object idObject = idGetterMethod.invoke(adInfo);
                            if (idObject != null) {
                                advertisingId = idObject.toString();
                            }
                            
                        } catch (Throwable gmsAdsThrowable) {
                            // Possible "ClassNotFoundException" or "NoClassDefFoundError"
                            SdkLogger.error(TAG, "Failed to fetch Ad ID / App Set ID ", gmsAdsThrowable);

                            try {
                                boolean isAdIdProviderAvailable = false;
                                Class<?> androidxAdvertisingIdClientClass = Class.forName("androidx.ads.identifier.AdvertisingIdClient");
                                // More details: https://developer.android.com/reference/androidx/ads/identifier/AdvertisingIdClient#isAdvertisingIdProviderAvailable(android.content.Context)
                                Method advertisingInfoStaticGetter = androidxAdvertisingIdClientClass.getMethod("isAdvertisingIdProviderAvailable", Context.class);
                                Object isAdIdProviderAvailableObject = advertisingInfoStaticGetter.invoke(null, context);
                                if (isAdIdProviderAvailableObject != null) {
                                    isAdIdProviderAvailable = (boolean) isAdIdProviderAvailableObject;
                                }

                                if (isAdIdProviderAvailable) {
                                    SdkLogger.log(TAG, "AndroidX ads is available to get Ad ID");
                                }
                            } catch (Throwable androidxAdsThrowable) {
                                // Possible "ClassNotFoundException" or "NoClassDefFoundError"
                                SdkLogger.error(TAG, "Failed to fetch Ad ID also via AndroidX ads", androidxAdsThrowable);
                            }
                        } finally {
                            new SwAndroidTaskWrapper<String>(idGetterTaskObject, new SwAndroidTaskWrapper.IResultExtractor<String>() {
                                @Override
                                public String getResult(Object taskResultObject) throws ReflectiveOperationException {
                                    if (taskResultObject == null) return null;

                                    Class<?> appSetIdInfoClass = Class.forName("com.google.android.gms.appset.AppSetIdInfo");
                                    Method infoGetterMethod = appSetIdInfoClass.getMethod("getId");
                                    Object resultObject = infoGetterMethod.invoke(taskResultObject);

                                    if (resultObject == null) return null;

                                    return (String) resultObject;
                                }
                            }, new SwCallback<String>() {
                                @Override
                                public void onDone(String result) {
                                    appSetIdentifier = result == null ? "" : result;
                                    SwUtils.bgThreadHandler().post(new Runnable() {
                                        @Override
                                        public void run() {
                                            if (--counter[0] == 0) {
                                                onDone.run();
                                            }
                                        }
                                    });
                                }
                            });

                            advertisingIdentifier = advertisingId;
                            SwUtils.bgThreadHandler().post(new Runnable() {
                                @Override
                                public void run() {
                                    if (--counter[0] == 0) {
                                        onDone.run();
                                    }
                                }
                            });
                        }
                    }
                });
            }
        }

        private boolean didFinishBoth() {
            return advertisingIdentifier != null && appSetIdentifier != null;
        }
    }

    public static void fetch(Context context, SwCallback<GetterResults> callback) {
        IdentifiersGetterController.instance.fetch(context, callback);
    }

    public static String getAdvertisingId() {
        return IdentifiersGetterController.instance.advertisingIdentifier;
    }

    public static String getAppSetId() {
        return IdentifiersGetterController.instance.appSetIdentifier;
    }

}
