#if !SUPERSONIC_WISDOM_TEST
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwStageSymbolUpdater
    {

        [InitializeOnLoadMethod]
        public static void UpdateStageSymbols()
        {
            SwStage stage = JsonUtility.FromJson<SwStage>(SwStageUtils.StageMetadataContent);
            SwStageSymbolsUtils.ApplyStage(stage);
        }
    }
}
#endif