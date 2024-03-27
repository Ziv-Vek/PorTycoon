#if SW_STAGE_STAGE10_OR_ABOVE
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace SupersonicWisdomSDK
{
    public class SwIntegrationToolSearchInHierarchyTestCase<T> : SwIntegrationToolManualBaseTestCase where T : MonoBehaviour
    {
        #region --- Construction ---

        public SwIntegrationToolSearchInHierarchyTestCase()
        {
            Status = ESwTestCaseStatus.Running;
            Title = "Searching - " + typeof(T).Name;
            Description = "Searching - " + typeof(T).Name;

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;

            // Searching also on construction since an existing scene might contains component
            SearchInHierarchy();
        }

        #endregion


        #region --- Private Methods ---

        protected internal override void Finish()
        {
            if (Status == ESwTestCaseStatus.Failed) return;

            Status = ESwTestCaseStatus.Completed;
            OnDone();
        }

        protected override string GetID()
        {
            return nameof(SwIntegrationToolSearchInHierarchyTestCase<T>) + "-" + typeof(T).Name;
        }

        private void SearchInHierarchy()
        {
            if (Object.FindObjectOfType<T>() != null)
            {
                Status = ESwTestCaseStatus.Failed;
                OnDone();
            }
        }

        #endregion


        #region --- Event Handler ---

        protected override void OnDone()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            SearchInHierarchy();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            SearchInHierarchy();
        }

        private void OnActiveSceneChanged(Scene sceneOut, Scene sceneIn)
        {
            SearchInHierarchy();
        }

        #endregion
    }
}
#endif