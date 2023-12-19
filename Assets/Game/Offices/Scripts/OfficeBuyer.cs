using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeBuyer : Buyer
{
    public override void ActiveProduct(bool isOnPurchaseActivation)
    {
        Debug.Log(isOnPurchaseActivation + " " + product.name);
        product.SetActive(true);

        var productsCompArr = product.GetComponents<IProduct>();
        if (productsCompArr.Length > 0)
        {
            foreach (var activeProduct in product.GetComponents<IProduct>())
            {
                activeProduct.OnProductActivation(isOnPurchaseActivation);
            }
        }
        else // spawn at default position
        {
            product.transform.position = productClone.transform.position;
        }

        try
        {
            NextBuyer.SetActive(true);
        }
        catch
        {
        }


        if (productPlugin != null)
            productPlugin.SetActive(true);

        AudioManager.Instance.Play("Buying Upgrade");

        if (!GameManager.Instance.GoneThroughTutorial && gameObject.name == "HR Office Buyer")
            FindAnyObjectByType<TutorialM>().SetHRofficeShop_Target();
        else if (!GameManager.Instance.GoneThroughTutorial && gameObject.name == "Logistics Office Buyer")
        {
            FindAnyObjectByType<TutorialM>().Invoke(nameof(TutorialM.Instance.StartEndAnimation),1);
            PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
            playerMover.ToggleMovement(false);
            playerMover.HideJoystick();
        }

        StartCoroutine(UserDataManager.Instance.SaveUserDataWithDelay());

        Destroy(gameObject);
    }
}
