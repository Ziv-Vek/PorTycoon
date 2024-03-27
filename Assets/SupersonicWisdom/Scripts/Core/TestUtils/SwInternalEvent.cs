using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal delegate void OnInternal(string internalEvent, Dictionary<string, object> data = null);

    internal class SwInternalEvent
    {
        #region --- Events ---

        internal static event OnInternal OnInternalEvent;

        #endregion


        #region --- Private Methods ---

        internal static void Invoke(string internalEvent, Dictionary<string, object> data = null)
        {
            OnInternalEvent?.Invoke(internalEvent, data);
        }

        #endregion
    }
}