using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Product : MonoBehaviour
{
    public int Price = 000;
    int Level = 1;
    GameConfig gameConfig;

    void Update()
    {
        transform.Find("BuyButton").GetChild(0).GetComponent<TextMeshProUGUI>().text = Price.ToString();
        if (gameObject.name == "ship_speed")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level].shipSpeedLevel;
        if (gameObject.name == "ship_box_quantity")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level].quantityLevel;
        if (gameObject.name == "Quality")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level].qualityLevel;
        if (gameObject.name == "conveyor_speed")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level].convayorSpeedLevel;
        if (gameObject.name == "conveyor_scanning_speed")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level].scanningSpeedLevel;
        if (gameObject.name == "table_quantity")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level].tableStackLevel;
        if (gameObject.name == "handyman_speed")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level].openBoxTimeNpc;
        if (gameObject.name == "handyman_awarness")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level].awarenessTimeNpc;
        if (gameObject.name == "player_speed")
            Level = GameManager.Instance.playerSpeedLevel;
        if (gameObject.name == "player_box_quantity")
            Level = GameManager.Instance.playerBoxPlacesLevel;
        if (gameObject.name == "forklift_speed")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level].forklifSpeedLevel;
        if (gameObject.name == "forklift_box_quantity")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level]
                .forkliftBoxQuantityLevel;
        if (gameObject.name == "forklift_fuel_tank")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level].forkliftFuelTankLevel;

        transform.Find("Level (Text)").GetComponent<TextMeshProUGUI>().text = "Level: " + Level;
        if ((Level == 3 && gameObject.name == "table_quantity") ||
            (Level == 4 && (gameObject.name == "ship_box_quantity" || gameObject.name == "forklift_box_quantity")) ||
            Level == 5)
        {
            transform.Find("BuyButton").gameObject.SetActive(false);
            transform.Find("FreeButton").gameObject.SetActive(false);
            transform.Find("Level (Text)").GetComponent<TextMeshProUGUI>().text = "MAX (" + Level + ")";
            transform.Find("Level (Text)").GetComponent<TextMeshProUGUI>().color = Color.red;

        }  
        try
        {
            if ((Level < 4 && (gameObject.name == "ship_box_quantity" || gameObject.name == "forklift_box_quantity")) ||
                Level < 5)
                GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.level - 1]
                    .upgrades[gameObject.name].prices[Level - 1];
        }
        catch
        {
        }
    }

    public void OnEnable()
    {
        transform.Find("BuyButton").gameObject.SetActive(true);
        transform.Find("FreeButton").gameObject.SetActive(true);
        transform.Find("FreeButton").GetComponent<Button>().interactable = true;
        transform.Find("Level (Text)").GetComponent<TextMeshProUGUI>().color = Color.white;
        gameConfig = ConfigManager.Instance.Config;
      
    }
}