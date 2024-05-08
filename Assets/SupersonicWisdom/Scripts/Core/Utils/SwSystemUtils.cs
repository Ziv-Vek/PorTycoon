using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Object = UnityEngine.Object;
#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

namespace SupersonicWisdomSDK
{
    internal class SwSystemUtils
    {
        #region --- Members ---
        private readonly Lazy<string> _lazyPlatformDisplayName = new Lazy<string>(() => IsAndroidTarget() ? "Android" : IsIosTarget() ? "iOS" : "Unknown");
        private static readonly Lazy<byte[]> LazyAppIconBytes = new Lazy<byte[]>(() => Resources.Load<TextAsset>(SwConstants.AppIconResourcesPath)?.bytes);

        #endregion


        #region --- Constants ---

        private const short NON_BETA_VERSION_COMPONENT = 99;

        #endregion


        #region --- Properties ---
        
        private readonly Lazy<Sprite> LazyAppIconSprite = new Lazy<Sprite>(() =>
        {
            if (AppIconBytes == null) return null;

            // Texture width & height are dummy since they will be overwritten!
            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(AppIconBytes);
            var appIconSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(.5f, .5f));

            return appIconSprite;
        });
        
        private readonly Lazy<string> LazyUnityVersion = new Lazy<string>(() =>
        {
            var formattedUnityVersion = "";
            var unityVersionParts = Application.unityVersion.Split('.');

            for (var i = 0; i < unityVersionParts.Length; i++)
            {
                if (int.TryParse(unityVersionParts[i], out _))
                {
                    if (i == 0)
                    {
                        formattedUnityVersion = unityVersionParts[i];
                    }
                    else
                    {
                        formattedUnityVersion += "." + unityVersionParts[i];
                    }
                }
                else
                {
                    var regexVersion = Regex.Split(unityVersionParts[i], "[^\\d]+");

                    if (regexVersion.Length > 0 && int.TryParse(regexVersion[0], out _))
                    {
                        formattedUnityVersion += "." + regexVersion[0];
                    }
                }
            }

            return formattedUnityVersion;
        });
        
        public string UnityVersion
        {
            get { return LazyUnityVersion.Value; }
        }
        
        public string PlatformDisplayName
        {
            get { return _lazyPlatformDisplayName.Value; }
        }
        
        public Sprite AppIconSprite
        {
            get { return LazyAppIconSprite.Value; }
        }
        
        public static byte[] AppIconBytes
        {
            get { return LazyAppIconBytes.Value; }
        }
        
        public bool IsRunningOnMainThread
        {
            get { return SwInfra.MainThread == Thread.CurrentThread; }
        }
        
        public string AppVersion
        {
            get { return Application.version; }
        }
        
        public string PlatformVersion
        {
            get { return SystemInfo.operatingSystem; }
        }
        
        public string DeviceModel
        {
            get { return SystemInfo.deviceModel; }
        }
        
        public string BundleIdentifier
        {
            get { return Application.identifier; }
        }
        
        public bool IsDevelopmentBuild
        {
            get
            {
                return
#if DEVELOPMENT_BUILD
                    true;
#else
                    false;
#endif

            }
        }
        
        public bool IsSdkDevelopmentBuild
        {
            get
            {
                return SwConstants.SdkVersionId == 0;
            }
        }

        #endregion


        #region --- Public Methods ---

        internal long ComputeVersionId(string sdkVersion)
        {
            if (sdkVersion == null) return 0;

            short major = 0, minor = 0, patch = 0, beta = 0;

            var components = sdkVersion.Split('.', '-');

            if (components.Length >= 3)
            {
                major = Convert.ToInt16(components[0]);
                minor = Convert.ToInt16(components[1]);
                patch = Convert.ToInt16(components[2]);
                beta = NON_BETA_VERSION_COMPONENT;
            }

            if (components.Length == 4)
            {
                beta = Convert.ToInt16(components[3]);
            }

            return (long)(major * 1e6 + minor * 1e4 + patch * 1e2 + beta);
        }

