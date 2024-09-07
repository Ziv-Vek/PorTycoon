using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AppLovinMax.Scripts.IntegrationManager.Editor;
using Network = AppLovinMax.Scripts.IntegrationManager.Editor.Network;

namespace YsoCorp {
    namespace GameUtils {

        public class YCUpdatesHandler {

            public enum UpdatePackageType {
                Amazon,
                Firebase,
                Prod_Networks,
                Rate_Popup
            }

            private static string PACKAGES_URL = "https://gameutils-unity.ysocorp.com/public/unity/";
            public static Dictionary<UpdatePackageType, PackageData> PACKAGES = new Dictionary<UpdatePackageType, PackageData>() {
                {UpdatePackageType.Amazon, new PackageData("1.8.0", "AmazonAPS", "AmazonAPS_") },
                {UpdatePackageType.Firebase, new PackageData("12.0.0", "Firebase/Analytics", "FirebaseAnalytics_") },
                {UpdatePackageType.Prod_Networks, new PackageData("1.3.0", "ProdNetworks", "ProdNetworks_") },
                {UpdatePackageType.Rate_Popup, new PackageData("1.1.0", "Packages/RatePopup", "RatePopup_") },
            };

            static YCConfig YCCONFIGDATA;

            [InitializeOnLoadMethod]
            private static void Init() {
                GetYCConfigData();
                ResetMax();
            }

            private static void GetYCConfigData() {
                if (YCCONFIGDATA == null) {
                    YCCONFIGDATA = Resources.Load<YCConfig>("YCConfigData");
                }
            }

            // ----------------------------------- Upgrade GameUtils -----------------------------------
            #region Update_Gameutils

            private struct DataData {
                public GUUpdateData data;
            }

            public struct GUUpdateData {
                public bool isAdvanced;
                public bool isUpToDate;
                public string version;
                public string url;
                public string urlAuto;
                public List<string> additionalRemovalsPaths;
            }

            public static void UpdateGameutils() {
                GetYCConfigData();
                GUUpdateData data = GetGUUpdateData();
                if (data.version != null && data.version != "") {
                    YCGameutilsUpdateWindow window = EditorWindow.GetWindow<YCGameutilsUpdateWindow>(true, "GameUtils Update");
                    window.Init(data);
                    window.Show();
                }
            }

            private static GUUpdateData GetGUUpdateData() {
                if (YCCONFIGDATA.gameYcId != "") {
                    string url = RequestManager.GetUrlEmptyStatic("games/sdk-version/" + YCCONFIGDATA.gameYcId + "?currentVersion=" + YCConfig.VERSION, true);
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Method = "Get";
                    request.ContentType = "application/json";
                    try {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                            using (var reader = new StreamReader(response.GetResponseStream())) {
#if YC_NEWTONSOFT
                                GUUpdateData data = Newtonsoft.Json.JsonConvert.DeserializeObject<DataData>(reader.ReadToEnd()).data;
#else
                                GUUpdateData data = new GUUpdateData();
#endif
                                if (data.version != "") {
                                    return data;
                                } else {
                                    EditorUtility.DisplayDialog("Game Utils Update", "Error. Check your Game Yc ID or your connection.", "Ok");
                                    return new GUUpdateData();
                                }
                            }
                        }
                    } catch (Exception) {
                        EditorUtility.DisplayDialog("Game Utils Update", "Error. Check your Game Yc Id or your connection.", "Ok");
                        return new GUUpdateData();
                    }
                } else {
                    EditorUtility.DisplayDialog("Game Utils Update", "Please enter your Game Yc Id in the YCConfigData.", "Ok");
                    return new GUUpdateData();
                }
            }

            public static void ImportGameutilsPackage(GUUpdateData data) {
                string fileName = "Gameutils_" + data.version.Replace('.', '_');
                string url = data.url;
                bool interactive = true;
                if (data.isAdvanced == false && data.urlAuto != "") {
                    url = data.urlAuto;
                    fileName += "_Auto";
                    interactive = fileName.Contains("Auto") == false;
                }
                fileName += ".unitypackage";
                YCPackageManager.DownloadAndImportPackage(url, fileName, interactive);
            }

            #endregion


            // ----------------------------------- Upgrade MAX -----------------------------------
            #region Upgrade_MAX

            public static bool MAX_UPGRADING { get { return MAX_NETWORKS_UPGRADING.Count > 0 || MAX_MAIN_UPGRADING != null; } set { } }
            static List<Network> MAX_NETWORKS_UPGRADING = new List<Network>();
            static List<string> MAX_NETWORKS_PATHS = new List<string>();
            static Network MAX_MAIN_UPGRADING = null;
            static int UPDATE_COUNTER = 0;

            public static void ResetMax() {
                MAX_NETWORKS_UPGRADING.Clear();
                MAX_NETWORKS_PATHS.Clear();
                MAX_MAIN_UPGRADING = null;
                UPDATE_COUNTER = 0;
            }

