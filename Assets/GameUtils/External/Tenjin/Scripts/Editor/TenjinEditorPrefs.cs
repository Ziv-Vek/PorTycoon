//
//  Copyright (c) 2022 Tenjin. All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;

using UnityEngine;

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Compilation;
using UnityEditor.Build.Content;
using UnityEditor.Callbacks;

using System.Reflection;
using Assembly = System.Reflection.Assembly;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#endif

namespace Tenjin
{
    class TenjinEditorPrefs : IPreprocessBuildWithReport
    {
        private static string tenjin_admob = "tenjin_admob_enabled";
        private static string tenjin_applovin = "tenjin_applovin_enabled";
        private static string tenjin_facebook = "tenjin_facebook_enabled";
        private static string tenjin_hyperbid = "tenjin_hyperbid_enabled";
        private static string tenjin_ironsource = "tenjin_ironsource_enabled";
        private static string tenjin_topon = "tenjin_topon_enabled";
        private static string tenjin_cas = "tenjin_cas_enabled";
        private static string tenjin_tradplus = "tenjin_tradplus_enabled";


        public int callbackOrder => 0;

        private static bool isReloadingScripts = false;

        public void OnPreprocessBuild(BuildReport report)
        {
            TenjinEditorPrefs.Update3rdPartyIntegrations();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (isReloadingScripts)
            {
                return;
            }
            isReloadingScripts = true;

            try
            {
                Update3rdPartyIntegrations();
            }
            finally
            {
                isReloadingScripts = false;
            }
        }

        private static void Update3rdPartyIntegrations()
        {
            UpdateAdMob();
            UpdateAppLovin();
            UpdateFacebook();
            UpdateHyperBid();
            UpdateIronSource();
            UpdateTopOn();
            UpdateCAS();
            UpdateTradPlus();
        }

        #region third Party Library Detection
        private static void UpdateDefines(string entry, bool enabled, BuildTargetGroup[] groups)
        {
            foreach (var group in groups)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Where(d => d != entry);
                if (enabled)
                {
                    defines = defines.Concat(new[] { entry });
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defines.ToArray()));
            }
        }

        private static bool TypeExists(params string[] types)
        {
            if (types == null || types.Length == 0)
            {
                return false;
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    foreach (var typeValue in types)
                    {
                        if (type.Namespace == typeValue)
                        {
                            return true;
                        }
                        if (type.FullName == typeValue)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Sets the scripting define symbol `tenjin_admob_enabled` to true if AdMob classes are detected within the Unity project
        /// </summary>
        private static void UpdateAdMob()
        {
            var abmobTypes = new string[] { "GoogleMobileAds.Common", "GoogleMobileAds.Api" };
            if (TypeExists(abmobTypes))
            {
                UpdateDefines(tenjin_admob, true, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
            else
            {
                UpdateDefines(tenjin_admob, false, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
        }

        /// <summary>
        /// Sets the scripting define symbol `tenjin_applovin_enabled` to true if AppLovin classes are detected within the Unity project
        /// </summary>
        private static void UpdateAppLovin()
        {
            var applovinTypes = new string[] { "MaxSdkBase", "MaxSdkCallbacks" };
            if (TypeExists(applovinTypes))
            {
                UpdateDefines(tenjin_applovin, true, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
            else
            {
                UpdateDefines(tenjin_applovin, false, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
        }

        /// <summary>
        /// Sets the scripting define symbol `tenjin_facebook_enabled` to true if Facebook classes are detected within the Unity project
        /// </summary>
        private static void UpdateFacebook()
        {
            var facebookTypes = new string[] { "Facebook", "FB" };
            if (TypeExists(facebookTypes))
            {
                UpdateDefines(tenjin_facebook, true, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
            else
            {
                UpdateDefines(tenjin_facebook, false, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
        }

        /// <summary>
        /// Sets the scripting define symbol `tenjin_hyperbid_enabled` to true if HyperBid classes are detected within the Unity project
        /// </summary>
        private static void UpdateHyperBid()
        {
            var hyperbidTypes = new string[] { "HyperBid.Api", "HyperBid.Api.HBCallbackInfo" };
            if (TypeExists(hyperbidTypes))
            {
                UpdateDefines(tenjin_hyperbid, true, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
            else
            {
                UpdateDefines(tenjin_hyperbid, false, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
        }

        /// <summary>
        /// Sets the scripting define symbol `tenjin_ironsource_enabled` to true if IronSource classes are detected within the Unity project
        /// </summary>
        private static void UpdateIronSource()
        {
            var ironsourceTypes = new string[] { "IronSource", "IronSourceEvents" };
            if (TypeExists(ironsourceTypes))
            {
                UpdateDefines(tenjin_ironsource, true, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
            else
            {
                UpdateDefines(tenjin_ironsource, false, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
        }

        /// <summary>
        /// Sets the scripting define symbol `tenjin_topon_enabled` to true if TopOn classes are detected within the Unity project
        /// </summary>
        private static void UpdateTopOn()
        {
            var toponTypes = new string[] { "AnyThinkAds.Api", "AnyThinkAds.Api.ATCallbackInfo" };
            if (TypeExists(toponTypes))
            {
                UpdateDefines(tenjin_topon, true, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
            else
            {
                UpdateDefines(tenjin_topon, false, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
        }

        /// <summary>
        /// Sets the scripting define symbol `tenjin_cas_enabled` to true if CAS classes are detected within the Unity project
        /// </summary>
        private static void UpdateCAS()
        {
            var casTypes = new string[] { "CAS.IMediationManager", "CAS.IAdView" };
            if (TypeExists(casTypes))
            {
                UpdateDefines(tenjin_cas, true, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
            else
            {
                UpdateDefines(tenjin_cas, false, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
        }

        /// <summary>
        /// Sets the scripting define symbol `tenjin_tradplus_enabled` to true if TradPlus classes are detected within the Unity project
        /// </summary>
        private static void UpdateTradPlus()
        {
            var tradplusTypes = new string[] { "TradplusAds" };
            if (TypeExists(tradplusTypes))
            {
                UpdateDefines(tenjin_tradplus, true, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
            else
            {
                UpdateDefines(tenjin_tradplus, false, new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android });
            }
        }
        #endregion
    }
}
