using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using System.Net;
using Facebook.Unity.Settings;
using System.Collections.Generic;
using System;

namespace YsoCorp {
    namespace GameUtils {

        [CustomEditor(typeof(YCConfig))]
        public class YCConfigEditor : Editor {
            YCConfig myTarget;
            public override void OnInspectorGUI() {
                this.DrawDefaultInspector();
                GUILayout.Space(10);
                myTarget = (YCConfig)this.target;
                if (GUILayout.Button("Import Config")) {
                    EditorImportConfig(myTarget);
                }
                GUILayout.Space(10);

#if IN_APP_PURCHASING
                if (GUILayout.Button("Deactivate In App Purchases")) { 
                    YCScriptingDefineSymbols.RemoveDefineSymbolsForMobile("IN_APP_PURCHASING"); 
                    Client.Remove("com.unity.purchasing"); 
                } 
#else
                if (GUILayout.Button("Activate In App Purchases")) {
                    YCScriptingDefineSymbols.AddDefineSymbolsForMobile("IN_APP_PURCHASING");
#if UNITY_2020_3_OR_NEWER
                    Client.Add("com.unity.purchasing");
#else
                    Client.Add("com.unity.purchasing@4.0.3"); 
#endif
                }
#endif

#if FIREBASE
                if (GUILayout.Button("Deactivate Firebase")) { 
                    YCScriptingDefineSymbols.RemoveDefineSymbolsForMobile("FIREBASE"); 
                } 
#else
                if (GUILayout.Button("Activate Firebase")) {
                    if (Directory.Exists("Assets/Firebase")) {
                        YCScriptingDefineSymbols.AddDefineSymbolsForMobile("FIREBASE");
                    } else {
                        DisplayDialog("Error", "This only for validate game.\nPlease import Firebase Analytics before.", "Ok");
                    }
                }
#endif

#if AMAZON_APS
                if (GUILayout.Button("Deactivate Amazon")) {
                    YCScriptingDefineSymbols.RemoveDefineSymbolsForMobile("AMAZON_APS"); 
                } 
#else
                if (GUILayout.Button("Activate Amazon")) {
                    if (Directory.Exists("Assets/Amazon")) {
                        YCScriptingDefineSymbols.AddDefineSymbolsForMobile("AMAZON_APS");
                    } else {
                        DisplayDialog("Error", "Please import the Amazon package before.", "Ok");
                    }
                }
#endif

            }

            public static bool DisplayDialog(string title, string msg, string ok) {
                return EditorUtility.DisplayDialog(title, msg, ok);
            }

            public static void DisplayImportConfigDialog(YCConfig.InfosData infos) {
                string msg = "";
                string msgAnd = "";
                string msgIos = "";
                string msgUpdates = "";
                if (infos.facebook_app_id == "") msg += "Facebook ID is empty\n";
                if (infos.facebook_client_token == "") msg += "Facebook Client Token is empty\n";

                // Android
                if (infos.android_key != "") {
                    if (infos.android_key != PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android)) msgAnd += "- Package name is different\n";
                    if (infos.isYsocorp) {
                        if (infos.applovin.adunits.android.interstitial == "") msgAnd += "- Ad Unit : Interstitial is empty\n";
                        if (infos.applovin.adunits.android.rewarded == "") msgAnd += "- Ad Unit : Rewarded is empty\n";
                        if (infos.applovin.adunits.android.banner == "") msgAnd += "- Ad Unit : Banner is empty\n";
                    }
                    if (Directory.Exists("Assets/MaxSdk/Mediation/Google") && infos.admob_android_app_id == "") msgAnd += "- Google AdMob ID is Empty\n";
                    if (Directory.Exists("Assets/MaxSdk/Mediation/Google") == false && infos.admob_android_app_id != "") msgAnd += "- Google AdMob ID found but the network is not installed\n";
#if AMAZON_APS
                    if (infos.aps_android.app_id == "") msgAnd += "- Amazon App ID is empty but Amazon is active\n";
                    if (infos.aps_android.interstitial == "") msgAnd += "- Amazon Interstitial is empty but Amazon is active\n";
                    if (infos.aps_android.rewarded == "") msgAnd += "- Amazon Rewarded is empty but Amazon is active\n";
                    if (infos.aps_android.banner == "") msgAnd += "- Amazon Banner is empty but Amazon is active\n";
#else
                    if (infos.aps_android.app_id != "") msgAnd += "- Amazon App ID found, but Amazon is not active\n";
                    if (infos.aps_android.interstitial != "") msgAnd += "- Amazon Interstitial found, but Amazon is not active\n";
                    if (infos.aps_android.rewarded != "") msgAnd += "- Amazon Rewarded found, but Amazon is not active\n";
                    if (infos.aps_android.banner != "") msgAnd += "- Amazon Banner found, but Amazon is not active\n";
#endif
                    if (msgAnd != "") {
                        msgAnd = "\n-------- Android --------\n" + msgAnd;
                    }
                }

