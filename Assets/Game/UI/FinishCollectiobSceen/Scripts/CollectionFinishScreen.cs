using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionFinishScreen : MonoBehaviour
{
    public ScratchItemModel scratchItemModel;
    public List<Item> AllCollection;
    public int index = 0;
    private Animator animator;
    [SerializeField] Button CloseButton;

    [SerializeField] GameObject CollectionLine;
    //the collection in the main page
    [SerializeField] public GameObject MainCollection_List;
    //collection list prefab
    public GameObject Item;
    GameObject CollectionUI_Holder;

    [SerializeField] CanvasGroup CollectionCanvas;


    public void StartAnimation(List<Item> Collection)
    {
        CollectionCanvas.blocksRaycasts = false;

        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();

        AllCollection = Collection;
        animator = GetComponent<Animator>();
     //  animator.Play("Showing_all_The_Collection", 0);
         EndLoop();
    }
    public void SetModel()
    {
        if(scratchItemModel.transform.childCount > 0)
        {
            foreach (Transform child in scratchItemModel.transform)
                Destroy(child.gameObject);
        }
        scratchItemModel.ChangeModel(AllCollection[index].imagePath);
        foreach (Transform child in scratchItemModel.transform)
        {
            child.transform.rotation = scratchItemModel.transform.parent.rotation;
            child.transform.Rotate(new Vector3(0, 160, 0));
        }
    }
    public void EndOfAnimation()
    {
        index++;
        if (index < AllCollection.Count)
            animator.Play("Showing_all_The_Collection", 0);
        else
            EndLoop();
    }
    public void EndLoop()
    {
        foreach (Transform child in scratchItemModel.transform)
            Destroy(child.gameObject);
        CloseButton.gameObject.SetActive(true);  
        animator.Play("ShowingCollectionUI", 0);
        SetInCollectionList(MainCollection_List, AllCollection);
    }

    public void CloseWindow()
    {
        CloseButton.gameObject.SetActive(false);
        foreach (Transform child in MainCollection_List.transform)
            Destroy(child.gameObject);
        GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().OpenGatesWithCelebrating();

        if (GameObject.Find("ScratchBoard") == null && GameObject.Find("Collection Canvas") == null)// check if the player is not scratching at this time
        {
            PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
            playerMover.ToggleMovement(true);
            playerMover.ShowJoystick();
        }

        CollectionCanvas.blocksRaycasts = true;

        gameObject.SetActive(false);
    }

    public void SetInCollectionList(GameObject CollectionList, List<Item> collection)
    {
        foreach (Transform child in CollectionList.transform)
        {
            Destroy(child.gameObject);
        }

        //Adding all of the collection Items to UI "list" in the Collection canvas
        for (int i = 0; i < collection.Count; i++)
        {
            if (CollectionList.transform.childCount == 0 || i % 3 == 0)
            {
                CollectionUI_Holder = Instantiate(CollectionLine, CollectionList.transform.position,
                    CollectionList.transform.rotation, CollectionList.transform);
            }

            GameObject newItem = Instantiate(Item, CollectionUI_Holder.transform.position, CollectionUI_Holder.transform.rotation,
                CollectionUI_Holder.transform);
            newItem.transform.GetChild(0).gameObject.AddComponent<Image>();
            if (!ItemsManager.Instance.UnlockedItems.ContainsKey(collection[i].id))
                newItem.transform.GetChild(0).gameObject.GetComponent<Image>().color = new Color(0, 0, 0);
            newItem.transform.GetChild(0).gameObject.AddComponent<ScratchItemImage>()
                .ChangeImage(collection[i].imagePath);
            newItem.name = string.Format(collection[i].imagePath);
        }
    }

}
