#if UNITY_IOS
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwDeepLinkIosInjector
    {
        [PostProcessBuild(250)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                string plistPath = Path.Combine(path, "Info.plist");
                PlistDocument plist = new PlistDocument();
                plist.ReadFromFile(plistPath);
                
                InjectDeepLink(plist);
                
                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
        
        public static void InjectDeepLink(PlistDocument plist)
        {
            var bundleUrlTypes = plist.root.values.FirstOrDefault(keyValue => keyValue.Key.Equals("CFBundleURLTypes"));
            PlistElementArray bundleUrlTypesElement;
            if (bundleUrlTypes.Equals(default(KeyValuePair<string, PlistElement>)))
            {
                bundleUrlTypesElement = plist.root.CreateArray("CFBundleURLTypes");
            }
            else
            {
                bundleUrlTypesElement = bundleUrlTypes.Value as PlistElementArray;
            }

            var schemeAlreadyExists = false;
            string deepLinkScheme = SwDeepLinkHandler.GetDeepLinkScheme(SwEditorUtils.SwSettings.GetGameId());

            // ReSharper disable once PossibleNullReferenceException
            foreach (var element in bundleUrlTypesElement.values)
            {
                if (schemeAlreadyExists)
                {
                    break;
                }

                if (element is PlistElementDict)
                {
                    PlistElementDict elementDict = element.AsDict();
                    foreach (var subElement in elementDict.values)
                    {
                        if (subElement.Key.Equals("CFBundleURLSchemes") && subElement.Value is PlistElementArray)
                        {
                            PlistElementArray schemes = subElement.Value.AsArray();
                            PlistElement wisdomScheme = schemes.values.FirstOrDefault(scheme =>
                                scheme is PlistElementString && scheme.AsString().Equals(deepLinkScheme));
                            if (wisdomScheme != null)
                            {
                                schemeAlreadyExists = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (schemeAlreadyExists)
            {
                return;
            }

            PlistElementDict schemeDict = bundleUrlTypesElement.AddDict();
            schemeDict.SetString("CFBundleURLName", "supersonic-wisdom");
            PlistElementArray schemesElementArray = schemeDict.CreateArray("CFBundleURLSchemes");
            schemesElementArray.AddString(deepLinkScheme);
            SwEditorTracker.TrackEditorEvent(nameof(InjectDeepLink), ESwEditorWisdomLogType.IOS, ESwEventSeverity.Info, "Deep link injected");
        }
    }
}
#endif