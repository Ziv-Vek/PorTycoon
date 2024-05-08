using System;

namespace SupersonicWisdomSDK
{
    internal delegate void OnTickDelegate(float elapsed, float remaining);

    internal interface ISwTimer : ISwStartStopTimer
    {
        #region --- Properties ---

        bool PauseWhenUnityOutOfFocus { get; set; }
        bool IsDisabled { get; }
        string Name { get; set; }

        #endregion


        #region --- Public Methods ---

        ISwTimer PauseTimer (bool disableTimer);
        ISwTimer ResumeTimer ();
        void SetDuration (float duration);
        void SetElapsed(float elapsed);

        #endregion
    }

    internal interface ISwStartStopTimer : ISwTimerListener
    {
        ISwStartStopTimer StartTimer ();
        ISwStartStopTimer StopTimer ();
    }

    internal interface ISwTimerListener
    {
        #region --- Events ---

        event Action OnFinishEvent;
        event Action OnStoppedEvent;
        event OnTickDelegate OnTickEvent;

        #endregion


        #region --- Properties ---

        bool DidFinish { get; }
        bool IsPaused { get; }
        bool IsReset { get; }

        float Duration { get; }
        ////////////////////////////////////////////////////
        //          // IsEnabled | IsPaused | DidFinished //
        ////////////////////////////////////////////////////
        //  Start   //     V     |    X     |      X      //    
        //--------- // -----------------------------------//
        //  Pause   //     X     |    V     |      X      //
        //--------- // -----------------------------------//
        //  Resume  //     V     |    X     |      X      //
        //--------- // -----------------------------------//
        //   Stop   //     X     |    X     |      X      //
        //--------- // -----------------------------------//
        //  Finish  //     X     |    X     |      V      //
        ////////////////////////////////////////////////////

        float Elapsed { get; }
        bool IsEnabled { get; }

        #endregion
    }
}