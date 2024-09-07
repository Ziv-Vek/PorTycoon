using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace YsoCorp {

    namespace GameUtils {

        [DefaultExecutionOrder(-15)]
        public class MmpManager : BaseManager {

            public UnityEvent OnMmpsInit;

            public TenjinManager tenjinManager;

            private List<MmpBaseManager> _mmps = new List<MmpBaseManager>();


            private void Awake() {
                this.ycManager.mmpManager = this;
                if (this.ycManager.ycConfig.MmpTenjin) {
                    this._mmps.Add(this.tenjinManager);
                }
            }

            public void Init() {
                foreach (MmpBaseManager mmp in this._mmps) {
                    mmp.gameObject.SetActive(true);
                    mmp.Init();
                }
                this.OnMmpsInit?.Invoke();
            }

            public void SendEvent(string eventName) {
                foreach (MmpBaseManager mmp in this._mmps) {
                    mmp.SendEvent(eventName);
                }
            }
            public void SetConsent(bool consent) {
                foreach (MmpBaseManager mmp in this._mmps) {
                    mmp.SetConsent(consent);
                }
            }

        }

    }

}

