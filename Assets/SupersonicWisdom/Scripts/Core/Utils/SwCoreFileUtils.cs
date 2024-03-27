using System;
using System.IO;

namespace SupersonicWisdomSDK
{
    internal class SwCoreFileUtils
    {
        #region --- Public Methods ---
        
        #if UNITY_EDITOR
        public string WhereIs(string file)
        {
            string[] assets = { Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar };
            var myFile = new DirectoryInfo("Assets").GetFiles(file, SearchOption.AllDirectories);
            
            if (myFile.Length == 0) return string.Empty;
            
            var temp = myFile[0].ToString().Split(assets, 2, StringSplitOptions.None);

            return "Assets" + Path.DirectorySeparatorChar + temp[1];
        }
        #endif
        
        public string GenerateFileSizeString(ulong bytes)
        {
            if (bytes < 1024f)
            {
                return $"{bytes} bytes";
            }

            if ((double)bytes < 1024f * 1024f)
            {
                return $"{(double)bytes / 1024f:0.0}KB";
            }

            return $"{(double)bytes / 1024f / 1024f:0.0}MB";
        }

        #endregion
    }
}