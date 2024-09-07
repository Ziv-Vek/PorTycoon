using UnityEngine;
using UnityEngine.SceneManagement;

namespace YsoCorp {
    namespace GameUtils {
        public class BaseManager : MonoBehaviour {

            private static YCManager YCMANAGER;
            private static Ads CROSSPROMO;

            public YCManager ycManager { get { return YCMANAGER; } protected set { YCMANAGER = value; } }
            public Ads crossPromo { get { return CROSSPROMO; } protected set { CROSSPROMO = value; } }

            protected bool isQuitting = false;

            protected virtual void OnApplicationQuit() {
                this.isQuitting = true;
            }

            protected virtual void OnDestroy() {
                if (this.isQuitting == false) {
                    this.OnDestroyNotQuitting();
                }
            }

            protected virtual void OnDestroyNotQuitting() { }
        }
    }
}
