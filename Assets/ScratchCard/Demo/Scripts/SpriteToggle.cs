using UnityEngine;
using UnityEngine.UI;

namespace ScratchCardAsset.Demo
{
    public class SpriteToggle : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image imageOn;
        [SerializeField] private Image imageOff;
        
        private void Awake()
        {
            OnToggle(toggle.isOn);
            toggle.onValueChanged.AddListener(OnToggle);
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnToggle);
        }

        private void OnToggle(bool isToggleOn)
        {
            imageOn.gameObject.SetActive(isToggleOn);
            imageOff.gameObject.SetActive(!isToggleOn);
        }
    }
}