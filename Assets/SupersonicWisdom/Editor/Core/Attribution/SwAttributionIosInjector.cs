#if UNITY_IOS
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwAttributionIosInjector
    {
        [PostProcessBuild(200)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                string plistPath = Path.Combine(path, "Info.plist");
                PlistDocument plist = new PlistDocument();
                plist.ReadFromFile(plistPath);    
                
                plist.root.SetString(SwAttributionConstants.AdvertisingAttributionReportEndpointKey, SwAttributionConstants.AdvertisingAttributionReportEndpoint);

                try
                {
                    string[] networks = SwSkAdNetworkUtil.FetchSkAdNetworks();
                    SwSkAdNetworkUtil.InjectSkAdNetworks(plist, networks);
                }
                catch (Exception e)
                {
                    var errorPlist = $"An error had occurred while adding SKAdNetworks to the Info.plist: {e.Message}";
                    SwEditorTracker.TrackEditorEvent(nameof(OnPostProcessBuild), ESwEditorWisdomLogType.IOS, ESwEventSeverity.Error, $"{errorPlist}: {e.StackTrace}");
                    SwEditorLogger.LogError(errorPlist);
                    SwEditorLogger.LogError($"The built XCode Project in {path} is invalid and the app will quit immediately.");
                }
                finally
                {
                    File.WriteAllText(plistPath, plist.WriteToString());   
                }
            }
        }
    }
}
#endif