namespace SupersonicWisdomSDK
{
    internal interface ISwGameProgressionListener
    {
        #region --- Public Methods ---

        void OnTimeBasedGameStarted();
        void OnLevelCompleted(ESwLevelType levelType, long level, string levelName, long attempts, long revives);
        void OnLevelFailed(ESwLevelType levelType, long level, string levelName, long attempts, long revives);
        void OnLevelRevived(ESwLevelType levelType, long level, string levelName, long attempts, long revives);
        void OnLevelSkipped(ESwLevelType levelType, long level, string levelName, long attempts, long revives);
        void OnLevelStarted(ESwLevelType levelType, long level, string levelName, long attempts, long revives);
        void OnMetaStarted(string customString);
        void OnMetaEnded(string customString);
        
        #endregion
    }
}