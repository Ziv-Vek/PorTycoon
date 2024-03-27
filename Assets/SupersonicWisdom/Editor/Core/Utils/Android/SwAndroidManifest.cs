using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwAndroidManifest : SwAndroidXmlDocument
    {
        #region --- Members ---

        private readonly XmlElement _applicationElement;

        #endregion


        #region --- Construction ---

        public SwAndroidManifest(string path) : base(GetManifestPath(path))
        {
            _applicationElement = SelectSingleNode("/manifest/application") as XmlElement;
        }

        #endregion


        #region --- Private Methods ---

        private static string GetManifestPath(string basePath)
        {
            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");

            return pathBuilder.ToString();
        }

        internal XmlNode GetActivityWithLaunchIntent ()
        {
            return SelectSingleNode("/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and " + "intent-filter/category/@android:name='android.intent.category.LAUNCHER']", nsMgr);
        }

        internal void SetAndroidAttribute(XmlElement element, string name, string value)
        {
            element.SetAttribute(name, AndroidXmlNamespace, value);
        }

        internal void SetLaunchMode ()
        {
            var xmlAttributeCollection = GetActivityWithLaunchIntent().Attributes;

            if (xmlAttributeCollection != null)
            {
                xmlAttributeCollection.Append(CreateAndroidAttribute("launchMode", "singleTop"));
            }
            else
            {
                Debug.LogError("Couldn't find Android activity with launch intent.");
            }
        }

        internal void SetMetadataElement(string name, string value)
        {
            var metaData = CreateElement("meta-data");
            metaData.SetAttribute("name", AndroidXmlNamespace, name);
            metaData.SetAttribute("value", AndroidXmlNamespace, value);
            SetOrReplaceXmlElement(_applicationElement, metaData);
        }

        private XmlAttribute CreateAndroidAttribute(string key, string value)
        {
            var attr = CreateAttribute("android", key, AndroidXmlNamespace);
            attr.Value = value;

            return attr;
        }

        #endregion
    }
}