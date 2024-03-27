#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SupersonicWisdomSDK
{
    public class SwBaseTesterManager : MonoBehaviour
    {
        #region --- Inspector ---

        [Header("Buttons")]
        [SerializeField] private GameObject _buttonPrefab;

        [Header("Panels")]
        [SerializeField] private List<SwBaseTestPanel> _panelsPrefabs;
        [SerializeField] private Transform _buttonsParent;

        #endregion


        #region --- Members ---

        private List<Button> _buttons;
        private List<SwBaseTestPanel> _panels;

        #endregion


        #region --- Mono Override ---

        protected virtual void Awake()
        {
            _buttons = new List<Button>();
            _panels = new List<SwBaseTestPanel>();
        }

        private void Start()
        {
            
            if (_panelsPrefabs == null) return;

            foreach (var panel in _panelsPrefabs)
            {
                TryCreatePanel(panel);
            }
            
            ClearPanels();

            InitializeSdk();
        }
        
        #endregion


        #region --- Private Methods ---

        private static void ClickOnButton(Selectable button, Component panel)
        {
            panel.gameObject.SetActive(true);
            button.image.color = Color.grey;
        }

        private void TryCreatePanel(SwBaseTestPanel panel)
        {
            if (panel == null) return;
            
            var panelInstance = Instantiate(panel);
            // Due to the limitation of creating an object and its children within the same frame, we must separate the SetParent call into a new line
            panelInstance.transform.SetParent(transform);
            _panels.Add(panelInstance);
            
            AddButton(panelInstance);
        }

        private void AddButton(SwBaseTestPanel panel)
        {
            var go = Instantiate(_buttonPrefab, _buttonsParent);
            var button = go.GetComponent<Button>();
            var text = button.GetComponentInChildren<TextMeshProUGUI>();

            text.SetText(panel.PanelName);
            button.onClick.AddListener(() => ShowPanel(button, panel));
            _buttons.Add(button);
        }

        private void ShowPanel(Selectable button, Component panel)
        {
            ClearButtons();
            ClearPanels();
            
            ClickOnButton(button, panel);
        }

        private void ClearPanels()
        {
            if (_panels == null) return;
            
            foreach (var panel in _panels)
            {
                if (panel == null) continue;
                
                panel.gameObject.SetActive(false);
            }
        }

        private void ClearButtons()
        {
            if (_buttons == null) return;

            foreach (var button in _buttons)
            {
                button.image.color = Color.white;
            }
        }

        private void InitializeSdk()
        {
            if (SupersonicWisdom.Api.IsReady()) return;
            
            SupersonicWisdom.Api.Initialize();
        }

        #endregion
    }
}
#endif