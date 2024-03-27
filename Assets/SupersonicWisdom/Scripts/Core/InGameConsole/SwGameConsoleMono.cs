using System;
using TMPro;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwGameConsoleMono : MonoBehaviour
    {
        #region --- Inspector ---

        [SerializeField] private TextMeshProUGUI consoleText;

        #endregion


        #region --- Members ---

        private static SwGameConsoleMono _instance;
        private int _lineNumber;

        #endregion


        #region --- Mono Override ---

        private void Awake ()
        {
            //Once created, we make the gameObject persistant
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);

                return;
            }
            
            consoleText.text = "";
            DontDestroyOnLoad(gameObject);
        }

        #endregion


        #region --- Public Methods ---

        public void InternalLogToConsole(string log, string stack, string logType)
        {
            var logColor = LogLevelToColor(logType);

            _lineNumber += 1;
            consoleText.text += $"{_lineNumber}: <color=\"{logColor}\">{log}</color> {Environment.NewLine} {stack}";
        }

        public void ClearConsole()
        {
            consoleText.text = "";
        }

        #endregion


        #region --- Private Methods ---

        private string LogLevelToColor(string logLevel)
        {
            string color;

            switch (logLevel)
            {
                case "Warning":
                    color = "yellow";

                    break;
                case "Error":
                    color = "orange";

                    break;
                case "Exception":
                    color = "red";

                    break;
                default:
                    color = "white";

                    break;
            }

            return color;
        }

        #endregion
    }
}