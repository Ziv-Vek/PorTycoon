using UnityEngine;
#if FIREBASE
using Firebase;
using Firebase.Analytics;
#endif

namespace YsoCorp {
    namespace GameUtils {

        [DefaultExecutionOrder(-15)]
        public class FirebaseManager : BaseManager {

            private void Start() {
#if FIREBASE
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith((task) => {
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                });
#endif
            }

        }
    }
}
