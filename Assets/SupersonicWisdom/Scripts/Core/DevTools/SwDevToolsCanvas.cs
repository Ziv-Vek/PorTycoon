using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SupersonicWisdomSDK
{
    internal class SwDevToolsCanvas : MonoBehaviour
    {
        #region --- Constants ---

        private const float ANIMATION_DURATION = 0.3f;

        #endregion


        #region --- Inspector ---

        [SerializeField] protected Transform popupContainer;
        [SerializeField] private LayoutGroup buttonLayoutGroup;
        [SerializeField] private SwButton buttonPrefab;
        [SerializeField] private SwDevToolsPopup _popup;
        [SerializeField] private SwDevToolsScreenStickyWidget _widget;
        [SerializeField] private SwDevToolsToastManager _toastManager;
        [SerializeField] private GameObject _integrationButton;

        #endregion


        #region --- Properties ---

        public SwIntegrationPanel IntegrationPanel
        {
            get { return _popup.IntegrationPanel; }
        }

        #endregion


        #region --- Mono Override ---

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _widget.ClickedEvent += OnWidgetClicked;
            _integrationButton.SetActive(false);
        }

        private void Start()
        {
            _popup.gameObject.SetActive(false);
            _toastManager.DisableAll();
        }

        #endregion


        #region --- Public Methods ---

        public void Setup()
        {
            _toastManager.Setup();
            _popup.Setup(buttonPrefab, buttonLayoutGroup, OnPopupClosed);
            _popup.gameObject.SetActive(false);
        }

        public void SetUtilButtons(Dictionary<string, Action> buttons)
        {
            _popup.SetUtilsButtons(buttons);
        }

        public void HidePopup()
        {
            _popup.Hide();
        }

        public void ShowToast(string message, float impressionIntervalInSeconds = 2f, Action onAccept = null, Action onReject = null)
        {
            _toastManager.ShowToast(message, impressionIntervalInSeconds, onAccept, onReject);
        }

        public void SetIntegrationData(Dictionary<string, ISwIntegrationToolTestCaseViewData> data)
        {
            _integrationButton.SetActive(true);
            _popup.SetIntegrationData(data);
            _popup.SetState(EDevToolsPanel.Integration);
        }

        public void ShowPopup()
        {
            _popup.gameObject.SetActive(true);
            _popup.EnlargeObject(ANIMATION_DURATION, callback: () => _widget.gameObject.SetActive(false));
        }

        #endregion


        #region --- Private Methods ---

        private void OnPopupClosed()
        {
            _widget.gameObject.SetActive(true);
            _popup.ShrinkObject(ANIMATION_DURATION, callback: () => _popup.gameObject.SetActive(false));
            _widget.Show();
        }

        #endregion


        #region --- Event Handler ---

        private void OnWidgetClicked()
        {
            ShowPopup();
        }

        #endregion
    }
}