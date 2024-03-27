using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    public static class SwAndroidManifestUtils
    {
        #region --- Constants ---

        internal const string ApplicationElement = "application";
        internal const string ManifestElement = "manifest";
        internal const string TargetManifestPath = "Plugins/Android/AndroidManifest.xml";
        internal const string ToolsPath = "http://schemas.android.com/tools";

        #endregion


        #region --- Members ---

        internal static readonly XNamespace AndroidNamespace = "http://schemas.android.com/apk/res/android";
        internal static readonly XNamespace Namespace = "http://www.w3.org/2000/xmlns/";
        internal static readonly XNamespace ToolsNamespace = "http://schemas.android.com/tools";

        #endregion


        #region --- Private Methods ---

        internal static string GetAndroidManifestPath ()
        {
            return Path.Combine(Application.dataPath, TargetManifestPath);
        }

        internal static XDocument LoadManifest ()
        {
            try
            {
                return XDocument.Load(GetAndroidManifestPath());
            }
            catch (Exception e)
            {
                SwEditorLogger.LogWarning($"Exception: {e.Message}");

                return null;
            }
        }

        #endregion
    }
}