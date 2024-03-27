#if SW_STAGE_STAGE10_OR_ABOVE

using System.Collections;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    public class SwStage10TestUserState : ISwDeepLinkListener
    {
        #region --- Constants ---

        private const string TESTER_KEY = "tester";

        #endregion


        #region --- Members ---

        private bool _isTester;

        #endregion


        #region --- Properties ---
        
        internal bool IsTester
        {
            get => _isTester;
            private set
            {
                SwInfra.KeyValueStore.SetBoolean(TESTER_KEY, value);
                _isTester = value;
            }
        }

        internal bool IsActivatedByDeeplink { get; private set; }

        #endregion


        #region --- Construction ---

        public SwStage10TestUserState()
        {
            IsTester = LoadStateFromData();
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator OnDeepLinkResolved(Dictionary<string, string> deepLinkParams)
        {
            IsActivatedByDeeplink = true;
            IsTester = true;
            yield break;
        }

        #endregion


        #region --- Private Methods ---

        private bool LoadStateFromData()
        {
            return SwInfra.KeyValueStore.GetBoolean(TESTER_KEY);
        }

        #endregion
    }
}

#endif