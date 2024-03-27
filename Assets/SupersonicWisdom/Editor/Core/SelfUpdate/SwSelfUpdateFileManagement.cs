using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwSelfUpdateFileManagement
    {
        #region --- Enum ---
        
        internal enum FileOrFolderPath
        {
            File,
            Folder,
        }
        
        #endregion
        
        
        #region --- Properties ---

        internal static string WisdomBackUpFolderPath
        {
            get { return (Application.temporaryCachePath + "/sw-previous-version-files").Replace(" ", "-"); }
        }

        internal static string PendingUnityPackagesToImportFolderPath
        {
            get { return (Application.temporaryCachePath + "/sw-pending-unity-packages-to-import").Replace(" ", "-"); }
        }

        #endregion


        #region --- Public Methods ---

        internal static string GenerateLocalFilePath(int stageNumber, string versionToDownload)
        {
            if (!Directory.Exists(PendingUnityPackagesToImportFolderPath))
            {
                Directory.CreateDirectory(PendingUnityPackagesToImportFolderPath);
            }

            return PendingUnityPackagesToImportFolderPath + "/SupersonicWisdomSDK_" + versionToDownload + "_stage_" + stageNumber + SwFileUtils.UNITY_PACKAGE_FILE_EXTENSION;
        }

        internal static HashSet<string> GetCurrentPackageManifestContent()
        {
            var wisdomFilesManifestFilePath = GetCurrentWisdomPackageManifestFilePath();

            if (string.IsNullOrEmpty(wisdomFilesManifestFilePath)) return new HashSet<string>();

            SwEditorLogger.Log($"Found package content file: {wisdomFilesManifestFilePath.SwToString()}");
            var currentFilesList = File.ReadAllText(wisdomFilesManifestFilePath).SwSplit("\n").SwToHashSet();
            currentFilesList.Add(wisdomFilesManifestFilePath.Replace($".{SwFileUtils.DirectorySeparatorChar}{SwEditorConstants.ASSETS}", SwEditorConstants.ASSETS));

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // Fix path separator in Windows Editor
                currentFilesList = currentFilesList.ToList().ConvertAll(item => item.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)).SwToHashSet();
            }

            return currentFilesList;
        }

        internal static bool RestorePackageFromFolder(string backUpFolderPath)
        {
            if (string.IsNullOrEmpty(backUpFolderPath)) return false;
            if (!Directory.Exists(backUpFolderPath)) return false;

            SwEditorLogger.Log("Deleting current version files...");
            SwEditorTracker.TrackEditorEvent(nameof(RestorePackageFromFolder), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, "Deleting current version files...");

            if (DeleteCurrentVersionFiles())
            {
                SwEditorLogger.Log("... deleted.");
                SwEditorTracker.TrackEditorEvent(nameof(RestorePackageFromFolder), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, "Deleted current version files.");
            }
            else
            {
                SwEditorLogger.LogError("... failed to delete all current files!");
                SwEditorTracker.TrackEditorEvent(nameof(RestorePackageFromFolder), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, "Failed to delete current version files.");
            }

            var allBackUpFiles = SwFileUtils.GetFolderContent(backUpFolderPath, true);
            var didRestoreAll = true;

            foreach (var backUpFile in allBackUpFiles)
            {
                var relativePath = backUpFile.Replace(backUpFolderPath, string.Empty);

                string destinationPath;
                var assetsFolder = $"/{SwEditorConstants.ASSETS}/";

                if (relativePath.StartsWith(assetsFolder))
                {
                    relativePath = relativePath.Remove(0, assetsFolder.Length);
                    destinationPath = Path.Combine(Application.dataPath, relativePath);
                }
                else
                {
                    destinationPath = Path.Combine(Application.dataPath, relativePath);
                }

                var didCreate = SwFileUtils.CreateDirectory(Path.GetDirectoryName(destinationPath));

                if (!didCreate)
                {
                    didRestoreAll = false;

                    continue;
                }

                if (backUpFile.EndsWith(SwFileUtils.UNITY_IGNORED_FILE_SUFFIX) || destinationPath.EndsWith(SwFileUtils.UNITY_IGNORED_FILE_SUFFIX)) continue;

                didRestoreAll &= SwFileUtils.CopyFile(backUpFile, destinationPath, true, true);
            }

            return didRestoreAll;
        }

        internal static string BackupCurrentVersionFiles()
        {
            var packageManifestFileNames = GetCurrentPackageManifestContent();

            if (packageManifestFileNames.Count == 0)
            {
                SwEditorLogger.LogError("Failed to backup current package contents, no files found!");
                SwEditorTracker.TrackEditorEvent(nameof(BackupCurrentVersionFiles), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, "Failed to backup current package contents, no files found!");

                return string.Empty;
            }

            var backUpFolderPath = Path.Combine(WisdomBackUpFolderPath, SwConstants.SDK_VERSION);

            if (!SwFileUtils.DeleteDirectory(backUpFolderPath))
            {
                SwEditorLogger.LogError("Failed to delete previous backup folder: " + backUpFolderPath);
                SwEditorTracker.TrackEditorEvent(nameof(BackupCurrentVersionFiles), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, "Failed to delete previous backup folder: " + backUpFolderPath);

                return string.Empty;
            }

            var didCreate = SwFileUtils.CreateDirectory(backUpFolderPath);

            if (!didCreate)
            {
                SwEditorLogger.LogError("Failed to create backup folder: " + backUpFolderPath);
                SwEditorTracker.TrackEditorEvent(nameof(BackupCurrentVersionFiles), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, "Failed to create backup folder: " + backUpFolderPath);

                return string.Empty;
            }

            SwEditorLogger.Log($"Backing up current package contents to: {backUpFolderPath}");
            SwEditorTracker.TrackEditorEvent(nameof(BackupCurrentVersionFiles), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, $"Backing up current package contents to: {backUpFolderPath}");
            var allCopied = true;

            // Copy all current package content
            foreach (var filePath in packageManifestFileNames)
            {
                if (string.IsNullOrEmpty(filePath)) continue;
                if (filePath.StartsWith(SwFileUtils.HIDDEN_FILE_PREFIX) || filePath.EndsWith(SwFileUtils.UNITY_IGNORED_FILE_SUFFIX)) continue;

                var fileInfo = new FileInfo(filePath);

                if (!fileInfo.Exists) continue;

                var destinationPath = Path.Combine(backUpFolderPath, filePath.Replace(Application.dataPath, string.Empty));

                try
                {
                    var destinationFolder = Path.GetDirectoryName(destinationPath);
                    didCreate = SwFileUtils.CreateDirectory(destinationFolder);
                }
                catch (Exception e)
                {
                    didCreate = false;
                    SwEditorLogger.LogError(e);
                }

                if (!didCreate)
                {
                    SwEditorLogger.LogError("Failed to create backup destination folder: " + destinationPath);
                    SwEditorTracker.TrackEditorEvent(nameof(BackupCurrentVersionFiles), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, "Failed to create backup destination folder: " + destinationPath);

                    return string.Empty;
                }

                // Clean all previous files list
                if (SwFileUtils.CopyFile(filePath, destinationPath, true))
                {
                    continue;
                }

                allCopied = false;

                break;
            }

            return allCopied ? backUpFolderPath : string.Empty;
        }

        internal static void DeletePendingUnityPackagesToImportFolder()
        {
            DeleteFolder(PendingUnityPackagesToImportFolderPath);
        }

        internal static void DeleteWisdomBackUpFolder()
        {
            DeleteFolder(WisdomBackUpFolderPath);
        }

        internal static bool DeleteCurrentVersionFiles()
        {
            // Delete all files from "previous files list".
            var deleteComplete = SwFileUtils.DeleteFilesAtPaths(GetCurrentPackageManifestContent().ToArray());

            if (!deleteComplete)
            {
                SwEditorLogger.LogError("Failed to delete all current files!");
            }
            
            AssetDatabase.Refresh();

            return deleteComplete;
        }
        
        /// <summary>
        /// Ensures the path is valid. Returns a file/folder from fallback directory, if the path is empty.
        /// Logs an error and returns an empty string if the fallback directory does not exist.
        /// </summary>
        internal static string EnsurePath(string path, string fallbackPath, FileOrFolderPath pathType)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }

            if (Directory.Exists(fallbackPath))
            {
                switch (pathType)
                {
                    case FileOrFolderPath.File:
                        return Directory.GetFiles(fallbackPath).FirstOrDefault();
                    case FileOrFolderPath.Folder:
                        return Directory.GetDirectories(fallbackPath).FirstOrDefault();
                    default:
                        SwEditorLogger.LogError($"Unsupported pathType value: {pathType}");
                        return string.Empty;
                }
            }

            SwEditorLogger.LogWarning($"Fallback path does not exist: {fallbackPath}");

            return string.Empty;
        }

        /// <summary>
        /// Tries to find the path of a .unitypackage file.
        /// </summary>
        /// <param name="unityFilePath">Reference to the string where the found unity file path will be stored.</param>
        internal static void TryFindingUnityPathFile(out string unityFilePath)
        {
            var dir = new DirectoryInfo(PendingUnityPackagesToImportFolderPath);
            var unityFile = dir.GetFiles(SwFileUtils.GET_ALL_REGEX_SIGN + SwFileUtils.UNITY_PACKAGE_FILE_EXTENSION, SearchOption.AllDirectories).FirstOrDefault();

            unityFilePath = unityFile?.FullName;
        }
        
        /// <summary>
        /// Tries to locate the backup folder path.
        /// </summary>
        /// <param name="backupFolderPath">Reference to the string where the found backup folder path will be stored.</param>
        /// <returns>Returns true if a backup folder was found; otherwise, returns false.</returns>
        internal static bool TryFindingBackupFolder(out string backupFolderPath)
        {
            var directories = Directory.GetDirectories(WisdomBackUpFolderPath);

            if (!directories.Any())
            {
                backupFolderPath = null;
                return false;
            }
    
            backupFolderPath = directories.First();
            return true;
        }
        
        #endregion


        #region --- Private Methods ---

        private static void DeleteFolder(string path)
        {
            SwFileUtils.DeleteDirectory(path);
        }

        private static string GetCurrentWisdomPackageManifestFilePath()
        {
            var packageManifestFolderPath = SwEditorUtils.SwUpdateMetadataFolderPath;
            var wisdomPackageManifestFilePath = Path.Combine(packageManifestFolderPath, SwEditorUtils.GetWisdomPackageManifestFileName());

            if (File.Exists(wisdomPackageManifestFilePath))
            {
                return wisdomPackageManifestFilePath;
            }

            SwEditorLogger.LogError("File not found! Expected to find package manifest file in: " + packageManifestFolderPath);
            SwEditorTracker.TrackEditorEvent(nameof(GetCurrentWisdomPackageManifestFilePath), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, "File not found! Expected to find package manifest file");
            wisdomPackageManifestFilePath = string.Empty;

            return wisdomPackageManifestFilePath;
        }

        #endregion
    }
}