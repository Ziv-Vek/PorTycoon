using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackNavigationButtonHandler : MonoBehaviour
{
    [SerializeField] private float maxClicksIntervalTime = 2.5f;
    [SerializeField] private int buttonClicksRequiredForAction = 3;
    private float timer = 0;
    private int clickCounter = 0;

    [SerializeField] private TextMeshProUGUI confirmationText;

    private void OnEnable()
    {
        TouchManager.onTouchPhoneBackNavBtn += TouchPhoneBackNavBtnHandler;
    }

    private void OnDisable()
    {
        TouchManager.onTouchPhoneBackNavBtn -= TouchPhoneBackNavBtnHandler;
    }

    private void Update()
    {
        if (clickCounter == 0) return;

        Timer();
        if (clickCounter >= buttonClicksRequiredForAction)
            ShowExitConfirmation();
    }

    void Timer()
    {
        timer += Time.deltaTime;

        if (timer > maxClicksIntervalTime)
        {
            clickCounter = 0;
            timer = 0;
        }
    }

    private void TouchPhoneBackNavBtnHandler()
    {
        clickCounter++;
    }

    void ShowExitConfirmation()
    {
        if (!confirmationText.enabled)
        {
            confirmationText.enabled = true;
        }

        ExitGame();
    }

    void ExitGame()
    {
        // GameManager.Instance.SaveUserData();
        Application.Quit();
    }
}
