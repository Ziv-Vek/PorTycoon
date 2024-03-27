using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    public static class SwFileUtils
    {
        #region --- Constants ---

        public const string UNITY_IGNORED_FILE_SUFFIX = "~";
        public const string GET_ALL_REGEX_SIGN = "*";
        public const string HIDDEN_FILE_PREFIX = ".";
        public const string META_FILE_EXTENSION = ".meta";
        public const string UNITY_PACKAGE_FILE_EXTENSION = ".unitypackage";

        #endregion


        #region --- Public Methods ---

        public static bool DeleteDirectory(string path)
        {
            bool didDelete = false;

            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }

                didDelete = true;
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError($"Failed to delete folder at path: '{path}' Error: {e}");
            }

            return didDelete;
        }

        public static bool CopyFile(string sourcePath, string destinationPath, bool shouldOverwrite = false, bool ignoreMissing = false)
        {
            var didCopy = false;

            try
            {
                File.Copy(sourcePath, destinationPath, shouldOverwrite);
                didCopy = true;
            }
            catch (FileNotFoundException)
            {
                // Gracefully ignore
                didCopy = ignoreMissing;
            }
            catch (Exception e)
            {
                // Possible exceptions can be found here: https://learn.microsoft.com/en-us/dotnet/api/system.io.file.copy
                SwEditorLogger.LogWarning("Failed to copy '" + sourcePath + "' to '" + destinationPath + "'\nError:" + e);
            }

            return didCopy;
        }

        public static bool CreateDirectory(string path)
        {
            bool didCreate = false;

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                didCreate = true;
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError(e);
            }

            return didCreate;
        }

        public static bool TryMoveOrRenameFile(string sourcePath, string destinationPath)
        {
            var didRename = false;

            try
            {
                if (!File.Exists(destinationPath) && File.Exists(sourcePath))
                {
                    File.Move(sourcePath, destinationPath);
                    didRename = true;
                }
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError(e);
            }

            return didRename;
        }

        public static bool TryMoveOrRenameFolder(string sourcePath, string destinationPath)
        {
            var didRename = false;

            try
            {
                if (!Directory.Exists(destinationPath) && Directory.Exists(sourcePath))
                {
                    Directory.Move(sourcePath, destinationPath);
                    didRename = true;
                }
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError(e);
            }

            return didRename;
        }

        public static string[] GetFolderContent(string folderPath, bool recursive = false, params string[] excludedFileExtensions)
        {
            var allFiles = new List<string>();
            excludedFileExtensions ??= new string[] { };

            try
            {
                if (!Directory.Exists(folderPath)) return allFiles.ToArray();

                allFiles = Directory.GetFiles(folderPath).ToList();

                allFiles = allFiles.Where((item, _) =>
                {
                    var included = true;

                    foreach (var excludedExtension in excludedFileExtensions)
                    {
                        var extensionToExclude = (excludedExtension.StartsWith(".") ? "" : ".") + excludedExtension;
                        included &= !item.EndsWith(extensionToExclude);
                    }

                    return included;
                }).ToList();

                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    // Mandatory in Windows - convert all absolute paths to relative paths
                    var applicationDataPath = Application.dataPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                    allFiles = allFiles.ConvertAll(item => item.Replace(applicationDataPath, SwEditorConstants.ASSETS)).ToList();
                }

                if (!recursive) return allFiles.ToArray();

                var subFolders = new DirectoryInfo(folderPath).GetDirectories();

                foreach (var subDirectoryInfo in subFolders)
                {
                    var subFiles = GetFolderContent(subDirectoryInfo.FullName, true, excludedFileExtensions);
                    allFiles.AddRange(subFiles);
                }
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError($"{nameof(SwFileUtils)} | {nameof(GetFolderContent)} | Error: {e}");
            }

            return allFiles.ToArray();
        }
        
        internal static string ReadJsonAttributeFromFile(string fileSearchPattern, params string[] jsonSearchPattern)
        {
            var jsonObject = ReadJsonFile(fileSearchPattern);

            return (string)jsonObject?.SelectToken(string.Join(".", jsonSearchPattern));
        }

        internal static bool DeleteFileAtPath(string path, bool shouldAlsoDeleteMeta = true, bool shouldAlsoDeleteEmptyContainingFolder = true)
        {
            return DeleteFilesAtPaths(new[] { path }, shouldAlsoDeleteMeta, shouldAlsoDeleteEmptyContainingFolder);
        }
        
        internal static bool DeleteFilesAtPaths(IEnumerable<string> paths, bool shouldAlsoDeleteMeta = true, bool shouldAlsoDeleteEmptyContainingFolder = true)
        {
            var didDeleteAll = true;

            foreach (var fileOrFolderPath in paths)
            {
                var path = (Path.IsPathRooted(fileOrFolderPath) || Application.platform != RuntimePlatform.WindowsEditor)
                    ? fileOrFolderPath
                    : $"./{fileOrFolderPath}";
                
                if (string.IsNullOrWhiteSpace(path)) continue;

                try
                {
                    var deleteCandidateFileInfo = new FileInfo(path);

                    didDeleteAll &= SwTryDelete(deleteCandidateFileInfo) && (!shouldAlsoDeleteMeta || SwTryDelete(new FileInfo(path + META_FILE_EXTENSION)));

                    if (shouldAlsoDeleteEmptyContainingFolder)
                    {
                        DeleteEmptyFoldersRecursively(deleteCandidateFileInfo.Directory);
                    }
                    
                }
                catch (Exception e)
                {
                    SwEditorLogger.LogError($"Failed to delete file at path: {path}. Error: {e}");
                    
                    didDeleteAll = false;
                }
            }
            
            AssetDatabase.Refresh();

            return didDeleteAll;
        }
        
        internal static double FileSizeInMb(string filePath)
        {
            if (!File.Exists(filePath)) return -1;

            var resultZipFileInfo = new FileInfo(filePath);

            return (double)resultZipFileInfo.Length / 1024 / 1024;
        }

        internal static bool SwTryDelete(this FileInfo self)
        {
            if (!self.Exists) return true;

            var deleted = false;

            try
            {
                self.Delete();
                deleted = true;
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError($"Failed to delete file at path: {self.FullName}. Error: {e}");
            }

            return deleted;
        }

        
        #endregion


        #region --- Private Methods ---

        private static void DeleteEmptyFoldersRecursively(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null || !directoryInfo.Exists || directoryInfo.EnumerateFileSystemInfos().Any()) return;

            try
            {
                directoryInfo.Delete();
                SwTryDelete(new FileInfo(directoryInfo.FullName + META_FILE_EXTENSION));
                DeleteEmptyFoldersRecursively(directoryInfo.Parent);
            }
            catch (DirectoryNotFoundException) { /* THIS EXCEPTION IS EXPECTED, IGNORE IT */ }
            catch (Exception e)
            {
                SwEditorLogger.LogError($"Failed to delete empty folder at path: {directoryInfo.FullName}. Error: {e}");
            }
        }
        
        private static string ReadFile(string fileSearchPattern)
        {
            var dir = new DirectoryInfo(Application.dataPath);
            var files = dir.GetFiles(fileSearchPattern);

            if (files.Length == 0) return string.Empty;

            var source = new StreamReader(files.First().FullName);
            var fileContents = source.ReadToEnd();
            source.Close();

            return fileContents;
        }

        private static JObject ReadJsonFile(string fileSearchPattern)
        {
            var fileContent = ReadFile(fileSearchPattern);
            var googleServices = JObject.Parse(fileContent);

            return googleServices;
        }

        #endregion


        public static char DirectorySeparatorChar
        {
            get { return Application.platform == RuntimePlatform.WindowsEditor ? Path.AltDirectorySeparatorChar : Path.DirectorySeparatorChar; }
        }

    }
}
