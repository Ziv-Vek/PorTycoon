using UnityEngine;

public class UiHolder : MonoBehaviour
{
    public void Close()
    {
        if (transform.parent.name == "HR Upgrades Canvas" || transform.parent.name == "logistic Upgrades Canvas")
            transform.parent.GetComponent<UpgradesMenu>().Exit();
        else if (transform.parent.name == "Collection Canvas")
            transform.parent.GetComponent<CollectionMenu>().Exit();
        else if (transform.parent.name == "Settings Canvas")
            transform.parent.GetComponent<Settings>().Exit();
        else if (transform.parent.name == "ExitConfirmationCanvas")
            transform.parent.GetComponent<BackNavigationButtonHandler>().CloseWindow();   
        else if (transform.parent.name == "FishingMenu Canvas")
            transform.parent.GetComponent<FishingMenu>().Exit();
    }
}