#if SW_STAGE_STAGE10_OR_ABOVE
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwDebugStage10SettingsTab : SwDebugCoreSettingsTab
    {
        #region --- Construction ---

        public SwDebugStage10SettingsTab(SerializedObject soSettings) : base(soSettings)
        { }

        #endregion
    }
}
#endif