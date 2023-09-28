using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionMenu : MonoBehaviour
{
    [SerializeField] GameObject CollectionLine;
    GameObject CollectionUI_Holder;
    [SerializeField] GameObject Collections_Holder;
    public GameObject Item;
    [SerializeField] private TextMeshProUGUI stars;  
    GameConfig gameConfig;
    [SerializeField] ScratchBoard scratch;


    private void Update()
    {
        stars.text = GameManager.Instance.stars.ToString();
    }
    public void Exit()
    {
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        try { playerMover = GameObject.Find("Player_New").GetComponent<PlayerMover>(); } catch { }
        playerMover.ToggleMovement(true);
        playerMover.ShowJoystick();
        foreach (Transform child in Collections_Holder.transform)
        {Destroy(child.gameObject);}
        gameObject.SetActive(false);
    }
    public void SetInCollectionPanel()
    {
        foreach (Transform child in Collections_Holder.transform)
        { Destroy(child.gameObject); }
        //Adding all of the collection Items to UI "list" in the Collection canvas
        for (int i = 0; i < ItemsManager.Instance.GetAllLevelItems(1).Count; i++)
        {
            if (Collections_Holder.transform.childCount == 0 || i % 3 == 0)
            {
                CollectionUI_Holder = Instantiate(CollectionLine, Collections_Holder.transform.position, Quaternion.identity, Collections_Holder.transform);
            }
            GameObject newItem = Instantiate(Item, CollectionUI_Holder.transform.position, Quaternion.identity, CollectionUI_Holder.transform);
            newItem.AddComponent<Image>();
            if (!ItemsManager.Instance.UnlockedItems.ContainsKey(ItemsManager.Instance.GetAllLevelItems(1)[i].id))
                newItem.GetComponent<Image>().color = new Color(0, 0, 0);
            newItem.AddComponent<ScratchItemImage>().ChangeImage(ItemsManager.Instance.GetAllLevelItems(1)[i].imagePath);
            newItem.name = string.Format("Item {0} ({1})", i, ItemsManager.Instance.GetAllLevelItems(1)[i].id);
        } 
    }
    public void BuyingBox1(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<BoxProduct>().Price <= GameManager.Instance.stars || Button.name == "FreeButton"))
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateStarsText(GameManager.Instance.stars -= Button.transform.parent.GetComponent<BoxProduct>().Price);
            scratch = GameObject.Find(GameManager.Instance.currentLevel + "Port").transform.Find("BoxTable").transform.Find("ScratchBoard").GetComponent<ScratchBoard>();
            PortBox box = new PortBox();
            scratch.Open(box);
        }
        else
        {
            if (Button.transform.parent.GetComponent<BoxProduct>().Price > GameManager.Instance.stars)
                Debug.Log("dont have enough money to buy: " + Button.transform.parent.name);
        }
    }
}
