using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace YsoCorp {
    namespace GameUtils {

        public class YCDebugWindow : YCCustomWindow {

            // ----------------------------------- Base -----------------------------------
            private static YCConfig YCCONFIGDATA;
            private bool _isGUInit = false;
            private bool _isYCInit = false;

            [UnityEditor.Callbacks.DidReloadScripts]
            public static void Init() {
                YCCONFIGDATA = Resources.Load<YCConfig>("YCConfigData");
            }

            private void OnGUI() {
                if (Application.isPlaying) {
                    this.OnGUIGU();
                    this.OnGUICustom();
                } else {
                    this.AddLabel("\nPlease enter play mode", TextAnchor.MiddleCenter);
                    this.ResetInits();
                }
            }

            private void UpdateData() {
                if (Application.isPlaying) {
                    if (this._isGUInit == false) {
                        this.InitDataGU();
                    }
                    this.UpdateDataGU();
                }
            }

            private void ResetInits() {
                if (this._isGUInit) {
                    this._isGUInit = false;
                }
                if (this._isYCInit) {
                    this._isYCInit = false;
                }
            }

            private void OnInspectorUpdate() {
                this.UpdateData();
                Repaint();
            }

            // ----------------------------------- GameUtils Data -----------------------------------

            public static string TIME = "";
            public static string ABTEST_DATA = "";
            public static string INTERDELAY_DATA = "";
            public static int INTERCOUNTER_DATA = 0;

            private static int BASE_INTER_COUNTER;
            public static int CURRENTLEVEL_DATA = 0;

            private void InitDataGU() {
                if (YCManager.instance != null) {
                    ABTEST_DATA = YCManager.instance.abTestingManager.GetPlayerSample() != "" ? YCManager.instance.abTestingManager.GetPlayerSample() : "Control";
                    BASE_INTER_COUNTER = YCManager.instance.dataManager.GetInterstitialsNb();
                    this._isGUInit = true;
                }
            }

            private void UpdateDataGU() {
                TIME = TimeSpan.FromSeconds(Time.time).ToString("hh':'mm':'ss");
                INTERDELAY_DATA = this.GetCurrentInterstitialDelay();
                INTERCOUNTER_DATA = YCManager.instance.dataManager.GetInterstitialsNb() - BASE_INTER_COUNTER;
                CURRENTLEVEL_DATA = YCManager.instance.currentLevel;
            }

            private void OnGUIGU() {
                this.AddLabel("------ GameUtils data ------");
                this.AddLabel("Time since start : " + TIME);
                this.AddLabel("Current AB test : " + ABTEST_DATA);
                this.AddLabel("Delay before next interstitial : " + INTERDELAY_DATA);
                this.AddLabel("Number of interstitials : " + INTERCOUNTER_DATA);
                this.AddLabel("Current level : " + CURRENTLEVEL_DATA);
            }

            private string GetCurrentInterstitialDelay() {
#if UNITY_ANDROID || UNITY_IOS
                if (YCManager.instance.adsManager.delayInterstitial != 0 && (string.IsNullOrEmpty(YCCONFIGDATA.AndroidInterstitial) == false || string.IsNullOrEmpty(YCCONFIGDATA.IosInterstitial) == false)) {
                    return YCManager.instance.adsManager.delayInterstitial > 0 ? YCManager.instance.adsManager.delayInterstitial.ToString("F2") + " seconds" : "Ready";
                } else {
                    return "Empty ad unit";
                }
#else
                return "Not in Android or iOS platform";
#endif
            }

            // ----------------------------------- Custom Data -----------------------------------


            private void OnGUICustom() {
                this.AddLabel("");
                this.AddLabel("------ Custom data ------");
                if (YCEditorUtils.debugWindowCustomDatas.Count > 0) {
                    foreach (KeyValuePair<string, string> data in YCEditorUtils.debugWindowCustomDatas) {
                        this.AddLabel(data.Key + " : " + data.Value);
                    }
                } else {
                    this.AddLabel("You do not track any custom data. To start tracking custom data, use this line of code :");
                    this.AddSelectableLabel("YsoCorp.GameUtils.YCEditorUtils.debugWindowCustomDatas[\"My unique title\"] = \"My string value\";");
                    this.AddLabel("Since it's a dictionary, you can reassign the value to keep track of the changes.");
                }
            }


        }
    }
}