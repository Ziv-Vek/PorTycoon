using System.Xml.Linq;

namespace SupersonicWisdomSDK.Editor
{
    public static class SwAndroidNetworkSecurityConfig
    {
        #region --- Constants ---

        private const string NetworkSecurityConfigTag = "networkSecurityConfig";
        private const string NetworkSecurityConfigValue = "@xml/sw_network_security_config";

        private const string NetworkSecurityConfigXmlContent = "<debug-overrides>\n" + "\t<trust-anchors>\n" + "\t\t<!-- Trust user added CAs while debuggable only -->\n" + "\t\t<certificates src=\"user\" />\n" + "\t</trust-anchors>\n" + "</debug-overrides>";

        #endregion


        #region --- Members ---

        private static readonly XName NetworkSecurityConfigAttrName = SwAndroidManifestUtils.AndroidNamespace + NetworkSecurityConfigTag;

        #endregion


        #region --- Private Methods ---

        internal static bool ApplyAndroidNetworkSecurityConfig ()
        {
            var manifest = SwAndroidManifestUtils.LoadManifest();

            if (manifest == null)
            {
                SwEditorLogger.LogWarning($"Could not load {SwAndroidManifestUtils.TargetManifestPath}.");

                return false;
            }

            var elemManifest = manifest.Element(SwAndroidManifestUtils.ManifestElement);

            if (elemManifest == null)
            {
                SwEditorLogger.LogWarning($"{SwAndroidManifestUtils.TargetManifestPath} is not valid, missing <manifest> element.");

                return false;
            }

            var elemApplication = elemManifest.Element(SwAndroidManifestUtils.ApplicationElement);

            if (elemApplication == null)
            {
                SwEditorLogger.LogWarning($"{SwAndroidManifestUtils.TargetManifestPath} is not valid, missing <application> element");

                return false;
            }

            var networkSecurityConfigAttr = elemApplication.Attribute(NetworkSecurityConfigAttrName);

            if (networkSecurityConfigAttr != null)
            {
                SwEditorLogger.LogWarning($"The <application/> element in {SwAndroidManifestUtils.TargetManifestPath} already has value for attribute android:{NetworkSecurityConfigTag}");
                SwEditorLogger.LogWarning($"Please copy the text between the lines:\n----- \n{NetworkSecurityConfigXmlContent}\n-----\n into {networkSecurityConfigAttr.Value}");

                return false;
            }


            elemApplication.SetAttributeValue(NetworkSecurityConfigAttrName, NetworkSecurityConfigValue);
            elemManifest.Save(SwAndroidManifestUtils.GetAndroidManifestPath());

            return true;
        }

        #endregion
    }
}