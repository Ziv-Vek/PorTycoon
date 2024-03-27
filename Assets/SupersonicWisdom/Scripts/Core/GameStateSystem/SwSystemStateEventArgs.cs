using System;

namespace SupersonicWisdomSDK
{
    internal class SwSystemStateEventArgs : EventArgs
    {
        public SwSystemState.EStateEvent StateEndStatusRequest { get; set; }
        public SwSystemState.EGameState NewGameState { get; set; }
        public SwSystemState.EGameState? PreviousGameState { get; set; }
        public SwGameStateProgressionData ProgressionData { get; set; }

        public override string ToString ()
        {
            return $"{nameof(SwSystemStateEventArgs)}: {nameof(NewGameState)}: {NewGameState.ToString()}, {nameof(StateEndStatusRequest)}: {StateEndStatusRequest.ToString()}";
        }
    }
}