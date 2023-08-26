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
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.shipSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            float speed = FindAnyObjectByType<ShipController>().shipSpeed *= 1.2f;
            FindAnyObjectByType<ShipController>().shipSpeed = speed;
            GameManager.Instance.shipSpeedLevel++;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ShipQuantity(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.qualityLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindAnyObjectByType<ShipCarrier>().addBoxPlace();
            GameManager.Instance.quantityLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price *= 2;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ConveyorSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.convayorSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<Conveyor>().beltSpeed += 0.125f;
            GameManager.Instance.convayorSpeedLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price *= 2;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ScanningSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.scanningSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<Scanner>().scanningDuration--;
            GameManager.Instance.scanningSpeedLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void BoxStackTable(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.tableStackLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<TableCarrier>().addBoxPlace();
            GameManager.Instance.tableStackLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void NPC_OpenBoxTime(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.openBoxTimeNpc < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<TableNPC>().waitTime -= 10;
            GameManager.Instance.openBoxTimeNpc++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
  
    } 
    public void NPC_AwarenessTime(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.awarenessTimeNpc < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<TableNPC>().AwarenessSeconds += 35;
            GameManager.Instance.awarenessTimeNpc++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void PlayerSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.playerSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            GameObject.Find("Player").GetComponent<PlayerMover>().maxMovementSpeed *= 1.25f;
            GameManager.Instance.playerSpeedLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void PlayerBoxPlaces(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.playerBoxPlacesLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            GameObject.Find("Player").GetComponent<PlayerCarrier>().addBoxPlace();
            GameManager.Instance.playerBoxPlacesLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ForkliftBoxPlaces(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.forkliftBoxQuantityLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<ForkliftCarrier>().addBoxPlace();
            GameManager.Instance.forkliftBoxQuantityLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ForkliftFuelTank(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Prodact>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.forkliftFuelTankLevel< 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Prodact>().Price);
            FindObjectOfType<ForkliftMover>().FuelUpgrade(30);
            GameManager.Instance.forkliftFuelTankLevel++;
            Button.transform.parent.GetComponent<Prodact>().Price += 100;
        }
        else
        {
            if (Button.transform.parent.GetComponent<Prodact>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
}
