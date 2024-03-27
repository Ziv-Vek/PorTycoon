#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SupersonicWisdomSDK
{
    internal class SwStage10DataBridgeTesterCanvas : SwBaseTestPanel
    {
        #region --- Inspector ---
        
        [SerializeField] private LayoutGroup _layoutGroup;
        [SerializeField] private Sprite _buttonSprite;
        [SerializeField] private TextMeshProUGUI _answerContainer;
        
        #endregion
        
        
        #region --- Properties
        
        public override string PanelName
        {
            get { return "DataBridge"; }
        }

        #endregion


        #region --- Private Methods ---
        
        protected override void OnSwReady()
        {
            base.OnSwReady();
            
            PopulateScreenWithGetters();
        }
        
        private void PopulateScreenWithGetters()
        {
            foreach (ESwGetDataFlag enumValue in Enum.GetValues(typeof(ESwGetDataFlag)))
            {
                var buttonGo = new GameObject("Button_" + enumValue);
                var button = buttonGo.AddComponent<Button>();
                var buttonImage = buttonGo.AddComponent<Image>();
                buttonImage.sprite = _buttonSprite;
                button.targetGraphic = buttonImage;
                
                var buttonText = new GameObject("ButtonText").AddComponent<TextMeshProUGUI>();
                buttonText.transform.SetParent(buttonGo.transform, false);
                
                buttonText.text = enumValue.ToString();
                buttonText.alignment = TextAlignmentOptions.Center;
                buttonText.autoSizeTextContainer = true;
                buttonText.fontStyle = FontStyles.Bold;
                buttonText.color = Color.black;

                button.onClick.AddListener(() => OnButtonClick(enumValue));

                buttonGo.transform.SetParent(_layoutGroup.transform, false);
            }
        }

        private void OnButtonClick(ESwGetDataFlag flag)
        {
            var value = SupersonicWisdom.Api.GetDataBasedOnFlagsAsJsonString(flag);
            _answerContainer.text = value;
        }

        #endregion
    }
}
#endif