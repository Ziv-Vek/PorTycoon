using UnityEngine;

namespace SupersonicWisdomSDK
{
    public class SwDrawGui
    {
        #region --- Constants ---

        private const string DEV_BUILD_LABEL = "Development";

        #endregion


        #region --- Members ---

        private readonly float _screenRatio;
        private GUIStyle _watermarkStyle;

        #endregion


        #region --- Construction ---

        public SwDrawGui()
        {
            var baseResolution = new Vector2(1080, 1920);
            
            _screenRatio = Mathf.Sqrt(Screen.width * Screen.height / (baseResolution.x * baseResolution.y));
        }

        #endregion


        #region --- Public Methods ---

        public void ShowLoadDevelopmentWaterMark()
        {
            _watermarkStyle ??= LoadWaterMark();
            
            var width = Screen.width / 2f;
            var height = Screen.height / 2f;
            var pivotPoint = new Vector2(width, height);
            var position = new Rect(width - 200, height - 100, 400, 200);
            
            GUIUtility.RotateAroundPivot(-45, pivotPoint);
            GUI.Label(position, DEV_BUILD_LABEL, _watermarkStyle);
        }

        #endregion


        #region --- Private Methods ---

        private GUIStyle LoadWaterMark()
        {
            return new GUIStyle
            {
                fontSize = Mathf.FloorToInt(150 * _screenRatio),
                normal =
                {
                    textColor = new Color(1f, 1f, 1f, 0.3f)
                },
                alignment = TextAnchor.MiddleCenter
            };
        }

        #endregion
    }
}