using System;
using System.IO;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwFilesCacheManager
    {
        #region --- Constants ---

        /// <summary>
        ///     Name of the directory inside the cache
        /// </summary>
        private const string CacheDirectoryName = "supersonic_wisdom";

        #endregion


        #region --- Members ---

        /// <summary>
        ///     Path of the cache
        /// </summary>
        public static readonly string Path = string.Format("{0}/{1}/", Application.temporaryCachePath, CacheDirectoryName);

        private readonly ISwLogger _logger;

        #endregion


        #region --- Construction ---

        public SwFilesCacheManager()
        {
            _logger = SwInfra.Logger;
        }

        #endregion


        #region --- Public Methods ---

        public void Clear ()
        {
            _logger.Log(EWisdomLogType.Cache, "Clear cached files");

            foreach (var file in GetAllFilesFromCache())
            {
                DeleteFile(file);
                _logger.Log(EWisdomLogType.Cache, "Removed cached file | " + file);
            }
        }

        /// <summary>
        ///     Delete the file inside of the Cache
        /// </summary>
        /// <param name="filePath">Absolute path of the file</param>
        public void DeleteFile(string filePath)
        {
            try
            {
                File.Delete(filePath);
                _logger.Log(EWisdomLogType.Cache, filePath);
            }
            catch (Exception e)
            {
                _logger.LogError(EWisdomLogType.Cache, "Deleting file failed " + e);
            }
        }

        /// <summary>
        ///     Check if the file exist inside of the cache
        /// </summary>
        /// <param name="filePath">Relative path of the file</param>
        /// <returns><c>true</c> if the file exist or <c>false</c> if not</returns>
        public bool DoesFileExist(string filePath)
        {
            try
            {
                return File.Exists(Path + filePath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Get all the files in the cache
        /// </summary>
        /// <param name="directory">Relative path of the directory to get the files from</param>
        /// <returns>All the absolute files path</returns>
        public string[] GetAllFilesFromCache(string directory = "")
        {
            try
            {
                return Directory.GetFiles($"{Path}{directory}", "*.*", SearchOption.AllDirectories);
            }
            catch (Exception)
            {
                return new string[0];
            }
        }

        /// <summary>
        ///     Get the current directory of file
        /// </summary>
        /// <param name="filePath">Absolute or relative path of the file</param>
        /// <returns>The current directory</returns>
        public string GetDirectoryOfFile(string filePath)
        {
            return System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(filePath));
        }

        /// <summary>
        ///     Create the cache directory if it is not present
        /// </summary>
        public void Setup ()
        {
            try
            {
                CreateDirectories(Path);
            }
            catch (Exception e)
            {
                _logger.LogError(EWisdomLogType.Cache, e.Message);
            }
        }

        /// <summary>
        ///     Write the content of a <c>byte[]</c> in a file inside the cache
        /// </summary>
        /// <param name="filePath">Relative path of the file</param>
        /// <param name="bytes">The content of the file in <c>bytes[]</c></param>
        public void WriteFile(string filePath, byte[] bytes)
        {
            try
            {
                var fileFullPath = Path + filePath;
                CreateDirectories(fileFullPath, true);
                File.WriteAllBytes(fileFullPath, bytes);
                _logger.Log(EWisdomLogType.Cache, fileFullPath);
            }
            catch (Exception e)
            {
                _logger.LogError(EWisdomLogType.Cache, e.Message);
            }
        }

        #endregion


        #region --- Private Methods ---

        /// <summary>
        ///     Create a directory if no exist
        /// </summary>
        /// <param name="path">Absoulte path of the directory</param>
        /// <param name="fromFilePath">
        ///     Create directories and subdirectory
        ///     if the <paramref name="path" /> is a file path
        /// </param>
        /// <exception cref="Exception">Invalid directory name</exception>
        private void CreateDirectories(string path, bool fromFilePath = false)
        {
            if (fromFilePath)
            {
                var directoryName = System.IO.Path.GetDirectoryName(path);

                if (string.IsNullOrEmpty(directoryName))
                    throw new Exception("Wrong Directory Name");

                path = directoryName;
            }

            if (Directory.Exists(path)) return;
            Directory.CreateDirectory(path);
            _logger.Log(EWisdomLogType.Cache, path);
        }

        #endregion
    }
}