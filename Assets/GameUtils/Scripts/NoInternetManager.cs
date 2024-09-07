using System;
using UnityEngine;

namespace YsoCorp {
    namespace GameUtils {

        [DefaultExecutionOrder(-15)]
        public class NoInternetManager : BaseManager {

            private void Awake() {
                this.ycManager.noInternetManager = this;
            }

            [Obsolete("NoInternetManager will be removed. Please remove any call")]
            public void CheckInternet() { }

        }
    }
}
