using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Product : MonoBehaviour
{
    public int Price = 000; 
    int Level = 1;
    GameConfig gameConfig;
   
    void Update()
    {
        transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Price.ToString();
        if (gameObject.name == "ship_speed")
            Level = GameManager.Instance.shipSpeedLevel;
        if (gameObject.name == "ship_box_quantity")
            Level = GameManager.Instance.quantityLevel;
        if (gameObject.name == "Quality")
            Level = GameManager.Instance.qualityLevel;
        if (gameObject.name == "conveyor_speed")
            Level = GameManager.Instance.convayorSpeedLevel;
        if (gameObject.name == "conveyor_scanning_speed")
            Level = GameManager.Instance.scanningSpeedLevel;
        if (gameObject.name == "table_quantity")
            Level = GameManager.Instance.tableStackLevel;
        if (gameObject.name == "handyman_speed")
            Level = GameManager.Instance.openBoxTimeNpc;
        if (gameObject.name == "handyman_awarness")
            Level = GameManager.Instance.awarenessTimeNpc;
        if (gameObject.name == "player_speed")
            Level = GameManager.Instance.playerSpeedLevel;
        if (gameObject.name == "player_box_quantity")
            Level = GameManager.Instance.playerBoxPlacesLevel; 
        if (gameObject.name == "forklift_speed")
            Level = GameManager.Instance.forklifSpeedLevel;
        if (gameObject.name == "forklift_box_quantity")
            Level = GameManager.Instance.forkliftBoxQuantityLevel;
        if (gameObject.name == "forklift_fuel_tank")
            Level = GameManager.Instance.forkliftFuelTankLevel;

        transform.Find("Level (Text)").GetComponent<TextMeshProUGUI>().text = "Level: " + Level;
        if ((Level == 3 && gameObject.name == "table_quantity") || (Level == 4 && (gameObject.name == "ship_box_quantity" || gameObject.name == "forklift_box_quantity")) || Level == 5)
        {
           transform.GetChild(1).gameObject.SetActive(false);
           transform.GetChild(2).gameObject.SetActive(false);
            transform.Find("Level (Text)").GetComponent<TextMeshProUGUI>().text = "MAX ("+Level+")";
            transform.Find("Level (Text)").GetComponent<TextMeshProUGUI>().color = Color.red;
        } 
        //else if(Level == 5)
        //{
        //    transform.GetChild(1).gameObject.SetActive(false);
        //    transform.GetChild(2).gameObject.SetActive(false);
        //}
    }
    private void OnEnable()
    {
        Update();
    }

    void Start()
    {
        gameConfig = ConfigManager.Instance.Config;
        try
        {
            if ((Level < 4 && (gameObject.name == "ship_box_quantity" || gameObject.name == "forklift_box_quantity")) || Level < 5)
                GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades[gameObject.name].prices[Level - 1];
        }catch { }      
    }
}
