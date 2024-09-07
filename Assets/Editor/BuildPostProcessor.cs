//
//  Copyright (c) 2022 Tenjin. All rights reserved.
//

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

public class BuildPostProcessor : MonoBehaviour
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            BuildiOS(path: path);
        }
        else if (buildTarget == BuildTarget.Android)
        {
            BuildAndroid(path: path);
        }
    }

#if UNITY_IOS
    [PostProcessBuild(50)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            EmbedSignFramework(path);
        }
    }

    public static void EmbedSignFramework(string path)
    {
        string projPath = PBXProject.GetPBXProjectPath(path);
        if (!File.Exists(projPath))
        {
            Debug.LogError("Project file does not exist: " + projPath);
            return;
        }
        
        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));

        // Get the target GUID
        string unityFrameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();
        string targetGuid = proj.GetUnityMainTargetGuid();

        // Unzip the .xcframework to the Frameworks folder in the Xcode project
        string zipPathInUnity = "Assets/Plugins/iOS/TenjinSDK.xcframework.zip";
        string extractionPath = Path.Combine(path, "Frameworks");
        ZipFile.ExtractToDirectory(zipPathInUnity, extractionPath);

        // Add the .xcframework to the Xcode project and embed it in the main target
        string frameworkPath = Path.Combine(extractionPath, "TenjinSDK.xcframework");
        string fileGuid = proj.AddFile(frameworkPath, "Frameworks/TenjinSDK.xcframework");
        proj.AddFileToEmbedFrameworks(targetGuid, fileGuid);

        string fileGuidForUnityFramework = proj.AddFile(frameworkPath, "Frameworks/TenjinSDK.xcframework");

        // Add the .xcframework to the Xcode project's build phase
        var mainLinkPhaseGuid = proj.GetFrameworksBuildPhaseByTarget(targetGuid);
        var unityFrameworkLinkPhaseGuid = proj.GetFrameworksBuildPhaseByTarget(unityFrameworkTargetGuid);

        proj.AddFileToBuildSection(targetGuid, mainLinkPhaseGuid, fileGuid);
        proj.AddFileToBuildSection(unityFrameworkTargetGuid, unityFrameworkLinkPhaseGuid, fileGuidForUnityFramework);

        File.WriteAllText(projPath, proj.WriteToString());
    }
#endif

    private static void BuildAndroid(string path = "")
    {
        Debug.Log("TenjinSDK: Starting Android Build");
    }

    private static void BuildiOS(string path = "")
    {
#if UNITY_IOS
        Debug.Log("TenjinSDK: Starting iOS Build");

        string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);

#if UNITY_2019_3_OR_NEWER
        string buildTarget = project.GetUnityFrameworkTargetGuid();
#else
    string buildTarget = project.TargetGuidByName("Unity-iPhone");
#endif

        List<string> frameworks = new List<string>();

        frameworks.Add("AdServices.framework");
        frameworks.Add("AdSupport.framework");
        frameworks.Add("AppTrackingTransparency.framework");
        frameworks.Add("iAd.framework");
        frameworks.Add("StoreKit.framework");

        foreach (string framework in frameworks)
        {
            Debug.Log("TenjinSDK: Adding framework: " + framework);
            project.AddFrameworkToProject(buildTarget, framework, true);
        }

        Debug.Log("TenjinSDK: Adding -ObjC flag to other linker flags (OTHER_LDFLAGS)");
        project.AddBuildProperty(buildTarget, "OTHER_LDFLAGS", "-ObjC");

        File.WriteAllText(projectPath, project.WriteToString());

        // Add NSUserTrackingUsageDescription to Info.plist
        string plistPath = Path.Combine(path, "Info.plist");
        PlistDocument plist = new PlistDocument();
            
        plist.ReadFromFile(plistPath);

        plist.root.SetString("NSUserTrackingUsageDescription", 
                "We request to track data to enhance ad performance and user experience. Your privacy is respected.");

        File.WriteAllText(plistPath, plist.WriteToString());
#endif  
    }
}
