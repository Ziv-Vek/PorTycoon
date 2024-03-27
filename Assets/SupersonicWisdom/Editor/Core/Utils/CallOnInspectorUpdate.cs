using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

namespace SupersonicWisdomSDK.Editor
{
    public class CallOnInspectorUpdate
    {
        #region --- Members ---

        private Action _actionToRun;

        #endregion


        #region --- Construction ---

        public CallOnInspectorUpdate(Action actionToRun)
        {
            _actionToRun = actionToRun;

            if (_actionToRun != null)
            {
                EditorApplication.update += _update;
            }
        }

        ~CallOnInspectorUpdate()
        {
            // Destructor
            _actionToRun = null;
        }

        #endregion


        #region --- Private Methods ---

        private void _update()
        {
            EditorApplication.update -= _update;
            _actionToRun?.Invoke();
            _actionToRun = null;
        }

        #endregion
    }
}