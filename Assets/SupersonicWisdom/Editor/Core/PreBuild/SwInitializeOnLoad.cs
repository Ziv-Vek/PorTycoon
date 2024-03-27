using System.IO;
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    [InitializeOnLoad]
    internal static class SwInitializeOnLoad
    {
        #region --- Constants ---

        private const string IOS_CHINA_POPUP_MESSAGE = "Since iOS CN is enabled in your wisdom \nsettings, this build will be created with iOS CN \nsettings. Are you sure?";
        private const string IOS_CHINA_POPUP_TITLE = "iOS China Build";

        #endregion


        #region --- Construction ---

        static SwInitializeOnLoad()
        {
            if (SwEditorUtils.SwSettings != null)
            {
                if (!SwEditorUtils.SwSettings.areTextMeshProEssentialsInstalled)
                {
                    if (Directory.Exists("Assets/TextMesh Pro"))
                    {
                        SwEditorUtils.SwSettings.areTextMeshProEssentialsInstalled = true;
                    }
                }

                SwTextMeshProUtils.InstallEssentialsIfNeeded();
                BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuild);

                //Methods registered below will be called once the editor is fully loaded
                EditorApplication.delayCall += () =>
                {
                    SwOnInitDataValidator.ValidateMandatoryParameters();
                };
            }
        }

        #endregion


        private static void OnBuild(BuildPlayerOptions buildPlayerOptions)
        {
            var isDevelopmentBuild = (buildPlayerOptions.options & BuildOptions.Development) != 0;
            var shouldBuild = true;

            if (SwEditorUtils.SwSettings.iosChinaBuildEnabled)
            {
                if (!EditorUtility.DisplayDialog(IOS_CHINA_POPUP_TITLE, IOS_CHINA_POPUP_MESSAGE, "Ok", "Cancel"))
                {
                    return;
                }
            }

            if (!isDevelopmentBuild)
            {
                var flags = SwEditorUtils.SwSettings.EnabledDevelopmentFlags;
                shouldBuild = flags.Length == 0 || EditorUtility.DisplayDialog("SupersonicWisdom SDK Warning", $"The following development flags are activated:\n\n{string.Join("\n", flags)}\n\nAre you sure you want to proceed?", "Yes", "No");
                ForceDisableReleaseFlags();
            }

            shouldBuild = shouldBuild && CheckPlatformSettings(buildPlayerOptions.target);

            if (shouldBuild)
            {
                BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
            }
        }

        private static bool CheckPlatformSettings(BuildTarget target)
        {
            var validatorFactory = new SwSettingsValidatorFactory();
            var validator = validatorFactory.getSettingsValidator(target);
            var param = validator.GetMissingParam();
            var isValid = param == SwSettingsValidator.MissingParam.No;

            if (!isValid)
            {
                validator.HandleMissingParam(param);
            }

            var duplicateIapParam = validator.CheckIapForDuplicates();
            isValid = duplicateIapParam == SwSettingsValidator.IapDuplicateParam.No;

            if (!isValid)
            {
                validator.HandleDuplicateIap(duplicateIapParam);
            }

            return isValid;
        }
        
        private static void ForceDisableReleaseFlags()
        {
            SwEditorUtils.SwSettings.enableDevtools = false;
        }
    }
}