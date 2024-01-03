using System;
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
    [SerializeField] GameObject ClickHerePefab;

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

    private void OnEnable()
    {
        ItemsManager.Instance.OnItemUnlocked += UpdateCollectionState;
    }

    private void OnDisable()
    {
        ItemsManager.Instance.OnItemUnlocked -= UpdateCollectionState;
    }

    private void Start()
    {
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        UpdateCollectionState();
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

    public void UpdateCollectionState()
    {
        var totalItems = ItemsManager.Instance.GetAllLevelItems(GameManager.Instance.CurrentLevel).Count;
        var unlockedItems = ItemsManager.Instance.GetUnlockedItemsNumber(GameManager.Instance.CurrentLevel);
        CollectionStateText.text = unlockedItems + " / " + totalItems;
    }

    public void OpenCollectionCanvas()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        CollectionCanvas.SetActive(true);
        CollectionCanvas.GetComponent<CollectionMenu>().OpenMainPanel(null);
        CollectionCanvas.transform.Find("UI Holder").GetComponent<Animator>().Play("Open UI", 0);
        AudioManager.Instance.Play("Open UI Window");
        GameManager.Instance.ThereUIActive = true;

        if (!GameManager.Instance.GoneThroughTutorial)
            FindAnyObjectByType<TutorialM>().SetScratchMoney_Target();
    }
    
    public void OpenSettingsCanvas()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();
        SettingsCanvas.SetActive(true);
        SettingsCanvas.transform.Find("UI Holder").GetComponent<Animator>().Play("Open UI", 0);
        GameManager.Instance.ThereUIActive = true;

        AudioManager.Instance.Play("Open UI Window");
    }

    public static void ShowWinPanel()
    {
        Debug.Log("ShowWinPanel");
    }
    
    public void BuyBox()
    {
        Bank.Instance.DepositStars(5);
        GameObject clickHere;
        if(GameObject.Find("Collection Canvas") == null)
        {
            clickHere = Instantiate(ClickHerePefab);
            clickHere.transform.SetParent(transform);
            clickHere.transform.localScale = Vector3.one;
            clickHere.transform.localPosition = Vector3.zero;
            clickHere.transform.rotation = transform.rotation;
            clickHere.GetComponent<ClickHere>().Target = transform.Find("Collection");
        }
        clickHere = Instantiate(ClickHerePefab);
        clickHere.transform.SetParent(CollectionCanvas.transform.GetChild(0));
        clickHere.transform.localScale = Vector3.one;
        clickHere.transform.localPosition = Vector3.zero;
        clickHere.transform.rotation = transform.rotation;
        clickHere.GetComponent<ClickHere>().Target = CollectionCanvas.GetComponent<CollectionMenu>().BuyBoxButton.gameObject.transform;
        clickHere.transform.GetChild(0).localScale = new Vector3(26, 48, 1); 
    }
}