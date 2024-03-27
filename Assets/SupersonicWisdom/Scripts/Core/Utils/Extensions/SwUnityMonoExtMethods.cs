using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal static class SwUnityMonoExtMethods
    {
        #region --- Public Methods ---

        public static void DontDestroyOnLoad(this GameObject gameObject)
        {
            Object.DontDestroyOnLoad(gameObject);
        }

        public static void RenderLast(this Canvas canvas)
        {
            canvas.transform.SetAsLastSibling();
            canvas.sortingOrder = short.MaxValue;
        }

        #endregion
    }
}