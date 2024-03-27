#if UNITY_IOS
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor.iOS.Xcode;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwSkAdNetworkUtil
    {
        [Serializable]
        class SwSkAdNetworksResponse
        {
            public string[] networks;
        }
    
        private const string SK_AD_NETWORKS_URL = "https://assets.mobilegamestats.com/wisdom/skadnetwork/networks.json";

        public static string[] FetchSkAdNetworks()
        {
            UnityWebRequest unityWebRequest =
                UnityWebRequest.Get(SK_AD_NETWORKS_URL);
            var webRequest = unityWebRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                Thread.Sleep(300);
            }

            if (unityWebRequest.result == UnityWebRequest.Result.ProtocolError || unityWebRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                throw new SwException("Cannot fetch SKAdNetwork JSON");
            }

            var response = JsonUtility.FromJson<SwSkAdNetworksResponse>(unityWebRequest.downloadHandler.text);
            if (response == null)
            {
                throw new SwException("Cannot parse SKAdNetwork JSON");
            }

            return response.networks ?? new string[]{};
        }

        public static void InjectSkAdNetworks(PlistDocument plist, string[] skAdNetworks )
        {
            PlistElementArray networks = plist.root.CreateArray(SwAttributionConstants.SkAdNetworkItemsKey);
            foreach (var network in skAdNetworks)
            {
                PlistElementDict networkDict = networks.AddDict();
                networkDict.SetString(SwAttributionConstants.SkAdNetworkIdentifierKey, network);
            }
        }
    }
}
#endif