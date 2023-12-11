using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionFinishScreen : MonoBehaviour
{
    public ScratchItemModel scratchItemModel;
    public List<Item> AllCollection;
    public List<GameObject> Places;
    public int index = 0;
    [SerializeField] Button CloseButton;

    [SerializeField] GameObject CollectionLine;
    //the collection in the main page
    [SerializeField] public GameObject MainCollection_List;
    //collection list prefab
    public GameObject Item;
    GameObject CollectionUI_Holder;

    [SerializeField] CanvasGroup CollectionCanvas;

    [SerializeField] Transform InstantiatePlace;

    public void StartAnimation(List<Item> Collection)
    {
        CollectionCanvas.blocksRaycasts = false;

        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();

        AllCollection = Collection;
        SetInCollectionList(MainCollection_List, AllCollection);

    }
    public void SetModels(List<Item> Collection)
    {
        StartCoroutine(SpawnAndMoveModels(Collection));
    }

    private IEnumerator SpawnAndMoveModels(List<Item> Collection)
    {
        for (int i = 0; i < Collection.Count; i++)
        {
            GameObject model = Instantiate(new GameObject(), InstantiatePlace.position, InstantiatePlace.rotation, MainCollection_List.transform);
            Destroy(GameObject.Find("New Game Object"));
            model.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            model.AddComponent<ScratchItemModel>().ChangeModel(Collection[i].imagePath);
            foreach (Transform child in model.transform)
            {
                child.transform.rotation = model.transform.rotation;
                child.transform.Rotate(new Vector3(0, 160, 0));
            }

            bool isMoving = true;

            StartCoroutine(MoveItemToPlace(model, Places[i], () => isMoving = false));

            // Wait until the MoveItemToPlace coroutine sets isMoving to false
            while (isMoving)
            {
                yield return null;
            }
        }
        ShowClosing();
    }

    private IEnumerator MoveItemToPlace(GameObject Model, GameObject Place, System.Action onComplete)
    {
        while (Vector3.Distance(Model.transform.position, Place.transform.position) > 0.1f && Vector3.Distance(Model.transform.GetChild(0).rotation.eulerAngles, new Vector3(0,360,0)) > 0.1f)
        {
            // Calculate the new position using Lerp
            Vector3 newPosition = Vector3.Lerp(Model.transform.position, Place.transform.position, 5f * Time.deltaTime);
            Model.transform.position = newPosition;
            Model.transform.GetChild(0).Rotate(0, 30 * Time.deltaTime, 0);
            yield return null;
        }

        onComplete?.Invoke(); // Invoke the callback when the movement is complete
    }
    public void ShowClosing()
    {
        CloseButton.gameObject.SetActive(true);
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

        Places.Clear();
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
            newItem.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            Destroy(newItem.transform.GetChild(0).gameObject);
            Places.Add(newItem);
            newItem.name = string.Format(collection[i].imagePath);
        }    
        SetModels(AllCollection);

    }

}
