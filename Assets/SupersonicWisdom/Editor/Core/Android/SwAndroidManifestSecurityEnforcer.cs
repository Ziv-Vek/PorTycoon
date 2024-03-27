#if UNITY_ANDROID
using System.Xml;
using SupersonicWisdomSDK.Editor;
using UnityEditor.Android;

public class SwAndroidManifestSecurityEnforcer : IPostGenerateGradleAndroidProject
{
    #region --- Properties ---

    public int callbackOrder
    {
        get { return 1; }
    }

    #endregion
    

    #region --- Public Methods ---

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        var manifestPath = $"{path}/src/main/AndroidManifest.xml";
        SetManifestForProduction(manifestPath);
    }
    
    #endregion
    
    
    #region --- Private Methods ---
    
    private static void SetManifestForProduction(string manifestPath)
    {
        #if !DEVELOPMENT_BUILD
        var xmlDoc = new SwAndroidXmlDocument(manifestPath);
        var applicationNode = xmlDoc.DocumentElement?.SelectSingleNode("/manifest/application");

        if (applicationNode is XmlElement applicationElement)
        {
            applicationElement.SetAttribute("debuggable", SwAndroidXmlDocument.AndroidXmlNamespace, "false");
            applicationElement.SetAttribute("allowBackup", SwAndroidXmlDocument.AndroidXmlNamespace, "false");
            xmlDoc.Save();
        }

        #endif
    }
    
    #endregion
}

#endif