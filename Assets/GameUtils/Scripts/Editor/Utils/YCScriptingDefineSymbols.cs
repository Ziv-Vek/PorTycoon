using UnityEditor;

namespace YsoCorp {
    namespace GameUtils {

        public class YCScriptingDefineSymbols {

            public static void AddDefineSymbolsForGroups(string def, params BuildTargetGroup[] groups) {
                AddDefineSymbol(def, BuildTargetGroup.Standalone);
                foreach (BuildTargetGroup group in groups) {
                    AddDefineSymbol(def, group);
                }
                AssetDatabase.SaveAssets();
            }

            public static void AddDefineSymbolsForMobile(string def) {
                AddDefineSymbolsForGroups(def, BuildTargetGroup.iOS, BuildTargetGroup.Android);
            }

            public static void RemoveDefineSymbolsForGroups(string def, params BuildTargetGroup[] groups) {
                RemoveDefineSymbol(def, BuildTargetGroup.Standalone);
                foreach (BuildTargetGroup group in groups) {
                    RemoveDefineSymbol(def, group);
                }
                AssetDatabase.SaveAssets();
            }

            public static void RemoveDefineSymbolsForMobile(string def) {
                RemoveDefineSymbolsForGroups(def, BuildTargetGroup.iOS, BuildTargetGroup.Android);
            }

            private static void AddDefineSymbol(string def, BuildTargetGroup group) {
                string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
                if (current.Contains(def) == false) {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, current + ";" + def);
                }
            }

            private static void RemoveDefineSymbol(string def, BuildTargetGroup group) {
                string current = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
                if (current.Contains(def)) {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, current.Replace(";" + def, ""));
                }
            }
        }

    }
}