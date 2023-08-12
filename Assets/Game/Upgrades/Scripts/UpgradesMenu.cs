using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UpgradesMenu : MonoBehaviour
{
    [SerializeField] GameObject[] Panels;
    [SerializeField] TextMeshProUGUI Money;
    private void Update()
    {
        Money.text = GameManager.Instance.money + " $";
    }
    public void Exit()
    {
        for (int i = 0; i < Panels.Length; i++)
        {
            if (Panels[i].activeSelf)
                Panels[i].SetActive(false);
        }
        Panels[0].SetActive(true);
        PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
        playerMover.ToggleMovement(true);
        playerMover.ShowJoystick();
        gameObject.SetActive(false);  
    }
    public void PanelChoose(GameObject Button)
    {
        for (int i = 0; i < Panels.Length; i++)
        {
            if (Panels[i].activeSelf)
                Panels[i].SetActive(false);
        }
        Panels[int.Parse(Button.name[0].ToString())].SetActive(true);
    }
    public void ShipSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.ShipSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            float speed = FindAnyObjectByType<ShipController>().shipSpeed *= 1.2f;
            FindAnyObjectByType<ShipController>().shipSpeed = speed;
            GameManager.Instance.ShipSpeedLevel++;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ShipQuantity(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.QualityLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindAnyObjectByType<ShipCarrier>().addBoxPlace();
            GameManager.Instance.QuantityLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price *= 2;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ConveyorSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.ConvayorSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<Conveyor>().beltSpeed += 0.125f;
            GameManager.Instance.ConvayorSpeedLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price *= 2;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ScanningSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.ScanningSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<Scanner>().scanningDuration--;
            GameManager.Instance.ScanningSpeedLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void BoxStackTable(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.TableStackLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<TableCarrier>().addBoxPlace();
            GameManager.Instance.TableStackLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void NPC_OpenBoxTime(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.OpenBoxTime_NPC < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<TableNPC>().waitTime -= 10;
            GameManager.Instance.OpenBoxTime_NPC++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
  
    } 
    public void NPC_AwarenessTime(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.AwarenessTime_NPC < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<TableNPC>().AwarenessSeconds += 35;
            GameManager.Instance.AwarenessTime_NPC++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void PlayerSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.PlayerSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            GameObject.Find("Player").GetComponent<PlayerMover>().maxMovementSpeed *= 1.25f;
            GameManager.Instance.PlayerSpeedLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void PlayerBoxPlaces(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.PlayerBoxPlacesLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            GameObject.Find("Player").GetComponent<PlayerCarrier>().addBoxPlace();
            GameManager.Instance.PlayerBoxPlacesLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ForkliftBoxPlaces(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.ForkliftBoxQuantityLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<ForkliftCarrier>().addBoxPlace();
            GameManager.Instance.ForkliftBoxQuantityLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ForkliftFuelTank(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.ForkliftFuelTankLevel< 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<ForkliftMover>().FuelSlider.maxValue += 30;
            FindObjectOfType<ForkliftMover>().FuelSlider.value = FindObjectOfType<ForkliftMover>().FuelSlider.maxValue;
            GameManager.Instance.ForkliftFuelTankLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.ShipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
}