                // iOS
                if (infos.ios_bundle_id != "") {
                    if (infos.ios_bundle_id != PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS)) msgIos += "- Bundle identifier is different\n";
                    if (infos.ios_key == "") msgIos += "- Apple ID is empty\n";
                    if (infos.isYsocorp) {
                        if (infos.applovin.adunits.ios.interstitial == "") msgIos += "- Ad Unit : Interstitial is empty\n";
                        if (infos.applovin.adunits.ios.rewarded == "") msgIos += "- Ad Unit : Rewarded is empty\n";
                        if (infos.applovin.adunits.ios.banner == "") msgIos += "- Ad Unit : Banner is empty\n";
                    }
                    if (Directory.Exists("Assets/MaxSdk/Mediation/Google") && infos.admob_ios_app_id == "") msgIos += "- Google AdMob ID is Empty\n";
                    if (Directory.Exists("Assets/MaxSdk/Mediation/Google") == false && infos.admob_ios_app_id != "") msgIos += "- Google AdMob ID found but the network is not installed\n";
#if AMAZON_APS
                    if (infos.aps_ios.app_id == "") msgIos += "- Amazon App ID is empty but Amazon is active\n";
                    if (infos.aps_ios.interstitial == "") msgIos += "- Amazon Interstitial is empty but Amazon is active\n";
                    if (infos.aps_ios.rewarded == "") msgIos += "- Amazon Rewarded is empty but Amazon is active\n";
                    if (infos.aps_ios.banner == "") msgIos += "- Amazon Banner is empty but Amazon is active\n";
#else
                    if (infos.aps_ios.app_id != "") msgIos += "- Amazon App ID found, but Amazon is not active\n";
                    if (infos.aps_ios.interstitial != "") msgIos += "- Amazon Interstitial found, but Amazon is not active\n";
                    if (infos.aps_ios.rewarded != "") msgIos += "- Amazon Rewarded found, but Amazon is not active\n";
                    if (infos.aps_ios.banner != "") msgIos += "- Amazon Banner found, but Amazon is not active\n";
#endif
                    if (msgIos != "") {
                        msgIos = "\n---------- iOS ----------\n" + msgIos;
                    }
                }

                //Updates 
#if AMAZON_APS
                Version amazonWantedVersion = YCUpdatesHandler.PACKAGES[YCUpdatesHandler.UpdatePackageType.Amazon].version;
                if (new Version(AmazonConstants.VERSION) < amazonWantedVersion) msgUpdates += $"- Amazon: {AmazonConstants.VERSION} => {amazonWantedVersion}\n";
#endif
#if FIREBASE
                string firebaseVersion = new DirectoryInfo(Directory.GetDirectories(Application.dataPath + "/Firebase/m2repository/com/google/firebase/firebase-analytics-unity")[0]).Name;
                Version firebaseWantedVersion = YCUpdatesHandler.PACKAGES[YCUpdatesHandler.UpdatePackageType.Firebase].version;
                if (new Version(firebaseVersion) < firebaseWantedVersion) msgUpdates += $"- Firebase: {firebaseVersion} => {firebaseWantedVersion}\n"; 
#endif
                if (msgUpdates != "") {
                    msgUpdates = "\n---------- Updates ----------\nSome packages are outdated. Please reimport the package via the top menu.\n" + msgUpdates;
                }

