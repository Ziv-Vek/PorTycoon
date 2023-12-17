using TMPro;
using UnityEngine;


[DefaultExecutionOrder(1)]
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] GameObject CollectionCanvas;
    [SerializeField] GameObject SettingsCanvas;
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
    private void Update()
    {
        UpdateCollectionState(ItemsManager.Instance.GetAllLevelItems(GameManager.Instance.experience).Count, ItemsManager.Instance.GetUnlockedItemsNumber(GameManager.Instance.experience));
    }
    public void UpdateUI()
    {
        UpdateCollectionState(ItemsManager.Instance.GetAllLevelItems(GameManager.Instance.CurrentLevel).Count,
            ItemsManager.Instance.UnlockedItems.Count);
        UpdateMoneyText(GameManager.Instance.money);
        UpdateStarsText(GameManager.Instance.stars);
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString();
        if (moneyText.text.Length > 8)
            moneyText.text = moneyText.text.Substring(0, 7) + "..";
    }

    public void UpdateStarsText(int stars)
    {
        starsText.text = stars.ToString();
        if (starsText.text.Length > 7)
            starsText.text = moneyText.text.Substring(0, 6) + "..";
    }

    public void UpdateCollectionState(int totalItems, int unlockedItems)
    {
        CollectionStateText.text = unlockedItems + " / " + totalItems;
    }

    public void OpenCollectionCanvas()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        CollectionCanvas.SetActive(true);
        CollectionCanvas.GetComponent<CollectionMenu>().SetInCollectionList(
            CollectionCanvas.GetComponent<CollectionMenu>().MainCollection_List, GameManager.Instance.experience);
        CollectionCanvas.transform.Find("UI Holder").GetComponent<Animator>().Play("Open UI", 0);
        AudioManager.Instance.Play("Open UI Window");
        if (!GameManager.Instance.GoneThroughTutorial)
            FindAnyObjectByType<TutorialM>().DestroyItSelf();
    }
    public void OpenSettingsCanvas()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        SettingsCanvas.SetActive(true);
        SettingsCanvas.transform.Find("UI Holder").GetComponent<Animator>().Play("Open UI", 0);
        AudioManager.Instance.Play("Open UI Window");
    }

    public static void ShowWinPanel()
    {
        Debug.Log("ShowWinPanel");
    }
}