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
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel].shipSpeedLevel;
        if (gameObject.name == "ship_box_quantity")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel].quantityLevel;
        if (gameObject.name == "Quality")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel].qualityLevel;
        if (gameObject.name == "conveyor_speed")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel].convayorSpeedLevel;
        if (gameObject.name == "conveyor_scanning_speed")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel].scanningSpeedLevel;
        if (gameObject.name == "table_quantity")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel].tableStackLevel;
        if (gameObject.name == "handyman_speed")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel].openBoxTimeNpc;
        if (gameObject.name == "handyman_awarness")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel].awarenessTimeNpc;
        if (gameObject.name == "player_speed")
            Level = GameManager.Instance.playerSpeedLevel;
        if (gameObject.name == "player_box_quantity")
            Level = GameManager.Instance.playerBoxPlacesLevel;
        if (gameObject.name == "forklift_speed")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel].forklifSpeedLevel;
        if (gameObject.name == "forklift_box_quantity")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel]
                .forkliftBoxQuantityLevel;
        if (gameObject.name == "forklift_fuel_tank")
            Level = GameManager.Instance.LevelsData["Port" + GameManager.Instance.currentLevel].forkliftFuelTankLevel;

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
    }

    private void OnEnable()
    {
        transform.Find("BuyButton").gameObject.SetActive(true);
        transform.Find("FreeButton").gameObject.SetActive(true);
        transform.Find("FreeButton").GetComponent<Button>().interactable = true;
        transform.Find("Level (Text)").GetComponent<TextMeshProUGUI>().color = Color.white;
        gameConfig = ConfigManager.Instance.Config;
        try
        {
            if ((Level < 4 && (gameObject.name == "ship_box_quantity" || gameObject.name == "forklift_box_quantity")) ||
                Level < 5)
                GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1]
                    .upgrades[gameObject.name].prices[Level - 1];
        }
        catch
        {
        }
    }
}