using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using TMPro;

public class UpgradesMenu : MonoBehaviour
{
    [SerializeField] GameObject[] Panels;
    [SerializeField] TextMeshProUGUI Money;
    GameConfig gameConfig;
   
    private void Start()
    {
        gameConfig = ConfigManager.Instance.Config;
    } 
    private void Update()
    {
        Money.text = GameManager.Instance.money + " $";
    }
    private void OnEnable()
    {
        if (GameManager.Instance.HandyManNumber < 1 && gameObject.name == "HR Upgrades Canvas")
            transform.FindChild("1 HandyMan Button").gameObject.GetComponent<Button>().interactable = false;   
        else if (gameObject.name == "HR Upgrades Canvas")
            transform.FindChild("1 HandyMan Button").gameObject.GetComponent<Button>().interactable = true;

        if (gameObject.name == "logistic Upgrades Canvas")
            transform.FindChild("2 ForkLift Button").gameObject.GetComponent<Button>().interactable = GameManager.Instance.ForkliftIsEnabled;
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
        try { playerMover = GameObject.Find("Player_New").GetComponent<PlayerMover>(); } catch { }
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
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.shipSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price); 
            GameManager.Instance.shipSpeedLevel++;
            FindAnyObjectByType<ShipController>().setSpeed(gameConfig.levels[0].upgrades["ship_speed"].levels[GameManager.Instance.shipSpeedLevel - 1]);
            if (GameManager.Instance.shipSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["ship_speed"].prices[GameManager.Instance.shipSpeedLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ShipQuantity(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.quantityLevel < 4)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            foreach (GameObject ship in GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().Ships)
            {
                ship.GetComponent<ShipCarrier>().addBoxPlace();
            }
        //    FindAnyObjectByType<ShipCarrier>().addBoxPlace();
            GameManager.Instance.quantityLevel++;
            if (GameManager.Instance.quantityLevel < 4)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["ship_box_quantity"].prices[GameManager.Instance.quantityLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.qualityLevel == 4)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ConveyorSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.convayorSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.convayorSpeedLevel++;
            FindAnyObjectByType<Conveyor>().beltSpeed = gameConfig.levels[0].upgrades["conveyor_speed"].levels[GameManager.Instance.convayorSpeedLevel - 1];
            if(GameManager.Instance.convayorSpeedLevel < 5)
            Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["conveyor_speed"].prices[GameManager.Instance.convayorSpeedLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.convayorSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ScanningSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.scanningSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.scanningSpeedLevel++;
            FindAnyObjectByType<Scanner>().scanningDuration = gameConfig.levels[0].upgrades["conveyor_scanning_speed"].levels[GameManager.Instance.scanningSpeedLevel - 1];
            if (GameManager.Instance.scanningSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["conveyor_scanning_speed"].prices[GameManager.Instance.scanningSpeedLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.scanningSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void BoxStackTable(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.tableStackLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            FindObjectOfType<TableCarrier>().addBoxPlace();
            GameManager.Instance.tableStackLevel++;
            if (GameManager.Instance.tableStackLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["table_quantity"].prices[GameManager.Instance.tableStackLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.tableStackLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void NPC_OpenBoxTime(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.openBoxTimeNpc < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);   
            GameManager.Instance.openBoxTimeNpc++;
            FindObjectOfType<TableNPC>().waitTime = (int)gameConfig.levels[0].upgrades["handyman_speed"].levels[GameManager.Instance.openBoxTimeNpc - 1];
            if (GameManager.Instance.openBoxTimeNpc < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["handyman_speed"].prices[GameManager.Instance.openBoxTimeNpc - 1];

        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.openBoxTimeNpc == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
  
    } 
    public void NPC_AwarenessTime(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.awarenessTimeNpc < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);          
            GameManager.Instance.awarenessTimeNpc++;
            FindObjectOfType<TableNPC>().AwarenessSeconds = (int)gameConfig.levels[0].upgrades["handyman_awarness"].levels[GameManager.Instance.awarenessTimeNpc - 1];
            if (GameManager.Instance.awarenessTimeNpc < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["handyman_awarness"].prices[GameManager.Instance.awarenessTimeNpc - 1];

        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.awarenessTimeNpc == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void PlayerSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.playerSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);

            GameManager.Instance.playerSpeedLevel++;
            GameObject.Find("Player").GetComponent<PlayerMover>().maxMovementSpeed = gameConfig.levels[0].upgrades["player_speed"].levels[GameManager.Instance.playerSpeedLevel - 1];
            if (GameManager.Instance.playerSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["player_speed"].prices[GameManager.Instance.playerSpeedLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.playerSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void PlayerBoxPlaces(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.playerBoxPlacesLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.playerBoxPlacesLevel++;
            GameObject.Find("Player").GetComponent<PlayerCarrier>().addBoxPlace();
            if (GameManager.Instance.playerBoxPlacesLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["player_box_quantity"].prices[GameManager.Instance.playerBoxPlacesLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.playerBoxPlacesLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ForkliftSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.forklifSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);  
            GameManager.Instance.forklifSpeedLevel++;
            GameObject.Find("Forklift").GetComponent<NavMeshAgent>().speed = gameConfig.levels[0].upgrades["forklift_speed"].levels[GameManager.Instance.forklifSpeedLevel - 1];
            if (GameManager.Instance.forklifSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["forklift_speed"].prices[GameManager.Instance.forklifSpeedLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.forklifSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ForkliftBoxPlaces(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.forkliftBoxQuantityLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            FindObjectOfType<ForkliftCarrier>().addBoxPlace();
            GameManager.Instance.forkliftBoxQuantityLevel++;
            if (GameManager.Instance.forkliftBoxQuantityLevel < 4)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["forklift_box_quantity"].prices[GameManager.Instance.forkliftBoxQuantityLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.forkliftBoxQuantityLevel == 4)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ForkliftFuelTank(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.forkliftFuelTankLevel< 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);           
            GameManager.Instance.forkliftFuelTankLevel++;

            FindObjectOfType<ForkliftMover>().FuelUpgrade((int)gameConfig.levels[0].upgrades["forklift_fuel_tank"].levels[GameManager.Instance.forkliftFuelTankLevel - 1]);
            if (GameManager.Instance.forklifSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["forklift_fuel_tank"].prices[GameManager.Instance.forkliftFuelTankLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (GameManager.Instance.forkliftFuelTankLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    //public void BrushSizeScale(GameObject Button)
    //{
    //    if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.scratchSizeScaleLevel < 5)
    //    {
    //        if (Button.name != "FreeButton")
    //            UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
    //        GameManager.Instance.scratchSizeScaleLevel++;

    //        GameObject.Find("ScratchCard").GetComponent<ScratchCardManagerInspector>() = gameConfig.levels[0].upgrades["forklift_fuel_tank"].levels[GameManager.Instance.forkliftFuelTankLevel - 1];
    //        if (GameManager.Instance.forklifSpeedLevel < 5)
    //            Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["forklift_fuel_tank"].prices[GameManager.Instance.forkliftFuelTankLevel - 1];
    //    }
    //    else
    //    {
    //        if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
    //            Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
    //        if (GameManager.Instance.forkliftFuelTankLevel == 5)
    //            Debug.Log("Max Level: " + Button.transform.parent.name);
    //    }
    //}
}
