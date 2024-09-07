using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace YsoCorp
{
    public static class YCBootstrapper {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ExecuteYCManager() {
            Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("YCManager")));
        }
    }
}