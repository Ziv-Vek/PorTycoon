using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BackNavigationButtonHandler : MonoBehaviour
{
    [SerializeField] private float maxClicksIntervalTime = 2.5f;
    [SerializeField] private int buttonClicksRequiredForAction = 2;
    private float timer = 0;
    private int clickCounter = 0;
    [SerializeField] GameObject UiHolder;

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
        if(Input.GetKey(KeyCode.Escape))
        {
            TouchPhoneBackNavBtnHandler();
        }
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
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        UiHolder.SetActive(true);
        UiHolder.GetComponent<Animator>().Play("Open UI", 0);
        //   ExitGame();
    }
    public void RunCloseAnimation()
    {
        UiHolder.GetComponent<Animator>().Play("Close UI", 0);
        VibrationManager.Instance.LightVibrate();
        AudioManager.Instance.Play("Close UI Window");
    }
    public void CloseWindow()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        try
        {
            playerMover = GameObject.Find("Player_New").GetComponent<PlayerMover>();
        }
        catch
        {
        }
        if(!GameManager.Instance.ThereUIActive)
        {
            playerMover.ToggleMovement(true);
            playerMover.ShowJoystick();
            playerMover.joystick.DeactivateJoystick();
        }
        UiHolder.SetActive(false);
    }

    public void ExitGame()
    {
        UserDataManager.Instance.SaveUserDataAsync();
        Application.Quit();
    }
}
