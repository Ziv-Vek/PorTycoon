using System;
using System.IO;
using SupersonicWisdomSDK.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public static class SwPbxProcessor
{
    #region --- Private Methods ---

    [PostProcessBuild(int.MaxValue)]
    internal static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
#if UNITY_IOS
        if (buildTarget == BuildTarget.iOS)
        {
            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject project = new PBXProject();
            project.ReadFromString(File.ReadAllText(projectPath));
            string[] allTargets;

#if UNITY_2019_3_OR_NEWER
            string mainTargetGuid = project.GetUnityMainTargetGuid();
            string unityFrameworkTargetGuid = project.GetUnityFrameworkTargetGuid();
            
            /* Fix for https://github.com/facebook/facebook-sdk-for-unity/issues/468 */
            project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            allTargets = new[] {mainTargetGuid, unityFrameworkTargetGuid, project.TargetGuidByName(PBXProject.GetUnityTestTargetName())};
            
            // Workaround for Apple's ERROR ITMS-90206: "Invalid Bundle. The bundle at 'YOUR.app/Frameworks/UnityFramework.framework' contains disallowed file 'Frameworks'."
            project.SetBuildProperty(unityFrameworkTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
#else
            string mainTargetGuid = project.TargetGuidByName("Unity-iPhone");
            /* Fix for https://github.com/facebook/facebook-sdk-for-unity/issues/468 */
            project.AddBuildProperty(mainTargetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
            allTargets = new[] {mainTargetGuid, project.TargetGuidByName("UnityFramework"), project.TargetGuidByName("Unity-iPhone Tests")};
#endif
            /* End of fix */
            
            // Disabling Bitcode for all targets.
            // When ENABLE_BITCODE = Yes then Firebase symbol parsing is not working properly
            foreach (var targetGuid in allTargets)
            {
                project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            }

            string projectStr = project.WriteToString();
            projectStr = projectStr.Replace("ENABLE_BITCODE = YES;", "ENABLE_BITCODE = NO;");

            File.WriteAllText(projectPath, projectStr);
            SwEditorTracker.TrackEditorEvent(nameof(OnPostProcessBuild), ESwEditorWisdomLogType.IOS, ESwEventSeverity.Info, "PBX processed");
        }
#endif
    }

    #endregion
}