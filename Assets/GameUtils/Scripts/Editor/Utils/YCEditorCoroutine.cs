using System.Collections;
using UnityEditor;

namespace YsoCorp {
    namespace GameUtils {
        public class YCEditorCoroutine {

			private readonly IEnumerator routine;

			public static YCEditorCoroutine StartCoroutine (IEnumerator enumerator) {
				YCEditorCoroutine coroutine = new YCEditorCoroutine(enumerator);
				coroutine.Start();
				return coroutine;
			}

			YCEditorCoroutine(IEnumerator _routine) {
				this.routine = _routine;
			}

			private void Start() {
				EditorApplication.update += this.Update;
			}
			public void Stop() {
				EditorApplication.update -= this.Update;
			}

			private void Update() {
				if (!this.routine.MoveNext()) {
					this.Stop();
				}
			}
		}
    }
}