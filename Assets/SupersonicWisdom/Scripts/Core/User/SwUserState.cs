using System;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwUserState
    {
        #region --- Public Methods ---

        public virtual SwUserState Copy()
        {
            return new SwUserState(this);
        }

        #endregion


        #region --- Members ---

        public long age;
        public long completedLevels;
        public long consecutiveCompletedLevels;
        public long consecutiveFailedLevels;
        public long levelAttempts;
        public long levelRevives;
        public long playedLevels;
        public long todaySessionsCount;
        public long totalSessionsCount;
        public bool isDuringLevel;
        public long lastLevelStarted;
        public long lastBonusLevelStarted;
        public long lastTutorialLevelStarted;
        public long completedBonusLevels;
        public long completedTutorialLevels;
        public long previousLevelTypeNumber;
        public ESwLevelType previousLevelType;
        [CanBeNull] [NonSerialized] public string SessionId;

        #endregion


        #region --- Construction ---

        protected SwUserState(SwUserState other)
        {
            SessionId = other.SessionId;

            isDuringLevel = other.isDuringLevel;
            age = other.age;
            todaySessionsCount = other.todaySessionsCount;
            totalSessionsCount = other.totalSessionsCount;
            completedLevels = other.completedLevels;
            lastLevelStarted = other.lastLevelStarted;
            lastBonusLevelStarted = other.lastBonusLevelStarted;
            lastTutorialLevelStarted = other.lastTutorialLevelStarted;
            playedLevels = other.playedLevels;
            consecutiveFailedLevels = other.consecutiveFailedLevels;
            consecutiveCompletedLevels = other.consecutiveCompletedLevels;
            levelRevives = other.levelRevives;
            levelAttempts = other.levelAttempts;
            previousLevelType = other.previousLevelType;
            previousLevelTypeNumber = other.previousLevelTypeNumber;
            completedBonusLevels = other.completedBonusLevels;
            completedTutorialLevels = other.completedTutorialLevels;
        }

        internal SwUserState() { }

        #endregion
    }
}