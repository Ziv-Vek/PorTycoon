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
    [SerializeField] GameObject CollectionCanvas;
    [SerializeField] private TextMeshProUGUI totalItemsText;
    [SerializeField] private TextMeshProUGUI unlockedItemsText;
    [SerializeField] private TextMeshProUGUI CollectionStateText;


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

        UpdateUI();
    }

    public void UpdateUI()
    {
        //UpdateTotalItemsText(ItemsManager.Instance.GetAllLevelItems(GameManager.Instance.CurrentLevel).Count);
        //UpdateUnlockedItemsText(ItemsManager.Instance.UnlockedItems.Count);
        UpdateCollectionState(ItemsManager.Instance.GetAllLevelItems(GameManager.Instance.CurrentLevel).Count, ItemsManager.Instance.UnlockedItems.Count);
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
    public void UpdateCollectionState(int totalItems, int unlockedItems)
    {
        CollectionStateText.text = unlockedItems + " / " + totalItems;
    }
    public void UpdateTotalItemsText(int totalItems)
    {
        totalItemsText.text = totalItems.ToString();
    }

    public void UpdateUnlockedItemsText(int unlockedItems)
    {
        unlockedItemsText.text = unlockedItems.ToString();
    }

    public void OpenCollectionCanvas()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        CollectionCanvas.SetActive(true);
        CollectionCanvas.GetComponent<CollectionMenu>().SetInCollectionPanel();
    }

    public static void ShowWinPanel()
    {
        Debug.Log("ShowWinPanel");
    }
}