        internal long ComputeUnityVersionId(string unityVersion)
        {
            //Remove the ending letter from the unity version
            unityVersion = unityVersion.Split('a', 'b', 'f', 'p')[0];

            return ComputeVersionId(unityVersion);
        }
        
        internal string ComputeVersionString(long sdkId)
        {
            var major = (int)(sdkId / 1e6 % 100);
            var minor = (int)(sdkId / 1e4 % 100);
            var patch = (int)(sdkId / 1e2 % 100);
            var beta = (int)(sdkId / 1e0 % 100);
            var betaString = "";

            if (beta != 0 && beta != NON_BETA_VERSION_COMPONENT)
            {
                betaString = "-" + beta;
            }

            return $"{major}.{minor}.{patch}{betaString}";
        }
        
        public bool IsRunningOnEditor()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
        
        public bool IsRunningOnDevice()
        {
            return !IsRunningOnEditor();
        }
        
        public bool IsRunningOnAndroid()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
        
        public bool IsRunningOnIos()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
        
        public static bool IsAndroidTarget()
        {
#if UNITY_ANDROID
            return true;
#else
            return false;
#endif
        }
        
        public static bool IsIosTarget()
        {
#if UNITY_IOS
            return true;
#else
            return false;
#endif
        }
        
        internal string GetOperatingSystemVersion()
        {
            var osDescription = SystemInfo.operatingSystem;

            if (IsWindows(osDescription))
            {
                return ExtractVersion(osDescription, "Windows");
            }

            if (IsUnix(osDescription))
            {
                return ExtractVersion(osDescription, "Unix");
            }
            
            if (IsMacOS(osDescription))
            {
                return ExtractVersion(osDescription, "X");
            }

            return IsLinux(osDescription) ? ExtractVersion(osDescription, "Linux") : osDescription;
        }

        internal string GetOperatingSystemName()
        {
            var osDescription = SystemInfo.operatingSystem;

            if (IsWindows(osDescription))
            {
                return "Windows";
            }

            if (IsMacOS(osDescription))
            {
                return "Mac OS";
            }

            if (IsUnix(osDescription))
            {
                return "Unix";
            }

            return IsLinux(osDescription) ? "Linux" : osDescription;
        }
        
        internal string SerializeToQueryString(Dictionary<string, object> dict)
        {
            if (dict == null) return "";
            if (dict.Count == 0) return "";

            var serializedKeyValue = dict.Where(keyValue => keyValue.Value != null).Select(keyValue => $"{keyValue.Key}={Uri.EscapeDataString(keyValue.Value.ToString())}");

            return string.Join("&", serializedKeyValue);
        }

        public T Load<T>(string prefabPath) where T : Object
        {
            var suffix = SwUtils.Ui.IsLandscapeLayout() ? "l" : "p";

            return Resources.Load<T>($"{prefabPath}_{suffix}");
        }
        
        internal Dictionary<string, string> DeserializeQueryString(string str)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(str)) return result;

            var parts = str.Split('?');

            if (parts.Length == 0) return result;

            var queryString = parts.Last();
            var pairs = queryString.Split('&');

            foreach (var pair in pairs)
            {
                var entryPair = pair.Split('=');

                if (entryPair.Length == 2)
                {
                    result[entryPair[0]] = UnityWebRequest.UnEscapeURL(entryPair[1]);
                }
            }

            return result;
        }

        #endregion
        
        
        #region --- Private Methods ---
        
        private bool IsWindows(string osDescription)
        {
            return osDescription.Contains("Windows");
        }

        private bool IsMacOS(string osDescription)
        {
            return osDescription.Contains("OS X") || osDescription.Contains("Mac");
        }

        private bool IsUnix(string osDescription)
        {
            return osDescription.Contains("Unix");
        }

        private bool IsLinux(string osDescription)
        {
            return osDescription.Contains("Linux");
        }

        private string ExtractVersion(string osDescription, string osName)
        {
            var splitOs = osDescription.Split(' ');
            var versionIndex = Array.IndexOf(splitOs, osName);
    
            if (versionIndex != -1 && versionIndex + 1 < splitOs.Length)
            {
                return splitOs[versionIndex + 1];
            }

            return osDescription;
        }
        
        #endregion
    }
}