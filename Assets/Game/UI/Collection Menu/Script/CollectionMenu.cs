using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionMenu : MonoBehaviour
{
    [SerializeField] GameObject CollectionLine;
    //the collection in the main page
    [SerializeField] public GameObject MainCollection_List;
    //the collections list in the collections page
    [SerializeField] public GameObject CollectionS_List;
    //collection list prefab
    [SerializeField] GameObject Collection_List;
    [SerializeField] GameObject NameCollectionText;
    public GameObject Item;
    [SerializeField] private TextMeshProUGUI stars;
    [SerializeField] ScratchBoard scratch;

    public GameObject MainPanel;
    public GameObject AllCollectionsPanel;
    GameObject CollectionUI_Holder;
    public GameObject ItemScreen;


    private void Update()
    {
        stars.text = GameManager.Instance.stars.ToString();
        if (stars.text.Length > 7)
            stars.text = stars.text.Substring(0, 6) + "..";
    }
    public void RunCloseAnimation()
    {
        transform.Find("UI Holder").GetComponent<Animator>().Play("Close UI", 0);
        VibrationManager.Instance.LightVibrate();
        AudioManager.inctece.play("Close UI Window");
    }
    public void Exit()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        try
        {
            playerMover = GameObject.Find("Player_New").GetComponent<PlayerMover>();
            VibrationManager.Instance.LightVibrate();
        }
        catch
        {
        }

        playerMover.ToggleMovement(true);
        playerMover.ShowJoystick();

        MainPanel.SetActive(true);
        AllCollectionsPanel.SetActive(false);

        transform.Find("UI Holder").Find("All Collections Button").GetComponent<Button>().interactable = true;
        transform.Find("UI Holder").Find("Current Collection Button").GetComponent<Button>().interactable = false;
        gameObject.SetActive(false);
    }

    public void SetInCollectionList(GameObject CollectionList , int level)
    {
        foreach (Transform child in CollectionList.transform)
        {
            Destroy(child.gameObject);
        }

        //Adding all of the collection Items to UI "list" in the Collection canvas
        for (int i = 0; i < ItemsManager.Instance.GetAllLevelItems(level).Count; i++)
        {
            if (CollectionList.transform.childCount == 0 || i % 3 == 0)
            {
                CollectionUI_Holder = Instantiate(CollectionLine, CollectionList.transform.position,
                    CollectionList.transform.rotation, CollectionList.transform);
            }

            GameObject newItem = Instantiate(Item, CollectionUI_Holder.transform.position, CollectionUI_Holder.transform.rotation,
                CollectionUI_Holder.transform);
            newItem.transform.GetChild(0).gameObject.AddComponent<Image>();
            if (!ItemsManager.Instance.UnlockedItems.ContainsKey(ItemsManager.Instance.GetAllLevelItems(level)[i].id))
                newItem.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(0, 0, 0);
            newItem.transform.GetChild(0).gameObject.AddComponent<ScratchItemImage>()
                .ChangeImage(ItemsManager.Instance.GetAllLevelItems(level)[i].imagePath);
            newItem.name = string.Format(ItemsManager.Instance.GetAllLevelItems(level)[i].imagePath);
            if (ItemsManager.Instance.UnlockedItems.ContainsKey(ItemsManager.Instance.GetAllLevelItems(level)[i].id))
                newItem.GetComponent<Button>().onClick.AddListener(() => ItemPressed(newItem));
        }
    }
    public void SetAllCollectionsList()
    {
        foreach (Transform child in CollectionS_List.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < 5; i++)
        {
            Instantiate(NameCollectionText, CollectionS_List.transform.position, CollectionS_List.transform.rotation, CollectionS_List.transform).GetComponent<TextMeshProUGUI>().text = "Collection:  " + i;
            SetInCollectionList(Instantiate(Collection_List, CollectionS_List.transform.position, CollectionS_List.transform.rotation, CollectionS_List.transform),1);
        }
    }

    public void BuyingBox1(GameObject Button)
    {
        var boxProduct = Button.transform.parent.GetComponent<BoxProduct>();

        if ((boxProduct.Price <= GameManager.Instance.stars || Button.name == "FreeButton"))
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateStarsText(GameManager.Instance.stars -= boxProduct.Price);
            var newBox = Button.AddComponent<PortBox>();
            newBox.isPurchasedBox = true;
            gameObject.transform.SetAsLastSibling();
            scratch.Open(newBox);
            gameObject.SetActive(false);
        }
        else
        {
            if (boxProduct.Price > GameManager.Instance.stars)
                Debug.Log("dont have enough money to buy: " + Button.transform.parent.name);
        }
    }
    public void OpenAllCollectionsPanel(Button button)
    {
        AudioManager.inctece.play("Panel Selected");
        MainPanel.SetActive(false);
        button.interactable = false;
        transform.Find("UI Holder").Find("Current Collection Button").GetComponent<Button>().interactable = true;
        AllCollectionsPanel.SetActive(true);
        SetAllCollectionsList();
    } 
    public void OpenMainPanel(Button button)
    {
        AudioManager.inctece.play("Panel Selected");
        MainPanel.SetActive(true);
        button.interactable = false;
        transform.Find("UI Holder").Find("All Collections Button").GetComponent<Button>().interactable = true;
        AllCollectionsPanel.SetActive(false);
        SetInCollectionList(MainCollection_List,GameManager.Instance.currentLevel);
    }
    public void ItemPressed(GameObject Button)
    {
        ItemScreen.SetActive(true);
        ItemScreen.transform.Find("ItemPlace").rotation = Quaternion.EulerAngles(0, 0, 0);
        ItemScreen.transform.Find("RotateItemOnY").rotation = Quaternion.EulerAngles(0, 0, 0);
        ItemScreen.transform.Find("ItemPlace").GetComponent<ScratchItemModel>().ChangeModel(Button.name);
        ScratchItemModel ItemModel = ItemScreen.transform.Find("ItemPlace").GetComponent<ScratchItemModel>();

        ItemScreen.transform.Find("Frame").GetComponent<Image>().sprite = Button.GetComponent<Image>().sprite;

        GameObject item = ItemModel.transform.GetChild(0).gameObject;
        foreach (Transform child in ItemModel.transform)
        {
            
            child.transform.rotation = item.transform.parent.rotation;
            child.transform.Rotate(new Vector3(0, 160, 0));
            child.transform.position = item.transform.parent.position - new Vector3(0, 3, 0);
        }
        VibrationManager.Instance.LightVibrate();
        AudioManager.inctece.play("Button Click");
    }
    public void CloseItemScreen()
    {
        Destroy(ItemScreen.transform.Find("ItemPlace").GetChild(0).gameObject);
        ItemScreen.transform.Find("ItemPlace").rotation = Quaternion.EulerAngles(0, 0, 0);
        ItemScreen.transform.Find("RotateItemOnY").rotation = Quaternion.EulerAngles(0, 0, 0);
        ItemScreen.SetActive(false);
        VibrationManager.Instance.LightVibrate();
        AudioManager.inctece.play("Button Click");
    }
}