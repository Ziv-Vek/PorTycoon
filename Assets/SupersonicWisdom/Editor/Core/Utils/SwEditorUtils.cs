using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Facebook.Unity.Settings;
using UnityEditor.Build;

namespace SupersonicWisdomSDK.Editor
{
    public class SwEditorUtils
    {
        #region --- Constants ---

        public struct Keys
        {
            public const string CHECKSUM = "checksum";
            public const string ELIGIBLE_WISDOM_STAGE = "eligibleWisdomStage";
        }
        
        private const string UNITY_VERSION_REGEX_PATTERN = @"^(\d+)\.(\d+)\.(\d+)";
        private const string SUPERSONIC_WISDOM_RESOURCE_DIR_NAME = "SupersonicWisdom";
        private const string SUPERSONIC_WISDOM_SETTINGS_ASSET_RESOURCE_FILE_NAME = "Settings";
        private const string SUPERSONIC_WISDOM_ACCOUNT_DATA_ASSET_RESOURCE_FILE_NAME = "SwAccountData";
        internal const string SW_UPDATE_METADATA_FOLDER_NAME = "sw-update-metadata";
        private const string SHOULD_DRAW_IOS_CHINA_TAB_KEY = "shouldDrawIosChinaTab";
        public static readonly string SwUpdateMetadataFolderPath = Path.Combine(SwEditorConstants.ASSETS, SUPERSONIC_WISDOM_RESOURCE_DIR_NAME, SW_UPDATE_METADATA_FOLDER_NAME);

        #endregion
            
        
        #region --- Members ---

        private static readonly string SettingsAssetFileRelativePath = $"Resources/{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME}/{SUPERSONIC_WISDOM_SETTINGS_ASSET_RESOURCE_FILE_NAME}.asset";
        private static readonly string AccountDataAssetFileRelativePath = $"Resources/{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME}/{SUPERSONIC_WISDOM_ACCOUNT_DATA_ASSET_RESOURCE_FILE_NAME}.asset";
        private static SwSettingsManager<SwSettings> _settingsManager;
        private static readonly Lazy<SwMainThreadActionsQueue> LazyMainThreadActions = new Lazy<SwMainThreadActionsQueue>(() =>
        {
            // Register to main thread updates, the "Lazy"'s factory mechanism will ensure that it will occur only once.
            EditorApplication.update += OnEditorApplicationUpdate;

            return new SwMainThreadActionsQueue();
        });

        #endregion


        #region --- Properties ---

        public static string AppName
        {
            get { return string.IsNullOrEmpty(SwSettings.appName) ? Application.productName : SwSettings.appName; }
        }

        public static SwSettings SwSettings
        {
            get
            {
                if (_settingsManager == null)
                {
                    _settingsManager = new SwSettingsManager<SwSettings>($"{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME}/{SUPERSONIC_WISDOM_SETTINGS_ASSET_RESOURCE_FILE_NAME}");
                }

                var doesSettingsAssetFileExist = File.Exists($"{Application.dataPath}/{SettingsAssetFileRelativePath}");
                // Fix unity issue in post build script where Resource.Load doesn't always work on first run
                if (_settingsManager.Settings == null && doesSettingsAssetFileExist)
                {
                    var settings = AssetDatabase.LoadAssetAtPath($"Assets/{SettingsAssetFileRelativePath}", typeof(SwSettings)) as SwSettings;
                    _settingsManager.Load(settings);
                }

                return _settingsManager.Settings;
            }
        }

        public static bool AllowEditingSettings { get; set; }

        public static string FacebookAppId
        {
            set
            {
                if (FacebookSettings.AppIds.Count == 0)
                {
                    FacebookSettings.AppIds = new List<string> { value };
                    FacebookSettings.AppLabels = new List<string> { AppName };
                }
                else
                {
                    FacebookSettings.AppIds[0] = value;
                    FacebookSettings.AppLabels[0] = AppName;
                }

                EditorUtility.SetDirty(FacebookSettings.Instance);
            }
            get
            {
                if (FacebookSettings.AppIds.Count == 0) return "";

                return FacebookSettings.AppIds[0];
            }
        }
        
        public static string FacebookClientToken
        {
            set
            {
                if (FacebookSettings.ClientTokens.Count == 0)
                {
                    FacebookSettings.ClientTokens = new List<string> { value };
                }
                else
                {
                    FacebookSettings.ClientTokens[0] = value;
                }

                EditorUtility.SetDirty(FacebookSettings.Instance);
            }
            get
            {
                if (FacebookSettings.ClientTokens.Count == 0) return "";

                return FacebookSettings.ClientTokens[0];
            }
        }

