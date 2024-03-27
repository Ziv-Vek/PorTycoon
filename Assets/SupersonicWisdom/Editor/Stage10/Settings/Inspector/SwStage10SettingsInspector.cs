#if SW_STAGE_STAGE10_OR_ABOVE

using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
#if SW_STAGE_STAGE10
    [CustomEditor(typeof(SwSettings))]
#endif
    internal class SwStage10SettingsInspector : SwCoreSettingsInspector
    {
        #region --- Members ---

        private bool _didChangeGameAnalyticsKeys;

        #endregion


        #region --- Private Methods ---

        protected override void AfterDrawSelectedTab ()
        {
            base.AfterDrawSelectedTab();

            if (_didChangeGameAnalyticsKeys)
            {
                EditorUtility.SetDirty(GameAnalytics.SettingsGA);
                _didChangeGameAnalyticsKeys = false;
            }
        }

        protected override void BeforeDrawSelectedTab ()
        {
            base.BeforeDrawSelectedTab();
            SwStage10EditorUtils.OnGameAnalyticsKeysChangedEvent -= OnGameAnalyticsKeysChanged;
            SwStage10EditorUtils.OnGameAnalyticsKeysChangedEvent += OnGameAnalyticsKeysChanged;
            _didChangeGameAnalyticsKeys = false;
        }

        protected override SwCoreSettingsTab[] StageTabs ()
        {
            return new SwCoreSettingsTab[]
            {
                new SwGeneralStage10SettingsTab(_soSettings),
                new SwIosStage10SettingsTab(_soSettings),
                new SwAndroidStage10SettingsTab(_soSettings),
                new SwDebugStage10SettingsTab(_soSettings),
            };
        }

        #endregion


        #region --- Event Handler ---

        private void OnGameAnalyticsKeysChanged ()
        {
            _didChangeGameAnalyticsKeys = true;
        }

        #endregion
    }
}
#endif