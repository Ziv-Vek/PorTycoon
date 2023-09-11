using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Product : MonoBehaviour
{
    public int Price = 000; 
    int Level = 1;
    void Update()
    {
        transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Price.ToString();
        if (gameObject.name == "ShipVelocity")
            Level = GameManager.Instance.shipSpeedLevel;
        if (gameObject.name == "Quantity")
            Level = GameManager.Instance.quantityLevel;
        if (gameObject.name == "Quality")
            Level = GameManager.Instance.qualityLevel;
        if (gameObject.name == "SpeedOfConveyor")
            Level = GameManager.Instance.convayorSpeedLevel;
        if (gameObject.name == "SpeedOfChecking")
            Level = GameManager.Instance.scanningSpeedLevel;
        if (gameObject.name == "StackBoxes")
            Level = GameManager.Instance.tableStackLevel;
        if (gameObject.name == "OpeningSpeed")
            Level = GameManager.Instance.openBoxTimeNpc;
        if (gameObject.name == "Awareness")
            Level = GameManager.Instance.awarenessTimeNpc;
        if (gameObject.name == "PlayerSpeed")
            Level = GameManager.Instance.playerSpeedLevel;
        if (gameObject.name == "BoxPlaces")
            Level = GameManager.Instance.playerBoxPlacesLevel; 
        if (gameObject.name == "SpeedOfForklift")
            Level = GameManager.Instance.forklifSpeedLevel;
        if (gameObject.name == "ForkliftBoxQuantity")
            Level = GameManager.Instance.forkliftBoxQuantityLevel;
        if (gameObject.name == "FuelTank")
            Level = GameManager.Instance.forkliftFuelTankLevel;

        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Level: " + Level;
        if (Level == 4 && (gameObject.name == "Quantity" || gameObject.name == "ForkliftBoxQuantity"))
        {
           transform.GetChild(1).gameObject.SetActive(false);
           transform.GetChild(2).gameObject.SetActive(false);
        } 
        else if(Level == 5)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
        }
    }
}
