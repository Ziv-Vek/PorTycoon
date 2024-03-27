using System;

namespace SupersonicWisdomSDK
{
    internal class SwGameStateSystemDataModule
    {
        #region --- Members ---

        private readonly SwCoreUserData _coreUserData;
        private ESwLevelType? _previousLevelType = null;
        private long? _previousLevelTypeNumber = null;
        
        #endregion
        
        
        #region --- Construction ---
        
        internal SwGameStateSystemDataModule(SwCoreUserData coreUserData)
        {
            _coreUserData = coreUserData;
        }
        
        #endregion
        
        
        #region --- Properties ---
        
        internal SwGameStateProgressionData? LatestGameStateProgressionData { get; private set; }

        #endregion
        
        
        #region --- Public Methods ---
        
        internal void SaveGameStateChangeInUserState(SwGameStateProgressionData gameStateProgressionData)
        {
            SwInfra.Logger?.Log(EWisdomLogType.GameStateSystem, gameStateProgressionData.ToString());
            LatestGameStateProgressionData = gameStateProgressionData;
            PopulateUserDataWithRelevantGameStateData(gameStateProgressionData);
        }
        
        internal void SetPreviousLevelData(ESwLevelType levelType, long level)
        {
            if (_previousLevelType == levelType && _previousLevelTypeNumber == level)
            {
                return;
            }
            
            _previousLevelType = levelType;
            _previousLevelTypeNumber = level;
            
            _coreUserData?.ModifyUserStateSync(modifier =>
            {
                modifier.previousLevelType = levelType;
                modifier.previousLevelTypeNumber = level;
            });
        }
        
        internal Tuple<ESwLevelType?, long?> GetPreviousLevelData()
        {
            return new Tuple<ESwLevelType?, long?>(_previousLevelType, _previousLevelTypeNumber);
        }

        #endregion
        
        
        #region --- Private Methods ---

        private void PopulateUserDataWithRelevantGameStateData(SwGameStateProgressionData gameStateProgressionData)
        {
            //Go over all the fields in the SwGameStateProgressionData struct and populate the relevant fields in the SwCoreUserData

            _coreUserData?.ModifyUserStateSync(modifier =>
            {
                if (gameStateProgressionData.ConsecutiveCompletedLevels.HasValue)
                {
                    modifier.consecutiveCompletedLevels = gameStateProgressionData.ConsecutiveCompletedLevels.Value;
                }
                
                if (gameStateProgressionData.ConsecutiveFailedLevels.HasValue)
                {
                    modifier.consecutiveFailedLevels = gameStateProgressionData.ConsecutiveFailedLevels.Value;
                }
                
                if (gameStateProgressionData.LevelAttempts.HasValue)
                {
                    modifier.levelAttempts = gameStateProgressionData.LevelAttempts.Value;
                }
                
                if (gameStateProgressionData.LevelRevives.HasValue)
                {
                    modifier.levelRevives = gameStateProgressionData.LevelRevives.Value;
                }
                
                if (gameStateProgressionData.PlayedLevels.HasValue)
                {
                    modifier.playedLevels = gameStateProgressionData.PlayedLevels.Value;
                }
                
                if (gameStateProgressionData.IsDuringLevel.HasValue)
                {
                    modifier.isDuringLevel = gameStateProgressionData.IsDuringLevel.Value;
                }
                
                if (gameStateProgressionData.LastLevelStarted.HasValue)
                {
                    modifier.lastLevelStarted = gameStateProgressionData.LastLevelStarted.Value;
                }
                
                if (gameStateProgressionData.LastBonusLevelStarted.HasValue)
                {
                    modifier.lastBonusLevelStarted = gameStateProgressionData.LastBonusLevelStarted.Value;
                }
                
                if (gameStateProgressionData.LastTutorialLevelStarted.HasValue)
                {
                    modifier.lastTutorialLevelStarted = gameStateProgressionData.LastTutorialLevelStarted.Value;
                }
                
                if (gameStateProgressionData.CompletedLevels.HasValue)
                {
                    modifier.completedLevels = gameStateProgressionData.CompletedLevels.Value;
                }
                
                if (gameStateProgressionData.CompletedBonusLevels.HasValue)
                {
                    modifier.completedBonusLevels = gameStateProgressionData.CompletedBonusLevels.Value;
                }
                
                if (gameStateProgressionData.CompletedTutorialLevels.HasValue)
                {
                    modifier.completedTutorialLevels = gameStateProgressionData.CompletedTutorialLevels.Value;
                }
                
                if (gameStateProgressionData.PreviousLevelTypeNumber.HasValue)
                {
                    modifier.previousLevelTypeNumber = gameStateProgressionData.PreviousLevelTypeNumber.Value;
                }
                
                if (gameStateProgressionData.PreviousLevelType.HasValue)
                {
                    modifier.previousLevelType = gameStateProgressionData.PreviousLevelType.Value;
                }
            });
        }
        
        #endregion
    }
}
