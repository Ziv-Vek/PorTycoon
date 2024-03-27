#if SW_STAGE_STAGE10_OR_ABOVE
using UnityEngine;
using SupersonicWisdomSDK;
using UnityEngine.UI;

public class Stage10Demo : MonoBehaviour
{
    #region --- Inspector ---

    [SerializeField] private Button levelCompletedButton;
    [SerializeField] private Button levelFailedButton;
    [SerializeField] private Button levelStartedButton;
    [SerializeField] private Text currentLevelText;
    [SerializeField] private Text readinessStateText;

    #endregion


    #region --- Members ---

    private int _currentLevel = 1;

    #endregion


    #region --- Mono Override ---

    private void Awake ()
    {
        SupersonicWisdom.Api.AddOnReadyListener(OnSupersonicWisdomReady);
        SupersonicWisdom.Api.Initialize();

        levelStartedButton.interactable = false;
        levelFailedButton.interactable = false;
        levelCompletedButton.interactable = false;
    }

    private void Update ()
    {
        currentLevelText.text = _currentLevel.ToString();
    }

    #endregion


    #region --- Public Methods ---

    public void CompleteLevel ()
    {
        SupersonicWisdom.Api.NotifyLevelCompleted(ESwLevelType.Regular, _currentLevel, null);

        levelStartedButton.interactable = true;
        levelFailedButton.interactable = false;
        levelCompletedButton.interactable = false;
        _currentLevel++;
    }

    public void FailLevel ()
    {
        SupersonicWisdom.Api.NotifyLevelFailed(ESwLevelType.Regular, _currentLevel, null);

        levelStartedButton.interactable = true;
        levelFailedButton.interactable = false;
        levelCompletedButton.interactable = false;
    }

    public void StartLevel ()
    {
        SupersonicWisdom.Api.NotifyLevelStarted(ESwLevelType.Regular, _currentLevel, null);

        levelStartedButton.interactable = false;
        levelFailedButton.interactable = true;
        levelCompletedButton.interactable = true;
    }

    #endregion


    #region --- Private Methods ---

    private void OnSupersonicWisdomReady ()
    {
        readinessStateText.text = true.ToString();

        levelStartedButton.interactable = true;
        levelFailedButton.interactable = false;
        levelCompletedButton.interactable = false;
    }

    #endregion
}
#endif