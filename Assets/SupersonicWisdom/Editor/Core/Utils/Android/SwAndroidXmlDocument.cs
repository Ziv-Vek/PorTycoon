using System.Text;
using System.Xml;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwAndroidXmlDocument : XmlDocument
    {
        #region --- Constants ---

        public const string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

        #endregion


        #region --- Members ---

        protected XmlNamespaceManager nsMgr;
        private readonly string m_Path;

        #endregion


        #region --- Construction ---

        public SwAndroidXmlDocument(string path)
        {
            m_Path = path;

            using (var reader = new XmlTextReader(m_Path))
            {
                reader.Read();
                Load(reader);
            }

            nsMgr = new XmlNamespaceManager(NameTable);
            nsMgr.AddNamespace("android", AndroidXmlNamespace);
        }

        #endregion


        #region --- Public Methods ---

        public static void SetOrReplaceXmlElement(XmlNode parent, XmlElement newElement)
        {
            var attrNameValue = newElement.GetAttribute("name");
            var elementType = newElement.Name;

            XmlElement existingElment;

            if (TryFindElementWithAndroidName(parent, attrNameValue, out existingElment, elementType))
            {
                parent.ReplaceChild(newElement, existingElment);
            }
            else
            {
                parent.AppendChild(newElement);
            }
        }

        public static bool TryFindElementWithAndroidName(XmlNode parent, string attrNameValue, out XmlElement element, string elementType = "activity")
        {
            var ns = parent.GetNamespaceOfPrefix("android");
            var curr = parent.FirstChild;

            while (curr != null)
            {
                var currXmlElement = curr as XmlElement;

                if (currXmlElement != null && currXmlElement.Name == elementType && currXmlElement.GetAttribute("name", ns) == attrNameValue)
                {
                    element = currXmlElement;

                    return true;
                }

                curr = curr.NextSibling;
            }

            element = null;

            return false;
        }

        public string Save ()
        {
            return SaveAs(m_Path);
        }

        public string SaveAs(string path)
        {
            using (var writer = new XmlTextWriter(path, new UTF8Encoding(false)))
            {
                writer.Formatting = Formatting.Indented;
                Save(writer);
            }

            return path;
        }

        #endregion
    }
}