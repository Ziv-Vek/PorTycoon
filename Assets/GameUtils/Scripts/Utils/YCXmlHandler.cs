using System;
using System.IO;
using UnityEngine;
using System.Xml;

namespace YsoCorp {
    namespace GameUtils {
        public class YCXmlHandler {


            public void InitAmazon() {
                string manifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
                XmlDocument manifestFile = new XmlDocument();
                manifestFile.Load(manifestPath);
                XmlElement manifestRoot = manifestFile.DocumentElement;
                XmlNode applicationRoot = manifestRoot.SelectSingleNode("descendant::application");

                bool modified = false;
                bool hasCoarseLoc = false;
                bool hasFineLoc = false;
                bool hasDTBInterstitialActivity = false;
                bool hasDTBADActivity = false;

                foreach (XmlNode node in manifestRoot.ChildNodes) {
                    if (node.Name == "uses-permission") {
                        foreach (XmlAttribute attribute in node.Attributes) {
                            if (attribute.Value.Contains("android.permission.ACCESS_COARSE_LOCATION")) {
                                hasCoarseLoc = true;
                            } else if (attribute.Value.Contains("android.permission.ACCESS_FINE_LOCATION")) {
                                hasFineLoc = true;
                            }
                        }
                    }
                }
                foreach (XmlNode node in applicationRoot.ChildNodes) {
                    if (node.Name == "activity") {
                        foreach (XmlAttribute attribute in node.Attributes) {
                            if (attribute.Value.Contains("com.amazon.device.ads.DTBInterstitialActivity")) {
                                hasDTBInterstitialActivity = true;
                            } else if (attribute.Value.Contains("com.amazon.device.ads.DTBAdActivity")) {
                                hasDTBADActivity = true;
                            }
                        }
                    }
                }

                XmlNodeList refElementsUsesPermission = manifestRoot.SelectNodes("descendant::uses-permission");
                XmlNodeList refElementsActivity = applicationRoot.SelectNodes("descendant::activity");

                if (hasCoarseLoc == false) {
                    this.AddAndroidName(manifestFile, manifestRoot, refElementsUsesPermission, "uses-permission", "android.permission.ACCESS_COARSE_LOCATION");
                    modified = true;
                }
                if (hasFineLoc == false) {
                    this.AddAndroidName(manifestFile, manifestRoot, refElementsUsesPermission, "uses-permission", "android.permission.ACCESS_FINE_LOCATION");
                    modified = true;
                }
                if (hasDTBInterstitialActivity == false) {
                    this.AddAndroidName(manifestFile, applicationRoot, refElementsActivity, "activity", "com.amazon.device.ads.DTBInterstitialActivity");
                    modified = true;
                }
                if (hasDTBADActivity == false) {
                    this.AddAndroidName(manifestFile, applicationRoot, refElementsActivity, "activity", "com.amazon.device.ads.DTBAdActivity");
                    modified = true;
                }

                if (modified) {
                    manifestFile.Save(manifestPath);
                }
            }

            private void AddAndroidName(XmlDocument manifestFile, XmlNode root, XmlNodeList refElements, string elementName, string value) {
                XmlElement element = manifestFile.CreateElement(elementName);
                element.SetAttribute("name", "http://schemas.android.com/apk/res/android", value);
                if (refElements.Count > 0) {
                    root.InsertAfter(element, refElements[refElements.Count - 1]);
                } else {
                    root.AppendChild(element);
                }
            }

            // --------------------------------------------------

            public static void UpdateDocument(string filePath, Action<XmlDocument, XmlElement> updates) {
                string path = Path.Combine(Application.dataPath, filePath);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                XmlElement documentRoot = xmlDocument.DocumentElement;

                updates?.Invoke(xmlDocument, documentRoot);
                xmlDocument.Save(path);
            }

