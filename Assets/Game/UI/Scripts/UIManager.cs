using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


[DefaultExecutionOrder(1)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] GameObject UpgradeCanvas;
    [SerializeField] private TextMeshProUGUI totalItemsText;
    [SerializeField] private TextMeshProUGUI unlockedItemsText;


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

        UpdateTotalItemsText(ItemsManager.Instance.GetAllLevelItems(GameManager.Instance.CurrentLevel).Count);
        UpdateUnlockedItemsText(ItemsManager.Instance.UnlockedItems.Count);
        UpdateMoneyText(GameManager.Instance.money);
        UpdateStarsText(GameManager.Instance.stars);
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }

    public void UpdateStarsText(int stars)
    {
        starsText.text = stars.ToString();
    }

    public void UpdateTotalItemsText(int totalItems)
    {
        totalItemsText.text = totalItems.ToString();
    }

    public void UpdateUnlockedItemsText(int unlockedItems)
    {
        unlockedItemsText.text = unlockedItems.ToString();
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