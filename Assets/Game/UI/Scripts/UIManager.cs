using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] GameObject UpgradeCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }
    public void UpdateStarsText(int stars)
    {
        starsText.text = stars.ToString();
    }
    public void OpenUpgradeCanvas()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        UpgradeCanvas.SetActive(true);
    }

    public static void ShowWinPanel()
    {
        Debug.Log("ShowWinPanel");
    }
}