            public static void CreateElement(XmlDocument xmlDocument, XmlElement documentRoot, YCXmlElement ycXmlElement) {
                if (GetExistingNode(documentRoot, ycXmlElement) == null) {
                    XmlNode elementRoot = GetElementRoot(documentRoot, ycXmlElement);
                    XmlNodeList refElements = elementRoot.SelectNodes("descendant::" + ycXmlElement.elementName);

                    XmlElement element = xmlDocument.CreateElement(ycXmlElement.elementName);
                    SetAttributes(element, ycXmlElement.attributes);

                    if (refElements.Count > 0) {
                        elementRoot.InsertAfter(element, refElements[refElements.Count - 1]);
                    } else {
                        elementRoot.AppendChild(element);
                    }
                }
            }

            public static void UpdateElement(XmlDocument xmlDocument, XmlElement documentRoot, YCXmlElement ycXmlElement, YCXmlAttribute[] newAttributes, bool createIfNotFound = false) {
                XmlElement element = (XmlElement)GetExistingNode(documentRoot, ycXmlElement);

                if (element != null) {
                    SetAttributes(element, newAttributes);
                } else if (createIfNotFound) {
                    ycXmlElement.attributes = newAttributes;
                    CreateElement(xmlDocument, documentRoot, ycXmlElement);

                }
            }

            public static void SetAttributes(XmlElement element, YCXmlAttribute[] attributes) {
                foreach (YCXmlAttribute attribute in attributes) {
                    SetAttribute(element, attribute);
                }
            }

            public static void SetAttribute(XmlElement element, YCXmlAttribute attribute) {
                if (attribute.ycAttributeType == YCAttributeType.Android) {
                    element.SetAttribute(attribute.name, "http://schemas.android.com/apk/res/android", attribute.value);
                } else {
                    element.SetAttribute(attribute.name, attribute.value);
                }
            }

            public static XmlNode GetElementRoot(XmlElement documentRoot, YCXmlElement ycXmlElement) {
                if (ycXmlElement.rootPath == "") {
                    return documentRoot;
                } else {
                    if (ycXmlElement.rootPath[0] != '/') {
                        Debug.LogError("Root Path must start with a\'/\'");
                        return null;
                    }
                    return documentRoot.SelectSingleNode(ycXmlElement.rootPath);
                }
            }

            public static XmlNode GetExistingNode(XmlElement documentRoot, YCXmlElement ycXmlElement) {
                XmlNode elementRoot = GetElementRoot(documentRoot, ycXmlElement);

                string xPath = ycXmlElement.elementName + "[";
                for (int i = 0; i < ycXmlElement.attributes.Length; i++) {
                    if (i > 0) {
                        xPath += " and ";
                    }
                    xPath += "@*[local-name()=\'" + ycXmlElement.attributes[i].name + "\']=\'" + ycXmlElement.attributes[i].value + "\'";
                }
                xPath += "]";

                return elementRoot.SelectSingleNode(xPath);
            }

            public struct YCXmlElement {
                public string rootPath;
                public string elementName;
                public YCXmlAttribute[] attributes;

                public YCXmlElement(string _rootPath, string _elementName, YCXmlAttribute[] _attributes) {
                    this.rootPath = _rootPath;
                    this.elementName = _elementName;
                    this.attributes = _attributes;
                }

                public YCXmlElement(string _rootPath, string _elementName, YCXmlAttribute _attribute) {
                    this.rootPath = _rootPath;
                    this.elementName = _elementName;
                    this.attributes = new YCXmlAttribute[] { _attribute };
                }
            }

            public struct YCXmlAttribute {
                public string name;
                public YCAttributeType ycAttributeType;
                public string value;

                public YCXmlAttribute(string _name, YCAttributeType _ycAttributeType, string _value) {
                    this.name = _name;
                    this.ycAttributeType = _ycAttributeType;
                    this.value = _value;
                }
            }

            public enum YCAttributeType {
                None,
                Android
            }
        }
    }
}