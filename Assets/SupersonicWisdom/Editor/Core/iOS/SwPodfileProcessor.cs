using System;
using System.IO;
using System.Linq;
using SupersonicWisdomSDK.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using System.Text.RegularExpressions;
using UnityEditor.iOS.Xcode;
using UnityEngine;
#endif

internal static class SwPodfileProcessor
{
    #region --- Public Methods ---

    [PostProcessBuildAttribute(45)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
#if UNITY_IOS
        if (buildTarget == BuildTarget.iOS)
        {
            string[] versionParts = Application.unityVersion.Split('.');
            var majorVersion = Int16.Parse(versionParts[0]);
            var minorVersion = Int16.Parse(versionParts[1]);
            if (majorVersion <= 2017)
            {
                var fileName = path + "/Podfile";
                var endTag = "platform :ios, '9.0'";
                var lineToAdd = "use_frameworks!";
                var txtLines = File.ReadAllLines(fileName).ToList();
                txtLines.Insert(txtLines.IndexOf(endTag) + 1, lineToAdd);
                File.WriteAllLines(fileName, txtLines);
            }
        }
        
        SwEditorTracker.TrackEditorEvent(nameof(OnPostProcessBuild), ESwEditorWisdomLogType.IOS, ESwEventSeverity.Info, "Podfile processed");
#endif
    }

    #endregion
}