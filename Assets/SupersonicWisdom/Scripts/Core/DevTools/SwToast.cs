using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    [RequireComponent(typeof(RectTransform))]
    public class SwToast : MonoBehaviour
    {
        #region --- Constants ---

        private const float SPEED = 6.5f;

        #endregion


        #region --- Inspector ---

        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private GameObject _acceptGameObject;
        [SerializeField] private GameObject _rejectGameObject;

        #endregion


        #region --- Members ---

        private Action _onAccept;
        private Action _onReject;
        private bool _wasClicked;
        private int _offset;
        private RectTransform _rectTransform;
        private Vector3 _baseLocalPosition;
        private Vector3 _startLocalPosition;
        private Vector3 _baseAdditionVector;
        private Vector3 _paddingAdditionVector;
        private Vector3 _endLocalPosition;

        #endregion


        #region --- Mono Override ---

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _baseLocalPosition = transform.localPosition;
            _baseAdditionVector = Vector3.up * 500;
            _paddingAdditionVector = Vector3.up * 200;
        }

        private void OnDisable()
        {
            _acceptGameObject.SetActive(false);
            _rejectGameObject.SetActive(false);
            _onAccept = null;
            _onReject = null;
            _message.SetText(string.Empty);
            _endLocalPosition = _baseLocalPosition;
            _startLocalPosition = _baseLocalPosition;
        }

        #endregion


        #region --- Public Methods ---

        public void EnableWithApproval(string message, bool withApprove, bool withReject, float toastTime = float.MaxValue, Action onAccept = null, Action onReject = null, Action onNoResponse = null, int offeset = 0)
        {
            if (isActiveAndEnabled) return;

            gameObject.SetActive(true);
            _acceptGameObject.SetActive(withApprove);
            _rejectGameObject.SetActive(withReject);
            _message.SetText(message);
            _onAccept = onAccept;
            _onReject = onReject;
            _startLocalPosition = _baseLocalPosition + _baseAdditionVector + _paddingAdditionVector * (offeset - 1);
            _endLocalPosition = _startLocalPosition + _paddingAdditionVector;

            StartCoroutine(SwMonoAnimationHelper.Animate(_rectTransform, SPEED, _startLocalPosition, _endLocalPosition, 2, () => { StartCoroutine(CloseToast(toastTime, onNoResponse)); }));
        }

        public void OnAccept()
        {
            OnClicked();
            RollBack(_onAccept);
        }

        public void OnReject()
        {
            OnClicked();
            RollBack(_onReject);
        }

        #endregion


        #region --- Private Methods ---

        private void OnClicked()
        {
            _wasClicked = true;
        }

        private void RollBack(Action callback)
        {
            StartCoroutine(SwMonoAnimationHelper.Animate(_rectTransform, SPEED, transform.localPosition, _startLocalPosition, 2, () =>
            {
                gameObject.SetActive(false);
                callback?.Invoke();
            }));
        }

        private IEnumerator CloseToast(float toastTime, Action onDone)
        {
            yield return new WaitForSeconds(toastTime);

            if (_wasClicked)
            {
                gameObject.SetActive(false);
            }
            else
            {
                yield return SwMonoAnimationHelper.Animate(_rectTransform, SPEED, _endLocalPosition, _startLocalPosition, 2);

                gameObject.SetActive(false);
                onDone?.Invoke();
            }
        }

        #endregion
    }
}