using UnityEngine;
using UnityEditor;

namespace YsoCorp {
    namespace GameUtils {

        public class YCMenuItemManager {

            static string WEBSITE_URL = "https://gameutils.ysocorp.com/";

            static YCConfig YCCONFIGDATA;

            [InitializeOnLoadMethod]
            private static void Init() {
                GetYCConfigData();
            }

            private static void GetYCConfigData() {
                if (YCCONFIGDATA == null) {
                    YCCONFIGDATA = Resources.Load<YCConfig>("YCConfigData");
                }
            }

            [MenuItem("GameUtils/Open game's page", false, 1001)]
            static void MenuOpenGamePage() {
                GetYCConfigData();
                if (YCCONFIGDATA.gameYcId != "") {
                    Application.OpenURL(WEBSITE_URL + YCCONFIGDATA.gameYcId + "/settings");
                } else {
                    EditorUtility.DisplayDialog("Game's web page", "Please enter your Game Yc ID in the YCConfigData.", "Ok");
                }
            }

            [MenuItem("GameUtils/Import Package/Amazon", false, 1002)]
            static void ImportAmazon() {
                YCUpdatesHandler.UpdatePackage(YCUpdatesHandler.UpdatePackageType.Amazon);
            }

            [MenuItem("GameUtils/Import Package/Firebase", false, 1003)]
            static void ImportFirebase() {
                YCUpdatesHandler.UpdatePackage(YCUpdatesHandler.UpdatePackageType.Firebase);
            }

            [MenuItem("GameUtils/Import Package/Prod Networks", false, 1004)]
            static void ImportProdNetworks() {
                YCUpdatesHandler.UpdatePackage(YCUpdatesHandler.UpdatePackageType.Prod_Networks);
            }

            [MenuItem("GameUtils/Import Package/Rate Popup", false, 1501)]
            static void ImportRatePopup() {
                YCUpdatesHandler.UpdatePackage(YCUpdatesHandler.UpdatePackageType.Rate_Popup);
            }

            [MenuItem("GameUtils/Update GameUtils", false, 2001)]
            static void MenuUpdateGameutils() {
                YCUpdatesHandler.UpdateGameutils();
            }

            [MenuItem("GameUtils/Upgrade Applovin MAX", false, 2002)]
            static void MenuUpgradeMax() {
                YCUpdatesHandler.UpgradeMax();
            }

            [MenuItem("GameUtils/Open Config", false, 2501)]
            static void MenuOpenConfig() {
                GetYCConfigData();
                Selection.activeObject = YCCONFIGDATA;
            }

            [MenuItem("GameUtils/Import Config", false, 2502)]
            static void MenuImportConfig() {
                GetYCConfigData();
                YCConfigEditor.EditorImportConfig(YCCONFIGDATA);
            }

            [MenuItem("GameUtils/Tools/Display Debug Window", false, 3001)]
            static void MenuToolDebugWindow() {
                YCDebugWindow window = EditorWindow.GetWindow<YCDebugWindow>(false, "GameUtils Debug Window");
                YCDebugWindow.Init();
                window.SetMinSize(570, 290);
                window.Show();
            }

            [MenuItem("GameUtils/Tools/Replace GameObjects", false, 3002)]
            static void MenuToolReplaceBy() {
                YCReplaceObjectsWindow window = EditorWindow.GetWindow<YCReplaceObjectsWindow>(false, "Replace GameObjects");
                window.Init();
                window.Show();
            }
        }
    }
}