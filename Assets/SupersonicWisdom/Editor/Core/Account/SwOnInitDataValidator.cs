using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwOnInitDataValidator
    {
        #region --- Public Methods ---

        public static void ValidateMandatoryParameters ()
        {
#if !SUPERSONIC_WISDOM_TEST
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            
            if (CheckIfThereAreMissingParams())
            {
                if (EditorUtility.DisplayDialog(SwEditorConstants.UI.SETTING_PARAMETER_MISSING_ERROR, ErrorMessageStringBuilder.ToString(), SwEditorConstants.UI.ButtonTitle.GO_TO_SETTINGS, SwEditorConstants.UI.ButtonTitle.CLOSE))
                {
                    SwCoreSettingsInspector.SelectedTabIndex = 0;
                    SwMenu.AllowEditingSettings();
                    SwEditorUtils.OpenSettings();
                }
            }
#endif
        }

        #endregion


        #region --- Private Variables ---

        private static readonly HashSet<SwSettingsValidator.MissingParam> MissingParamsForEditor = new HashSet<SwSettingsValidator.MissingParam>
        {
            SwSettingsValidator.MissingParam.FbClientToken,
            SwSettingsValidator.MissingParam.AdmobAppId,
        };

        private static readonly StringBuilder ErrorMessageStringBuilder = new StringBuilder();

        #endregion


        #region --- Private Properties ---

        private static bool CheckIfThereAreMissingParams ()
        {
            var settingValidator = new SwSettingsValidatorFactory();
            var missingParams = new List<SwSettingsValidator.MissingParam>();
            
            #if UNITY_ANDROID
            missingParams = settingValidator.getSettingsValidator(BuildTarget.Android).GetMissingParams();
            #elif UNITY_IOS
            missingParams = settingValidator.getSettingsValidator(BuildTarget.iOS).GetMissingParams();
            #endif
            
            if(missingParams.Count == 0)
            {
                return false;
            }

            missingParams.ForEach(param =>
            {
                if (MissingParamsForEditor.Contains(param))
                {
                    var message = SwEditorConstants.UI.SETTING_PARAMETER_MISSING_ERROR_MESSAGE.Format(param);
                    AddCustomMessageIfOneExists(param, message);
                    Debug.LogError(message);
                }
            });

            return ErrorMessageStringBuilder.Length > 0;
        }

        private static void AddCustomMessageIfOneExists(SwSettingsValidator.MissingParam missingParam, string defaultMessage)
        {
            switch (missingParam)
            {
                case SwSettingsValidator.MissingParam.FbClientToken:
                    ErrorMessageStringBuilder.AppendLine(SwEditorConstants.UI.PrebuildMessages.FB_CLIENT_TOKEN_MISSING_MESSAGE);

                    break;
                
                case SwSettingsValidator.MissingParam.AdmobAppId:
                    ErrorMessageStringBuilder.AppendLine(SwEditorConstants.UI.PrebuildMessages.ADMOB_APP_ID_MISSING_MESSAGE.Format(SwEditorUtils.GetCurrentBuildTargetName()));

                    break;
                
                default:
                    ErrorMessageStringBuilder.AppendLine(defaultMessage);

                    break;
            }
        }

        #endregion


    }
}