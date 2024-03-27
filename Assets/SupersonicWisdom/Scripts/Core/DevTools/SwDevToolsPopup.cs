using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SupersonicWisdomSDK
{
    internal sealed class SwDevToolsPopup : MonoBehaviour
    {
        #region --- Events ---

        public event Action ClosedEvent;

        #endregion


        #region --- Inspector ---

        [SerializeField] private GameObject _utilsPanel;
        [SerializeField] private GameObject _mainPanel;
        [SerializeField] private SwIntegrationPanel _integrationPanel;

        #endregion


        #region --- Members ---

        private readonly HashSet<SwButton> _currentButtonsGameObjects = new HashSet<SwButton>();

        private LayoutGroup _buttonLayoutGroup;
        private SwButton _buttonPrefab;

        #endregion


        #region --- Properties ---

        public SwIntegrationPanel IntegrationPanel
        {
            get { return _integrationPanel; }
        }

        #endregion


        #region --- Mono Override ---

        private void Awake()
        {
            _mainPanel.SetActive(true);
            _integrationPanel.gameObject.SetActive(false);
            _utilsPanel.SetActive(false);
        }

        #endregion


        #region --- Public Methods ---

        public void SetState(EDevToolsPanel eDevToolsPanel)
        {
            _mainPanel.SetActive(eDevToolsPanel == EDevToolsPanel.Main);
            _utilsPanel.SetActive(eDevToolsPanel == EDevToolsPanel.Utils);
            _integrationPanel.gameObject.SetActive(eDevToolsPanel == EDevToolsPanel.Integration);
        }

        public void ShowUtilsPanel()
        {
            SetState(EDevToolsPanel.Utils);
        }

        public void ShowMainPanel()
        {
            SetState(EDevToolsPanel.Main);
        }

        public void ShowIntegrationPanel()
        {
            SetState(EDevToolsPanel.Integration);
        }

        public void SetIntegrationData(Dictionary<string, ISwIntegrationToolTestCaseViewData> swDevtoolsTestCaseData)
        {
            _integrationPanel.AddTestCase(swDevtoolsTestCaseData);
        }

        #endregion


        #region --- Private Methods ---

        private void CreateButtons(Dictionary<string, Action> buttons)
        {
            if (buttons == null) return;

            foreach (var button in buttons)
            {
                var swButton = Instantiate(_buttonPrefab, _buttonLayoutGroup.transform);
                swButton.Text = button.Key;
                swButton.onClick.AddListener(() => button.Value.Invoke());
                _currentButtonsGameObjects.Add(swButton);
            }
        }

        #endregion


        #region --- Button Methods ---

        public void Setup(SwButton buttonPrefab, LayoutGroup layoutGroup, Action onClosed)
        {
            ClosedEvent += onClosed;
            _buttonPrefab = buttonPrefab;
            _buttonLayoutGroup = layoutGroup;
        }

        public void SetUtilsButtons(Dictionary<string, Action> buttons)
        {
            DestroyAllButtons();
            CreateButtons(buttons);
        }

        public void Hide()
        {
            ClosedEvent?.Invoke();
        }

        private void DestroyAllButtons()
        {
            if (_currentButtonsGameObjects == null) return;

            foreach (var buttonsGameObject in _currentButtonsGameObjects)
            {
                Destroy(buttonsGameObject.gameObject);
            }
        }

        #endregion
    }
}