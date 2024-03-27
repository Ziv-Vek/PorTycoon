using UnityEditor;
#if UNITY_2018_2_OR_NEWER
using TMPro.EditorUtilities;
#endif

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwTextMeshProUtils
    {
#if UNITY_2018_2_OR_NEWER
        public static void InstallEssentialsIfNeeded ()
        {
            if (!SwEditorUtils.SwSettings.areTextMeshProEssentialsInstalled)
            {
                var packageFullPath = TMP_EditorUtility.packageFullPath;
                AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
                AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
                AssetDatabase.ImportPackage(packageFullPath + "/Package Resources/TMP Essential Resources.unitypackage", false);
            }
        }

        private static void OnImportPackageCompleted(string packageName)
        {
            SwEditorUtils.SwSettings.areTextMeshProEssentialsInstalled = true;
            AssetDatabase.importPackageCompleted -= OnImportPackageCompleted;
        }
    }
#endif
}