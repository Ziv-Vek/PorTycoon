using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewItemScreen : MonoBehaviour
{
    public ScratchItemModel scratchItemModel;
    [SerializeField] CanvasGroup CollectionCanvas;
    public List<Item> ItemsToShow = new();
    private Item CurrentItem;

    public void ShowNewItem(Item newItem)
    {
        CurrentItem = newItem;

        TextMeshProUGUI NameText =
            transform.Find("UI Holder").Find("ItemName").GetChild(0).GetComponent<TextMeshProUGUI>();
        NameText.text = newItem.name;
        scratchItemModel.ChangeModel(newItem.imagePath);
        GameObject item = scratchItemModel.transform.GetChild(0).gameObject;
        foreach (Transform child in scratchItemModel.transform)
        {
            child.transform.rotation = item.transform.parent.rotation;
            child.transform.Rotate(new Vector3(0, 160, 0));
            child.transform.position = item.transform.parent.position;
        }

        scratchItemModel.gameObject.GetComponent<Animator>().Play("ToyPlacment_Anim");

        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(false);
        playerMover.HideJoystick();

        GameManager.Instance.ThereUIActive = true;

        CollectionCanvas.blocksRaycasts = false;
    }

    public void CloseScreen()
    {

        Destroy(scratchItemModel.gameObject.transform.GetChild(0).gameObject);
        ItemsToShow.Remove(CurrentItem);
        if (ItemsToShow.Count > 0)
        {
            ShowNewItem(ItemsToShow[0]);
            return;
        }

        if (GameObject.Find("ScratchBoard") == null &&
            GameObject.Find("Collection Canvas") == null) // check if the player is not scratching at this time
        {
            PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
            playerMover.ToggleMovement(true);
            playerMover.ShowJoystick();  
            GameManager.Instance.ThereUIActive = false;
        }
        CollectionCanvas.blocksRaycasts = true;
        if (GameObject.Find("ScratchBoard") == null)
        {
            FindAnyObjectByType<CameraManager>().setPointerCoinCamera(false);
        }
        gameObject.SetActive(false);
    }

    public void AddItemToList(Item newItem)
    {
        ItemsToShow.Add(newItem);
        if (ItemsToShow.Count == 1)
            ShowNewItem(ItemsToShow[0]);
    }
}