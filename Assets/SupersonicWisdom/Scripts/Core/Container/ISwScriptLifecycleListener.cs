using System;

namespace SupersonicWisdomSDK
{
    public interface ISwScriptLifecycleListener
    {
        #region --- Mono Override ---

        void OnApplicationPause(bool pauseStatus);

        void OnApplicationQuit();

        #endregion


        #region --- Public Methods ---

        void OnAwake();
        void OnStart();
        void OnUpdate();

        #endregion
    }
}