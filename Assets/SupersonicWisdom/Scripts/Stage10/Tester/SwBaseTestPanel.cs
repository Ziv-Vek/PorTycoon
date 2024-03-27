#if SW_STAGE_STAGE10_OR_ABOVE
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public abstract class SwBaseTestPanel : MonoBehaviour
    {
        #region --- Members ---

        protected bool _isReady;

        #endregion


        #region --- Properties ---

        public abstract string PanelName { get; }

        #endregion


        #region --- Mono Override ---

        protected virtual void Awake()
        {
            if (SupersonicWisdom.Api.IsReady())
            {
                OnSwReady();
            }
            else
            {
                SupersonicWisdom.Api.AddOnReadyListener(OnSwReady);
            }
        }

        #endregion


        #region --- Private Methods ---

        protected virtual void OnSwReady()
        {
            _isReady = true;
        }

        #endregion
    }
}
#endif