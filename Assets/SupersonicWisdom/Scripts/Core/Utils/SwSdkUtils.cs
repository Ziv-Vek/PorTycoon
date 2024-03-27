using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public static class SwSdkUtils
    {
        public static int[] GetAllStages()
        {
            const string path = SwConstants.SUPERSONIC_WISDOM_SCRIPTS_PATH;
            var regex = new Regex(@"Stage(\d+)");
            var stageNumbers = new List<int>();
            
            if (!Directory.Exists(path))
            {
                Debug.LogError($"The path '{path}' does not exist.");
                return stageNumbers.ToArray();
            }

            var directories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            foreach (var directory in directories)
            {
                var dirInfo = new DirectoryInfo(directory);
                var dirName = dirInfo.Name;
                
                var match = regex.Match(dirName);
                
                if (match.Success)
                {
                    if (int.TryParse(match.Groups[1].Value, out int stageNumber))
                    {
                        stageNumbers.Add(stageNumber);
                    }
                }
            }

            var stageArray = stageNumbers.ToArray();
            Array.Sort(stageArray);
            
            return stageArray;
        }
    }
}