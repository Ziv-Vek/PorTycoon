using UnityEngine;

namespace SupersonicWisdomSDK
{
    public class SwBringToFront : MonoBehaviour
    {
        #region --- Mono Override ---

        private void OnEnable ()
        {
            transform.SetAsLastSibling();
        }

        #endregion
    }
}