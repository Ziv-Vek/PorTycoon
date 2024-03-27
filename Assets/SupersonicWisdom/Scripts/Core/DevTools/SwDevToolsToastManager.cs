using System;
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwDevToolsToastManager : MonoBehaviour
    {
        #region --- Inspector ---

        [SerializeField] private GameObject _toastPrefab;

        #endregion


        #region --- Members ---

        private int _nextOffset;
        private Queue<SwToast> _toasts;
        private HashSet<SwToast> _currentToastsShowing;

        #endregion


        #region --- Public Methods ---

        public void Setup(int poolSize = 6)
        {
            _toasts = new Queue<SwToast>();
            _currentToastsShowing = new HashSet<SwToast>();

            for (var i = 0; i < poolSize; i++)
            {
                CreateToast();
            }
        }

        public void ShowToast(string message, float impressionIntervalInSeconds = 2f, Action onAccept = null, Action onReject = null)
        {
            if (_toasts.SwIsEmpty())
            {
                CreateToast();
            }

            var currentAvailableToast = _toasts.Dequeue();

            // This effect render order and make sure that the last toast is rendered behind the previous ones
            currentAvailableToast.gameObject.transform.SetAsFirstSibling();

            currentAvailableToast.EnableWithApproval(message, onAccept != null, onReject != null, impressionIntervalInSeconds, () =>
            {
                onAccept?.Invoke();
                OnToastDone(currentAvailableToast);
            }, () =>
            {
                onReject?.Invoke();
                OnToastDone(currentAvailableToast);
            }, () => { OnToastDone(currentAvailableToast); }, _nextOffset);

            _nextOffset++;
            _currentToastsShowing.Add(currentAvailableToast);
        }

        public void DisableAll()
        {
            foreach (var toast in _toasts)
            {
                toast.gameObject.SetActive(false);
            }
        }

        #endregion


        #region --- Private Methods ---

        private void OnToastDone(SwToast currentAvailableToast)
        {
            _toasts.Enqueue(currentAvailableToast);
            _currentToastsShowing.Remove(currentAvailableToast);

            if (_currentToastsShowing.Count == 0)
            {
                _nextOffset = 0;
            }
        }

        private void CreateToast()
        {
            var toastGameObject = Instantiate(_toastPrefab, transform);
            _toasts.Enqueue(toastGameObject.GetComponent<SwToast>());
        }

        #endregion
    }
}