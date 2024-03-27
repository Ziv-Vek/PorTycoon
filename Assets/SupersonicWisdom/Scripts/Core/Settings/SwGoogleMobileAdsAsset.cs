using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace SupersonicWisdomSDK
{
    internal static class SwGoogleMobileAdsAsset
    {
        private static readonly Lazy<Type> LazyGoogleMobileAdsSettingsType = new Lazy<Type>(() =>
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOr(a => a.FullName.Contains("GoogleMobileAds"), null)?.GetTypes().FirstOr(t => t.FullName?.Contains("GoogleMobileAdsSettings") ?? false, null);
        });
        
        public static Object GoogleMobileAdsSettingsInstance
        {
            get
            {
                try
                {
                    var value = LazyGoogleMobileAdsSettingsType.Value?.GetMethod("LoadInstance", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, new object[] { }) as Object;
  
                    return value;
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogError(EWisdomLogType.Privacy, $"Failed to call: `GoogleMobileAds.Editor.GoogleMobileAdsSettings.LoadInstance()`. Error: {e}");
                    
                    return null;
                }
            }
        }
    }
}