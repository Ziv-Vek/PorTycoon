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


    private void Update()
    {
        stars.text = GameManager.Instance.stars.ToString();
        if (stars.text.Length > 7)
            stars.text = stars.text.Substring(0, 6) + "..$";
    }
    public void RunCloseAnimation()
    {
        transform.Find("UI Holder").GetComponent<Animator>().Play("Close UI", 0);
    }
    public void Exit()
    {
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
            newItem.AddComponent<Image>();
            if (!ItemsManager.Instance.UnlockedItems.ContainsKey(ItemsManager.Instance.GetAllLevelItems(level)[i].id))
                newItem.GetComponent<Image>().color = new Color(0, 0, 0);
            newItem.AddComponent<ScratchItemImage>()
                .ChangeImage(ItemsManager.Instance.GetAllLevelItems(level)[i].imagePath);
            newItem.name = string.Format("Item {0} ({1})", i, ItemsManager.Instance.GetAllLevelItems(level)[i].id);
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
        MainPanel.SetActive(false);
        button.interactable = false;
        transform.Find("UI Holder").Find("Current Collection Button").GetComponent<Button>().interactable = true;
        AllCollectionsPanel.SetActive(true);
        SetAllCollectionsList();
    } 
    public void OpenMainPanel(Button button)
    {
        MainPanel.SetActive(true);
        button.interactable = false;
        transform.Find("UI Holder").Find("All Collections Button").GetComponent<Button>().interactable = true;
        AllCollectionsPanel.SetActive(false);
        SetInCollectionList(MainCollection_List,GameManager.Instance.currentLevel);
    }
}