            public static void UpgradeMax() {
                if (MAX_UPGRADING == false) {

                    var loadDataCoroutine = AppLovinEditorCoroutine.StartCoroutine(AppLovinIntegrationManager.Instance.LoadPluginData(data => {
                        if (data.AppLovinMax.CurrentToLatestVersionComparisonResult == MaxSdkUtils.VersionComparisonResult.Lesser) {
                            MAX_MAIN_UPGRADING = data.AppLovinMax;
                        }
                        for (int i = 0; i < data.MediatedNetworks.Length; i++) {
                            if (IsMaxNetworkInstalled(data.MediatedNetworks[i]) && data.MediatedNetworks[i].CurrentToLatestVersionComparisonResult == MaxSdkUtils.VersionComparisonResult.Lesser) {
                                MAX_NETWORKS_UPGRADING.Add(data.MediatedNetworks[i]);
                            }
                        }
                        YCMaxUpgradeWindow window = EditorWindow.GetWindow<YCMaxUpgradeWindow>(true, "Applovin Max Upgrade");
                        window.Init(MAX_MAIN_UPGRADING, MAX_NETWORKS_UPGRADING.ToArray());
                        window.Show();
                    }));
                } else {
                    Debug.Log("Applovin is currently upgrading");
                }
            }

            public static void ApplySelectedMaxUpgrades(Network mainPlugin, List<Network> networks) {
                MAX_MAIN_UPGRADING = mainPlugin;
                MAX_NETWORKS_UPGRADING = networks;
                UPDATE_COUNTER = networks.Count;

                YCEditorCoroutine.StartCoroutine(ImportAllNetworks());
                Network[] tmpNetworks = MAX_NETWORKS_UPGRADING.ToArray();
                for (int i = 0; i < tmpNetworks.Length; i++) {
                    Network currentNetwork = tmpNetworks[i];
                    DownloadMaxNetwork(tmpNetworks[i], () => MAX_NETWORKS_UPGRADING.Remove(currentNetwork));
                }
                if (MAX_MAIN_UPGRADING != null) {
                    YCEditorCoroutine.StartCoroutine(DownloadMaxMainPlugin(MAX_MAIN_UPGRADING));
                }
            }

            private static bool IsMaxNetworkInstalled(Network network) {
                return string.IsNullOrEmpty(network.CurrentVersions.Unity) == false;
            }

            private static void DownloadMaxNetwork(Network network, Action action) {
                string versionsString = network.LatestVersions.Unity.Replace(".", "-") + "_" + network.LatestVersions.Android.Replace(".", "-") + "_" + network.LatestVersions.Ios.Replace(".", "-");
                string fileName = "ApplovinMax_" + network.DisplayName + "_" + versionsString + ".unitypackage";
                YCEditorCoroutine.StartCoroutine(YCPackageManager.DownloadPackage(network.DownloadUrl, fileName, (downloaded, path) => {
                    if (downloaded) {
                        MAX_NETWORKS_PATHS.Add(path);
                    }
                    action();
                }));
            }

            private static IEnumerator DownloadMaxMainPlugin(Network applovin) {
                while (MAX_NETWORKS_UPGRADING.Count != 0) {
                    yield return new WaitForSeconds(0.1f);
                }
                DownloadMaxNetwork(applovin, () => MAX_MAIN_UPGRADING = null);
            }

            private static IEnumerator ImportAllNetworks() {
                while (MAX_UPGRADING) {
                    yield return null;
                }

                AssetDatabase.importPackageCompleted += ApplovinMaxImportCallback;
                foreach (string path in MAX_NETWORKS_PATHS) {
                    AssetDatabase.ImportPackage(path, false);
                }
                MAX_NETWORKS_PATHS.Clear();
            }

            private static void ApplovinMaxImportCallback(string packageName) {
                if (packageName.StartsWith("ApplovinMax_")) {
                    UPDATE_COUNTER--;
                    if (UPDATE_COUNTER < 1) {
                        AssetDatabase.importPackageCompleted -= ApplovinMaxImportCallback;
                        Debug.Log("Applovin MAX upgrade complete");
                    }
                }
            }
            #endregion

            // ----------------------------------- Update any package -----------------------------------
            #region Any Package

            public static void UpdatePackage(UpdatePackageType packageType) {
                string folder = PACKAGES[packageType].folderPath;
                string fileName = PACKAGES[packageType].fileName + ".unitypackage";
                string downloadURL = PACKAGES_URL + folder + fileName;
                YCPackageManager.DownloadAndImportPackage(downloadURL, fileName, false);
            }

            #endregion

            // ----------------------------------- Structures -----------------------------------
            #region Structures

            public struct PackageData {
                public Version version;
                public string folderPath;
                public string fileName;

                public PackageData(string version, string folderPath, string filePrefix) {
                    this.version = new Version(version);
                    this.folderPath = folderPath + "/";
                    this.fileName = filePrefix + version.Replace('.', '_');
                }
            }

            #endregion

        }
    }
}