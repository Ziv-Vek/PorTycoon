using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

namespace YsoCorp {
    namespace GameUtils {

        public class YCPackageManager {

            public static IEnumerator DownloadPackage(string url, string fileName, Action<bool, string> onDownload = null) {
                if (fileName.EndsWith(".unitypackage") == false) {
                    fileName += ".unitypackage";
                }
                string path = Path.Combine(Application.temporaryCachePath, fileName);
                if (File.Exists(path) == false) {
                    var downloadHandler = new DownloadHandlerFile(path);

                    UnityWebRequest webRequest = new UnityWebRequest(url) {
                        method = UnityWebRequest.kHttpVerbGET,
                        downloadHandler = downloadHandler
                    };

                    var operation = webRequest.SendWebRequest();
                    Debug.Log("Downloading " + fileName);
                    while (!operation.isDone) {
                        yield return new WaitForSeconds(0.1f);
                    }

#if UNITY_2020_1_OR_NEWER
                    if (webRequest.result != UnityWebRequest.Result.Success)
#else
                    if (webRequest.isNetworkError || webRequest.isHttpError)
#endif
                    {
                        Debug.LogError("The file " + fileName + " could not be downloaded.");
                        onDownload?.Invoke(false, path);
                        yield break;
                    }
                }
                onDownload?.Invoke(true, path);
            }

            public static void DownloadAndImportPackage(string url, string fileName, bool interactive, Action<bool, string> onDownload = null) {
                onDownload = ((downloaded, path) =>  AssetDatabase.ImportPackage(path, interactive)) + onDownload ;
                YCEditorCoroutine.StartCoroutine(DownloadPackage(url, fileName, onDownload));
                    
            }

            public static IEnumerator InstallPackage(string packageName, string version = "", Action onFinished = null) {
                var pack = Client.List();
                while (!pack.IsCompleted) yield return null;

                bool isInstalled = pack.Result.FirstOrDefault(q => q.name == packageName) != null;
                UnityEditor.PackageManager.Requests.AddRequest packAdd = null;
                if (isInstalled == false) {
                    if (string.IsNullOrEmpty(version) == false) {
                        packageName += "@" + version;
                    }
                    packAdd = Client.Add(packageName);
                }

                while (packAdd != null && !packAdd.IsCompleted) yield return null;
                onFinished?.Invoke();
            }
        }
    }
}