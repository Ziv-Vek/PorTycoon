using UnityEngine;
using System;

namespace YsoCorp {
    namespace GameUtils {
        [DefaultExecutionOrder(-20)]
        public class YCManager : BaseManager {

            public static YCManager instance;

            #region Static Field
            public static string VERSION = "0.1.0";

            public bool HasGameStarted { get; private set; } = false;
            #endregion

            public YCConfig ycConfig;

            public ABTestingManager abTestingManager { get; set; }
            public AdsManager adsManager { get; set; }
            public AnalyticsManager analyticsManager { get; set; }
            public FBManager fbManager { get; set; }
            public GdprManager gdprManager { get; set; }
            public I18nManager i18nManager { get; set; }
            public InAppManager inAppManager { get; set; }
            public MmpManager mmpManager { get; set; }
            public NoInternetManager noInternetManager { get; set; }
            public RequestManager requestManager { get; set; }
            public SettingManager settingManager { get; set; }
            public YCDataManager dataManager { get; set; }

            private int _currentLevel;
            public int currentLevel { get { return this._currentLevel; }}

            private void Awake() {
                if (instance != null) {
                    DestroyImmediate(this.gameObject);
                } else {
                    instance = this;
                    this.ycManager = this;
                    DontDestroyOnLoad(this.gameObject);
                    if (IsTestBuild() == false) {
                        this.ycConfig.activeLogs = 0;
                        this.ycConfig.ABDebugLog = false;
                        this.ycConfig.InappDebug = false;
                    }
                    Debug.Log("[GameUtils] YCManager : Initialize !");
                }
            }

            private void Start() {
                this.ycManager.dataManager.IncrementPlayerLaunchCount();
                this.CheckMmpLaunchCountEvent();
            }

            /// <summary>
            /// Checks if the current runtime is a Test build or Editor
            /// </summary>
            /// <returns>Returns true if the current runtime is a Test build or Editor</returns>
            public static bool IsTestBuild() {
                return Application.installMode == ApplicationInstallMode.Editor || Application.installMode == ApplicationInstallMode.DeveloperBuild;
            }

            /// <summary>
            /// Get the amount of time the game was launched.
            /// </summary>
            /// <returns>Returns the amount of time the game was launched</returns>
            public int GetPlayerLaunchCount() {
                return this.ycManager.dataManager.GetPlayerLaunchCount();
            }

            /// <summary>
            /// Gets if this is the first time the game was launched.
            /// </summary>
            /// <returns>Returns true if this is the first time the game was launched</returns>
            public bool IsFirstTimeAppLaunched() {
                return this.GetPlayerLaunchCount() == 1;
            }

            private void CheckMmpLaunchCountEvent() {
                if (this.GetPlayerLaunchCount() == 20) {
                    this.mmpManager.OnMmpsInit.AddListener(() => this.mmpManager.SendEvent("20_game_launch"));
                }
            }

            /// <summary>
            /// Begins gathering data for the given level. Index must be between 1 and infinity
            /// </summary>
            /// <param name="level">The level index that starts</param>
            public void OnGameStarted(int level) {
                if (this.HasGameStarted == false) {
                    this.HasGameStarted = true;
                    this.analyticsManager.OnGameStarted(level);
                    this.ycManager.dataManager.IncrementPlayerGameCount();
                    this._currentLevel = level;
                }
            }

            /// <summary>
            /// Stops gathering data for the level previously started with OnGameStarted
            /// </summary>
            /// <param name="levelComplete">Was the level won (true) or lost (false)</param>
            /// <param name="score">The score for the level</param>
            public void OnGameFinished(bool levelComplete, float score = 0f) {
                if (this.HasGameStarted == true) {
                    this.HasGameStarted = false;
                    this.analyticsManager.OnGameFinished(levelComplete, score);
                    this.dataManager.IncrementLevelPlayed(this._currentLevel, levelComplete);
                }
            }

        }
    }
}
