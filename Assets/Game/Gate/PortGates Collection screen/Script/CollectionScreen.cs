using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CollectionScreen : MonoBehaviour
{
    [SerializeField] GameObject CollectionLine;
    //the collection in the main page
    [SerializeField] public GameObject MainCollection_List;
    //collection list prefab
    public GameObject Item;
    GameObject CollectionUI_Holder;
    Transform cameraT;

    void Start()
    {
        SetInCollectionList(MainCollection_List, 1);
        cameraT = GameObject.Find("Follow Camera").transform;
    }
    private void Update()
    {
        transform.LookAt(cameraT);
    }
    public void SetInCollectionList(GameObject CollectionList, int level)
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
        }
    }
}