        internal static SwAccountData SwAccountData
        {
            get
            {
                var assetPath = $"Assets/{AccountDataAssetFileRelativePath}";

                var swAccountData = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SwAccountData)) as SwAccountData;

                if (swAccountData == null)
                {
                    var swAccountDataAsset = ScriptableObject.CreateInstance<SwAccountData>();
                    AssetDatabase.CreateAsset(swAccountDataAsset, assetPath);
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();

                    swAccountData = swAccountDataAsset;
                }

                return swAccountData;
            }
        }
        
        internal static bool ShouldDrawIosChinaTab
        {
            get
            {
                return EditorPrefs.GetBool(SHOULD_DRAW_IOS_CHINA_TAB_KEY, false);
            }
            set
            {
                EditorPrefs.SetBool(SHOULD_DRAW_IOS_CHINA_TAB_KEY, value);
            }
        }

        private static SwMainThreadActionsQueue MainThreadActions
        {
            get { return LazyMainThreadActions.Value; }
        }

        #endregion


        #region --- Public Methods ---

        internal static void UpdateDefines(string entry, bool enabled, params BuildTargetGroup[] groups)
        {
            var edited = false;
            
            foreach (var group in groups)
            {
                var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries));

                if (enabled && !defines.Contains(entry))
                {
                    defines.Add(entry);
                    edited = true;
                }
                else if (!enabled && defines.Contains(entry))
                {
                    defines.Remove(entry);
                    edited = true;
                }

                if (edited)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines.ToArray()));
                }
            }
            
            if (edited)
            {
                // Save project settings only when Editor UI is open
                if (!Application.isBatchMode)
                {
                    AssetDatabase.SaveAssets();
                }
            }
        }

        internal static void CallOnNextUpdate(Action action)
        {
            new CallOnInspectorUpdate(action);
        }

        internal static async Task<bool> ImportPackage(string packagePath, bool isSilent)
        {
            var importListener = new SwImportPackageCallback(errorMessage =>
            {
                var didImport = errorMessage == null;
                
                if (!didImport)
                {
                    SwEditorLogger.LogError(errorMessage);
                }
            });

            try
            {
                SwEditorLogger.Log("Importing package...");
                AssetDatabase.StartAssetEditing();
                AssetDatabase.ImportPackage(packagePath, !isSilent);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                SwEditorLogger.Log("Importing package finished");
            }

            while (importListener.IsCompletedSuccessfully == null)
            {
                await Task.Delay(300);
            }
            
            return importListener.IsCompletedSuccessfully.Value;
        }

        internal static void OpenSettings()
        {
            if (SwSettings == null)
            {
                CreateSettings();
            }

            Selection.activeObject = SwSettings;
        }

        internal static float Percentage(float part, float total)
        {
            if (part == 0 || total == 0) return 0;

            return part / total * 100f;
        }

        /// <summary>
        /// Will return True if the major,minor and patch versions are equal or higher than the versionToCompare.
        /// </summary>
        /// <param name="versionToCompare">Unity version to compare with as string</param>
        /// <param name="major">Major version</param>
        /// <param name="minor">Minor version</param>
        /// <param name="patch">Patch version</param>
        /// <returns></returns>
        internal static bool IsAboveUnityVersion(string versionToCompare, int major, int minor, int patch)
        {
            try
            {
                var parsedUnityVersion = Regex.Match(versionToCompare, UNITY_VERSION_REGEX_PATTERN).Value.Split('.');
                int parsedMajor = int.TryParse(parsedUnityVersion[0], out parsedMajor) ? parsedMajor : 0;
                int parsedMinor = int.TryParse(parsedUnityVersion[1], out parsedMinor) ? parsedMinor : 0;
                int parsedPatch = int.TryParse(parsedUnityVersion[2], out parsedPatch) ? parsedPatch : 0;

                if (parsedMajor < major)
                {
                    return true;
                }
                else if (parsedMajor == major)
                {
                    if (parsedMinor < minor)
                    {
                        return true;
                    }
                    else if (parsedMinor == minor)
                    {
                        return parsedPatch <= patch;
                    }
                }
            }catch (Exception exception)
            {
                SwEditorLogger.LogError(exception);
            }

            return false;
        }

        /// <summary>
        ///     Runs a given action on the main thread (UI thread).
        ///     In general, when we want to present dialogs, progress bars or make operations like import packages, etc. we must
        ///     call it on the main thread. Otherwise the Unity Editor will ignore our request (it won't crash or throw an error,
        ///     but it will ignore).
        ///     Whenever the process returns from a <see cref="T:System.Threading.Tasks.Task" />, it runs on a secondary thread (
        ///     by a pool or custom). Therefore we should use this
        /// </summary>
        /// <param name="actionToRun">The action that will actually run on the main thread.</param>
        /// <param name="afterDelayMilliseconds">
        ///     The delay to wait before executing the action. Default value is zero (run
        ///     immediately)
        /// </param>
        internal static void RunOnMainThread(Action actionToRun, int afterDelayMilliseconds = 0)
        {
            if (afterDelayMilliseconds == 0)
            {
                void SafeRun()
                {
                    try
                    {
                        actionToRun();
                    }
                    catch (Exception mainThreadActionException)
                    {
                        SwEditorLogger.LogError(mainThreadActionException.ToString());
                    }
                }

                var isRunningOnMainThread = !Thread.CurrentThread.IsBackground;

                if (isRunningOnMainThread)
                {
                    SafeRun();
                }
                else
                {
                    MainThreadActions.Add(SafeRun);
                }
            }
            else
            {
                new Task(() =>
                {
                    Thread.Sleep(afterDelayMilliseconds);
                    RunOnMainThread(actionToRun);
                }).Start();
            }
        }
        
        internal static bool IsUnityIapAssetInstalled()
        {
            return DoesAssemblyExists(SwEditorConstants.IAP_ASSEMBLY_FULL_NAME);
        }
        
        internal static bool IsUnityGamingServicesAnalyticsInstalled()
        {
            return DoesAssemblyExists(SwEditorConstants.UGS_ANALYTICS_ASSEMBLY_FULL_NAME);
        }

        internal static bool IsUnityMobileNotificationsAssetInstalled()
        {
            return DoesAssemblyExists(SwEditorConstants.UNITY_MOBILE_NOTIFICATIONS_ASSEMBLY_FULL_NAME, SwEditorConstants.UNITY_MOBILE_NOTIFICATIONS_PACKAGE_VERSION);
        }

        internal static void OpenIronSourceIntegrationManager ()
        {
            var ironSourceMenuType = Type.GetType("IronSourceMenu, Assembly-CSharp-Editor");

            try
            {
                ironSourceMenuType.GetMethod("SdkManagerProd", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
            }
            catch (Exception)
            {
                Debug.LogError("Could not open IronSource Integration Manager");
            }
        }

        internal static void OpenUgsProjectSettings()
        {
            SettingsService.OpenProjectSettings("Project/Services");
        }

        internal static IEnumerator WaitUntilEndOfCompilation ()
        {
            yield return new WaitUntil(() => !EditorApplication.isCompiling);
        }
        
        public static void RepaintEditorWindow<T>(Type t) where T : UnityEditor.Editor
        {
            var editors = Resources.FindObjectsOfTypeAll(t);

            foreach (var obj in editors)
            {
                var editor = obj as UnityEditor.Editor;

                if (editor == null)
                {
                    continue;
                }

                editor.Repaint();
            }
        }
        
        public static string Plural(Int64 amount, string single)
        {
            return amount == 1 ? single : $"{single}s";
        }

        public static void DrawGuiLayoutSwLogoLabel(float dimensions)
        {
            GUILayout.Label(SwGameObjectLogo.Logo, GUILayout.Width(dimensions), GUILayout.Height(dimensions));
        }

        public static void DrawClickableButtonLink(string linkTitle, string linkUrl, Action onClick)
        {
            var previousLabelFontStyle = GUI.skin.label.fontStyle;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            var linkButtonWidth = GUI.skin.button.CalcSize(new GUIContent(linkTitle)).x;

            if (GUILayout.Button(new GUIContent(linkTitle, linkUrl), EditorStyles.linkLabel, GUILayout.MaxWidth(linkButtonWidth)))
            {
                onClick();
            }

            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
            GUI.skin.label.fontStyle = previousLabelFontStyle;
        }

        public static void FailBuildWithMessage(string tag, string message)
        {
            throw new BuildFailedException($"{tag} | {message}");
        }

        public static string GetWisdomPackageManifestFileName()
        {
            return $"{SwConstants.SDK_VERSION}-manifest.txt";
        }

        public static string GetCurrentBuildTargetName()
        {
#if UNITY_ANDROID
            return "Android";
#elif UNITY_IOS
            return "iOS";
#else
            return "General";
#endif
        }

        #endregion


        #region --- Private Methods ---

        private static void CreateSettings ()
        {
            try
            {
                if (_settingsManager.Settings == null)
                {
                    // If the settings asset doesn't exist, then create it. We require a resources folder
                    if (!Directory.Exists($"{Application.dataPath}/Resources"))
                    {
                        Directory.CreateDirectory($"{Application.dataPath}/Resources");
                    }

                    if (!Directory.Exists($"{Application.dataPath}/Resources/{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME}"))
                    {
                        Directory.CreateDirectory($"{Application.dataPath}/Resources/{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME}");
                        Debug.Log($"SupersonicWisdom: Resources/{SUPERSONIC_WISDOM_RESOURCE_DIR_NAME} folder is required to store settings. it was created ");
                    }

                    var assetPath = $"Assets/{SettingsAssetFileRelativePath}";

                    if (File.Exists(assetPath))
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                        AssetDatabase.Refresh();
                    }

                    var asset = ScriptableObject.CreateInstance<SwSettings>();
                    AssetDatabase.CreateAsset(asset, assetPath);
                    AssetDatabase.Refresh();

                    AssetDatabase.SaveAssets();
                    Debug.Log("SupersonicWisdom: Settings file didn't exist and was created");
                    Selection.activeObject = asset;

                    _settingsManager.Load();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("SupersonicWisdom: Error getting Settings in InitAPI: " + e.Message);
            }
        }

        private static bool DoesAssemblyExists(string assemblyFullName, string minVersion = "")
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOr(assembly => assembly.GetName().Name.Equals(assemblyFullName), null);
            var doesExist = assembly != null;
            
            if (!doesExist || string.IsNullOrEmpty(minVersion)) return doesExist;

            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);
            doesExist = SwUtils.System.ComputeVersionId(packageInfo.version) >= SwUtils.System.ComputeVersionId(minVersion);

            return doesExist;
        }

        private static void OnEditorApplicationUpdate()
        {
            MainThreadActions.Run();
        }
        
        public static string Md5(string filePath)
        {
            try
            {
                using var md5 = MD5.Create();
                using var stream = File.OpenRead(filePath);
                var md5Hash = md5.ComputeHash(stream);
                var md5Checksum = BitConverter.ToString(md5Hash).Replace("-", string.Empty).ToLowerInvariant();

                return md5Checksum;
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError($"Error occured while trying to compute MD5 hash for {filePath}: {e.Message}");

                return string.Empty;
            }
        }
        
        #endregion
    }

    internal class SwImportPackageCallback
    {
        #region --- Members ---

        private readonly AssetDatabase.ImportPackageCallback _onImportCompleted;
        private readonly AssetDatabase.ImportPackageFailedCallback _onImportFailed;
        public bool? IsCompletedSuccessfully { get; private set; }

        #endregion


        #region --- Construction ---

        /// <summary>
        /// This class registers itself as a callback, no need to pass it to any "import" method.
        /// As long as the object of this class lives, it will observe Editor import callbacks.
        /// Once this object is destroyed it will stop observing.
        /// </summary>
        /// <param name="onComplete"></param>
        public SwImportPackageCallback(Action<string> onComplete)
        {
            _onImportCompleted = packageName =>
            {
                UnregisterListeners();
                IsCompletedSuccessfully = true;
                onComplete?.Invoke(null);
            };

            _onImportFailed = (packageName, errorMessage) =>
            {
                UnregisterListeners();
                IsCompletedSuccessfully = false;
                onComplete?.Invoke(errorMessage);
            };

            RegisterListeners();
        }

        private void RegisterListeners()
        {
            AssetDatabase.importPackageCompleted += _onImportCompleted;
            AssetDatabase.importPackageFailed += _onImportFailed;
        }

        private void UnregisterListeners()
        {
            AssetDatabase.importPackageCompleted -= _onImportCompleted;
            AssetDatabase.importPackageFailed -= _onImportFailed;
        }

        ~SwImportPackageCallback()
        {
            UnregisterListeners();
        }
        
        #endregion
    }
}
