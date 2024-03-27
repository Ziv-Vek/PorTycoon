using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SupersonicWisdomSDK
{
    public class SwDevtoolsTestCaseMono : MonoBehaviour
    {
        #region --- Inspector ---
        
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _statusImage;
        [SerializeField] private Sprite _failedSprite;
        [SerializeField] private Sprite _completedSprite;
        [SerializeField] private Sprite _pendingSprite;
        [SerializeField] private Sprite _runningSprite;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _info;

        #endregion


        #region --- Members ---

        private ESwTestCaseStatus _status;
        private readonly Color _baseColor = new Color(186f/ 255f, 191/ 255f, 248/ 255f, 0.9f);
        private readonly Color _grayedOutColor = new Color(186f/ 255f, 191/ 255f, 248/ 255f, 0.3f);

        #endregion


        #region --- Properties ---

        public ESwTestCaseStatus Status
        {
            set
            {
                _status = value;
                _statusImage.sprite = GetCurrentSprite(_status);
                _backgroundImage.color = GetCurrentColor(_status);
            }
        }

        #endregion
        

        #region --- Public Methods ---

        public void Setup(ISwIntegrationToolTestCaseViewData data)
        {
            if (data == null)
            {
                Destroy(gameObject);
            }
            else
            {
                _title.SetText(data.Title);
                _info.SetText(data.Description);
                _statusImage.sprite = GetCurrentSprite(data.Status);
                _backgroundImage.color = GetCurrentColor(data.Status);
            }
        }

        public void SetLayoutElementPriority(ESwTestCaseStatus status)
        {
            switch (status)
            {
                case ESwTestCaseStatus.Failed:
                    transform.SetAsFirstSibling();

                    break;

                case ESwTestCaseStatus.Completed:
                    transform.SetAsLastSibling();

                    break;
            }
        }

        #endregion


        #region --- Private Methods ---

        private Sprite GetCurrentSprite(ESwTestCaseStatus status)
        {
            switch (status)
            {
                case ESwTestCaseStatus.Pending:
                    return _pendingSprite;
                case ESwTestCaseStatus.Running:
                    return _runningSprite;
                case ESwTestCaseStatus.Completed:
                    return _completedSprite;
                case ESwTestCaseStatus.Failed:
                    return _failedSprite;
                default:
                    return _failedSprite;
            }
        }

        private Color GetCurrentColor(ESwTestCaseStatus status)
        {
            switch (status)
            {
                case ESwTestCaseStatus.Running:
                case ESwTestCaseStatus.Failed:
                    return _baseColor;
                case ESwTestCaseStatus.Pending:
                case ESwTestCaseStatus.Completed:
                default:
                    return _grayedOutColor;
            }
        }

        #endregion
    }
}