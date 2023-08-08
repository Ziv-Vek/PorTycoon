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
}
