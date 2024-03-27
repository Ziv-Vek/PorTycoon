using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    public static class SwStageSymbolsUtils
    {
        #region --- Constants ---

        private const string SUPERSONIC_WISDOM_STAGE_DEFINE_SYMBOL_PREFIX = "SW_STAGE_";

        #endregion


        #region --- Members ---

        private static readonly BuildTargetGroup[] StageDefineSymbolsBuildTargetGroups =
        {
            BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS
        };

        #endregion


        #region --- Public Methods ---

        public static void ApplyStage(SwStage stage)
        {
            if (ShouldSetStageSymbols(stage.defineSymbols))
            {
                ClearAllStageSymbols();
                SetDefineSymbols(stage.defineSymbols);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        #endregion


        #region --- Private Methods ---

        private static void ClearAllStageSymbols ()
        {
            foreach (var group in StageDefineSymbolsBuildTargetGroups)
            {
                var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                var originalCount = defines.Count;
                defines = defines.Where(define => !define.StartsWith(SUPERSONIC_WISDOM_STAGE_DEFINE_SYMBOL_PREFIX)).ToList();

                if (defines.Count < originalCount)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines.ToArray()));
                }
            }
        }

        private static void SetDefineSymbols(string[] defineSymbols)
        {
            foreach (var group in StageDefineSymbolsBuildTargetGroups)
            {
                var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                var originalCount = defines.Count;
                var definesSet = new HashSet<string>(defines);

                foreach (var define in defineSymbols)
                {
                    definesSet.Add(define);
                }

                defines = definesSet.ToList();

                if (defines.Count > originalCount)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines.ToArray()));
                }
            }
        }

        private static bool ShouldSetStageSymbols(string[] stageDefineSymbols)
        {
            if (stageDefineSymbols.Length == 0)
            {
                return false;
            }

            foreach (var group in StageDefineSymbolsBuildTargetGroups)
            {
                var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                var definesWithSwStagePrefix = defines.Where(define => define.StartsWith(SUPERSONIC_WISDOM_STAGE_DEFINE_SYMBOL_PREFIX)).ToList();

                if (stageDefineSymbols.Length != definesWithSwStagePrefix.Count)
                {
                    return true;
                }

                return !new HashSet<string>(stageDefineSymbols).SetEquals(definesWithSwStagePrefix);
            }

            return false;
        }

        #endregion
    }
}