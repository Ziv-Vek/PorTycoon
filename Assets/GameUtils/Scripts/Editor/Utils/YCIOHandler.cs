using System.IO;
using UnityEngine;

namespace YsoCorp {
    namespace GameUtils {

        public class YCIOHandler {

            public static bool IsDirectory(string path) {
                if (File.Exists(path) || Directory.Exists(path)) {
                    FileAttributes attr = File.GetAttributes(path);
                    return attr.HasFlag(FileAttributes.Directory);
                } else {
                    return false;
                }
            }

            public static void DeleteDirectory(string path, bool recursive, bool includeMeta = true) {
                if (Directory.Exists(path)) {
                    Directory.Delete(path, recursive);
                    if (includeMeta) {
                        DeleteFile(path + ".meta", false);
                    }
                } else {
                    Debug.Log("[DeleteDirectory] Directory not found at " + path);
                }
            }

            public static void DeleteFile(string path, bool includeMeta = true) {
                if (File.Exists(path)) {
                    File.Delete(path);
                    if (includeMeta) {
                        DeleteFile(path + ".meta", false);
                    }
                } else {
                    Debug.Log("[DeleteFile] File not found at " + path);
                }
            }

        }
    }
}
