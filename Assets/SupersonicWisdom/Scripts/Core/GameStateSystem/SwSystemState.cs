namespace SupersonicWisdomSDK
{
    internal static class SwSystemState
    {
        public enum EGameState
        {
            Loading,
            //Start Playtime settings
            /// Time-based game
            Time,
            /// Level-based game
            Regular,
            Bonus,
            Tutorial,
            Meta,
            //End Playtime settings
            BetweenLevels,
        }

        public enum EStateEvent
        {
            Completed,
            Failed,
            Skipped,
        } 
    }
}