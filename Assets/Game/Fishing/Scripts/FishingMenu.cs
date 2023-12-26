using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FishingMenu : MonoBehaviour
{
    [SerializeField] FishingManager FishingGame; 
    public TMP_Text cooldownText; // Reference to your TextMeshPro text component
    public Button PlayButton;
    public Button BuyingButton;
    public int fishingPrice;

    [SerializeField] bool isCooldownActive = false;
    [SerializeField] float cooldownDuration = 300f; // 5 minutes in seconds
    [SerializeField] float cooldownEndTime = 0f;
    [SerializeField] float remainingTime;

    public void RunCloseAnimation()
    {
        transform.Find("UI Holder").GetComponent<Animator>().Play("Close UI", 0);
        VibrationManager.Instance.LightVibrate();
        AudioManager.Instance.Play("Close UI Window");
    }

    public void Exit()
    {
        PlayButton.gameObject.SetActive(false);
        BuyingButton.gameObject.SetActive(false);
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        try
        {
            playerMover = GameObject.Find("Player_New").GetComponent<PlayerMover>();
        }
        catch
        {
        }
        playerMover.ToggleMovement(true);
        playerMover.ShowJoystick();
        playerMover.joystick.DeactivateJoystick();
        gameObject.SetActive(false);
    }
    private void StartFishing()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        FishingGame.gameObject.SetActive(true);
        FishingGame.StartInvoke();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        AudioManager.Instance.ChangeSounds("General Music", "Fishing Music");
        gameObject.SetActive(false);

    }
    private void OnEnable()
    {
        PlayButton.gameObject.SetActive(false);
        BuyingButton.gameObject.SetActive(false);
        if (isCooldownActive)
            BuyingButton.gameObject.SetActive(true);
        else
            PlayButton.gameObject.SetActive(true);
    }
    private void Start()
    {
        // Load the cooldown end time from PlayerPrefs if available
        LoadCooldownEndTime();

        // Update the cooldown timer
        UpdateCooldownTimer();
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (isCooldownActive)
        {
            UpdateCooldownTimer();
            UpdateCooldownText();
        }
    }

    private void LoadCooldownEndTime()
    {
        if (PlayerPrefs.HasKey("CooldownEndTime"))
        {
            cooldownEndTime = PlayerPrefs.GetFloat("CooldownEndTime");

            // Check if the cooldown is still active
            isCooldownActive = cooldownEndTime > Time.realtimeSinceStartup;

            // If the cooldown is still active, update the remaining time
            if (isCooldownActive)
            {
                float remainingTime = cooldownEndTime - Time.realtimeSinceStartup;
                Debug.Log("Remaining Time: " + remainingTime);
            }
        }
    }

    private void SaveCooldownEndTime()
    {
        PlayerPrefs.SetFloat("CooldownEndTime", remainingTime);
        PlayerPrefs.Save();
    }

    private void UpdateCooldownTimer()
    {
        float currentTime = Time.realtimeSinceStartup;
        if (currentTime < cooldownEndTime)
        {
            isCooldownActive = true;
        }
        else
        {
            isCooldownActive = false;
            cooldownEndTime = 0;
            OnEnable();
            // Save the cooldown end time to PlayerPrefs
            SaveCooldownEndTime();
        }
    }

    private void UpdateCooldownText()
    {
        remainingTime = Mathf.Max(0, cooldownEndTime - Time.realtimeSinceStartup);
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
        string cooldownTextString = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);

        // Update your TextMeshPro text
        cooldownText.text = cooldownTextString;
        SaveCooldownEndTime();
    }

    public void StartMiniGame(GameObject Button)
    {
        if ((Button.name == "Buyng Button" && GameManager.Instance.stars >= fishingPrice) || Button.name == "Play Button")
        {
            // Start the mini-game
            StartFishing();
            // Activate cooldown
            isCooldownActive = true;
            cooldownEndTime = Time.realtimeSinceStartup + cooldownDuration;

            // Save the cooldown end time to PlayerPrefs
            SaveCooldownEndTime();

            // Update the cooldown timer immediately
            UpdateCooldownText();
            if (Button.name == "Buyng Button")
                Bank.Instance.DepositStars(-fishingPrice);
        }
    }
}


