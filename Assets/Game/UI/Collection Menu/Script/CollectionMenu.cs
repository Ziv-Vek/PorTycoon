using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionMenu : MonoBehaviour
{
    [SerializeField] GameObject CollectionCarrier;
    GameObject CollectionUI_Holder;
    [SerializeField] GameObject Collections_Holder;
    public GameObject Item;
    [SerializeField] private TextMeshProUGUI stars;

    private void Update()
    {
        stars.text = "Stars: " + GameManager.Instance.stars;
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
                CollectionUI_Holder = Instantiate(CollectionCarrier, Collections_Holder.transform.position, Quaternion.identity, Collections_Holder.transform);
            }
            GameObject newItem = Instantiate(Item, CollectionUI_Holder.transform.position, Quaternion.identity, CollectionUI_Holder.transform);
            newItem.AddComponent<Image>();
            if (!ItemsManager.Instance.UnlockedItems.ContainsKey(ItemsManager.Instance.GetAllLevelItems(1)[i].id))
                newItem.GetComponent<Image>().color = new Color(0, 0, 0);
            newItem.AddComponent<ScratchItemImage>().ChangeImage(ItemsManager.Instance.GetAllLevelItems(1)[i].imagePath);
            newItem.name = string.Format("Item {0} ({1})", i, ItemsManager.Instance.GetAllLevelItems(1)[i].id);
        } 
    }
}