                // Full Error
                msg += msgAnd + msgIos + msgUpdates;
                if (msg != "") {
                    msg = "\n/!\\ Import config Warning /!\\\n" + msg;
                } else {
                    msg = "Import config Succeeded!";
                }
                DisplayDialog("Success", msg, "Ok");
            }

            public static void EditorImportConfig(YCConfig config, bool ignoreIfNoId = false) {
                if (config.gameYcId != "") {
                    string url = RequestManager.GetUrlEmptyStatic("games/setting/" + config.gameYcId + "/" + Application.identifier, true);
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Method = "Get";
                    request.ContentType = "application/json";
                    try {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                            using (var reader = new StreamReader(response.GetResponseStream())) {
                                EditorUtility.SetDirty(config);
#if YC_NEWTONSOFT
                                YCConfig.InfosData infos = Newtonsoft.Json.JsonConvert.DeserializeObject<YCConfig.DataData>(reader.ReadToEnd()).data;
#else
                                YCConfig.InfosData infos = new YCConfig.InfosData();
#endif
                                if (infos.name != "") {
                                    config.Name = infos.name;
                                    config.FbAppId = infos.facebook_app_id;
                                    config.FbClientToken = infos.facebook_client_token;
                                    config.appleId = infos.ios_key;

                                    config.AdMobAndroidAppId = infos.admob_android_app_id;
                                    config.AdMobIosAppId = infos.admob_ios_app_id;

                                    // APPLOVIN
                                    config.IosInterstitial = infos.applovin.adunits.ios.interstitial;
                                    config.IosRewarded = infos.applovin.adunits.ios.rewarded;
                                    config.IosBanner = infos.applovin.adunits.ios.banner;
                                    config.AndroidInterstitial = infos.applovin.adunits.android.interstitial;
                                    config.AndroidRewarded = infos.applovin.adunits.android.rewarded;
                                    config.AndroidBanner = infos.applovin.adunits.android.banner;

                                    // MMPs
                                    config.MmpTenjin = infos.mmps.tenjin.active;

                                    // AMAZON
                                    config.AmazonIosAppID = infos.aps_ios.app_id;
                                    config.AmazonIosInterstitial = infos.aps_ios.interstitial;
                                    config.AmazonIosRewarded = infos.aps_ios.rewarded;
                                    config.AmazonIosBanner = infos.aps_ios.banner;
                                    config.AmazonAndroidAppID = infos.aps_android.app_id;
                                    config.AmazonAndroidInterstitial = infos.aps_android.interstitial;
                                    config.AmazonAndroidRewarded = infos.aps_android.rewarded;
                                    config.AmazonAndroidBanner = infos.aps_android.banner;

                                    InitFacebook(config);
                                    InitMax(config);
                                    InitFirebase(infos);
                                    InitAmazon();
                                    AssetDatabase.SaveAssets();
                                    DisplayImportConfigDialog(infos);
                                } else {
                                    DisplayDialog("Error", "Impossible to import config. Check your Game Yc Id or your connection.", "Ok");
                                }
                            }
                        }
                    } catch (Exception) {

                        DisplayDialog("Error", "Impossible to import config. Check your Game Yc Id or your connection.", "Ok");
                    }
                } else {
                    if (ignoreIfNoId == false) {
                        DisplayDialog("Error", "Please enter Game Yc Id.", "Ok");
                    }
                }
            }

            public static void EditorImportConfig(bool ignoreIfNoId = false) {
                YCConfig ycConfig = Resources.Load<YCConfig>("YCConfigData");
                EditorImportConfig(ycConfig, ignoreIfNoId);
            }

            public static void InitFacebook(YCConfig config) {
                FacebookSettings.AppIds = new List<string> { config.FbAppId };
                FacebookSettings.AppLabels = new List<string> { Application.productName };
                FacebookSettings.ClientTokens = new List<string> { config.FbClientToken };
                EditorUtility.SetDirty(FacebookSettings.Instance);
                AssetDatabase.SaveAssets();

                YCXmlHandler.UpdateDocument("Plugins/Android/AndroidManifest.xml", (xmlDocument, documentRoot) => {
                    string[] elementNames = new string[] {        "meta-data",                      "provider",                                                                   "meta-data" };
                    string[] nameAttributeValues = new string[] { "com.facebook.sdk.ApplicationId", "com.facebook.FacebookContentProvider",                                       "com.facebook.sdk.ClientToken" };
                    (string, string)[] mainAttributes = new[] {   ("value", "fb" + config.FbAppId), ("authorities", "com.facebook.app.FacebookContentProvider" + config.FbAppId), ("value", config.FbClientToken) };

                    for (int i = 0; i < 3; i++) {
                        YCXmlHandler.YCXmlAttribute nameAttribute = new YCXmlHandler.YCXmlAttribute("name", YCXmlHandler.YCAttributeType.Android, nameAttributeValues[i]);
                        YCXmlHandler.YCXmlElement element = new YCXmlHandler.YCXmlElement("/manifest/application", elementNames[i], nameAttribute);
                        YCXmlHandler.YCXmlAttribute[] newAttributes = new YCXmlHandler.YCXmlAttribute[] {
                            nameAttribute,
                            new YCXmlHandler.YCXmlAttribute(mainAttributes[i].Item1, YCXmlHandler.YCAttributeType.Android, mainAttributes[i].Item2)
                        };
                        YCXmlHandler.UpdateElement(xmlDocument, documentRoot, element, newAttributes);
                    }
                });
            }

            public static void InitMax(YCConfig config) {
                AppLovinSettings.Instance.AdMobIosAppId = config.AdMobIosAppId;
                AppLovinSettings.Instance.AdMobAndroidAppId = config.AdMobAndroidAppId;
                EditorUtility.SetDirty(AppLovinSettings.Instance);
                AssetDatabase.SaveAssets();
            }

            public static void InitFirebase(YCConfig.InfosData infos) {
#if FIREBASE
                if (infos.google_services_android != "") {
                    CreateOrUpdateFileInAssets("GameUtils/google-services.json", infos.google_services_android);
                }
                if (infos.google_services_ios != "") {
                    CreateOrUpdateFileInAssets("GameUtils/GoogleService-Info.plist", infos.google_services_ios);
                }
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
#endif
            }

            public static void InitAmazon() {
#if AMAZON_APS
                YCXmlHandler.UpdateDocument("Plugins/Android/AndroidManifest.xml", (xmlDocument, documentRoot) => {
                    string[] elementPaths = new string[]        { "/manifest",                                 "/manifest",                               "/manifest/application",                         "/manifest/application" };
                    string[] elementNames = new string[]        { "uses-permission",                           "uses-permission",                         "activity",                                      "activity" };
                    string[] nameAttributeValues = new string[] { "android.permission.ACCESS_COARSE_LOCATION", "android.permission.ACCESS_FINE_LOCATION", "com.amazon.device.ads.DTBInterstitialActivity", "com.amazon.device.ads.DTBAdActivity" };

                    for (int i = 0; i < 4; i++) {
                        YCXmlHandler.YCXmlAttribute nameAttribute = new YCXmlHandler.YCXmlAttribute("name", YCXmlHandler.YCAttributeType.Android, nameAttributeValues[i]);
                        YCXmlHandler.YCXmlElement element = new YCXmlHandler.YCXmlElement(elementPaths[i], elementNames[i], nameAttribute);
                        YCXmlHandler.CreateElement(xmlDocument, documentRoot, element);
                    }
                });
#endif
            }

            public static void CreateOrUpdateFileInAssets(string path, string content) {
                path = Application.dataPath + "/" + path;
                File.Delete(path);
                StreamWriter sw = File.CreateText(path);
                sw.Write(content + "\n");
                sw.Close();
            }
        }

    }
}