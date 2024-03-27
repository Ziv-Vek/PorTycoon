using UnityEngine;

namespace SupersonicWisdomSDK
{
    public abstract class SwMonoBehaviourSingleton<T> : MonoBehaviour where T : Component
    {
        #region --- Properties ---

        public static T Instance { get; private set; }

        #endregion


        #region --- Mono Override ---

        public virtual void Awake ()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}