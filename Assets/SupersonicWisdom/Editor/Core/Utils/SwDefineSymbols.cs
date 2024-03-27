using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwDefineSymbols
    {
        #region --- Constants ---

        private const string IS_TIME_BASED = "SW_IS_TIME_BASED";

        #endregion


        #region --- Public Methods ---

        public virtual void UpdateRequiredSymbols()
        {
            UpdateIsTimeBased();
        }

        #endregion


        #region --- Private Methods ---

        private static void UpdateIsTimeBased()
        {
            if (SwEditorUtils.SwSettings == null) return;
            
            SwEditorUtils.UpdateDefines(IS_TIME_BASED, SwEditorUtils.SwSettings.isTimeBased, BuildTargetGroup.Android, BuildTargetGroup.iOS);
        }

        #endregion
    }
}