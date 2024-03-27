using UnityEngine;
#if UNITY_EDITOR
using System.IO;
#endif

namespace SupersonicWisdomSDK
{
    public static class SwStageUtils
    {
        public static int CurrentStageNumber
        {
            get
            {
                CheckStage();
                return _currentStageNumber;
            }
        }

        public const int MAX_STAGE_NUMBER = 1000;

        private static SwStage _currentStage;
        private static int _currentStageNumber = -1;

        public static string CurrentStageName
        {
            get { return "Stage" + CurrentStageNumber; }
        }

        public static SwStage CurrentStage
        {
            get { return _currentStage ??= LoadStage(); }
        }

        private static SwStage LoadStage()
        {
            return JsonUtility.FromJson<SwStage>(StageMetadataContent);
        }

        private static void CheckStage()
        {
            _currentStageNumber =
#if SUPERSONIC_WISDOM_TEST
                SwTestUtils.ForceStage > -1 ? SwTestUtils.ForceStage :
#endif
#if SW_STAGE_STAGE10
                    10;
#elif SW_STAGE_STAGE20
                    20;
#elif SW_STAGE_STAGE30
                    30;
#elif SW_STAGE_STAGE40
                    40;
#else
                    -1;
#endif
        }

        public static string StageMetadataContent =>
#if SUPERSONIC_WISDOM_TEST && UNITY_EDITOR
            File.ReadAllText(Path.Combine(Application.dataPath, "Editor", "Stages", CurrentStageName, "Resources", "Core", "StageMetadata.StageResource.json.template"));
#elif UNITY_EDITOR
            File.ReadAllText(Path.Combine(
            Application.dataPath,
            "SupersonicWisdom",
            "Resources",
            "Core",
            "StageMetadata.StageResource.json"));
#else
            Resources.Load<TextAsset>("Core/StageMetadata.StageResource").text;
#endif

#if SUPERSONIC_WISDOM_TEST
        internal static void Reset()
        {
            _currentStage = null;
        }
#endif
    }
}