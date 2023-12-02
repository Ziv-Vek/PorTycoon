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
    [SerializeField] GameObject NewItemCanvas;
    GameConfig gameConfig;
   
    private void Start()
    {
        gameConfig = ConfigManager.Instance.Config;
    } 
    private void Update()
    {
        Money.text = GameManager.Instance.money + "$";
        if (Money.text.Length > 8)
        Money.text = Money.text.Substring(0, 7) + "..$";
    }
    private void OnEnable()
    {
        if (GameManager.Instance.HandyManNumber < 1 && gameObject.name == "HR Upgrades Canvas")
            transform.Find("UI Holder").Find("1 HandyMan Button").gameObject.GetComponent<Button>().interactable = false;   
        else if (gameObject.name == "HR Upgrades Canvas")
            transform.Find("UI Holder").Find("1 HandyMan Button").gameObject.GetComponent<Button>().interactable = true;

        if (gameObject.name == "logistic Upgrades Canvas")
            transform.Find("UI Holder").Find("2 ForkLift Button").gameObject.GetComponent<Button>().interactable = GameManager.Instance.ForkliftIsEnabled;
    }
    public void RunCloseAnimation()
    {
        transform.Find("UI Holder").GetComponent<Animator>().Play("Close UI", 0);    
        VibrationManager.Instance.LightVibrate();
        AudioManager.inctece.play("Close UI Window");

    }
    public void Exit()
    {
        for (int i = 0; i < Panels.Length; i++)
        {
            if (Panels[i].activeSelf)
                Panels[i].SetActive(false);
        }
        Panels[0].SetActive(true);
        if (!NewItemCanvas.active)
        {
            PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
            try { playerMover = GameObject.Find("Player_New").GetComponent<PlayerMover>(); } catch { }
            playerMover.ToggleMovement(true);
            playerMover.ShowJoystick();
        }
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
        VibrationManager.Instance.LightVibrate();
        AudioManager.inctece.play("Panel Selected");
    }
    public void ShipSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.shipSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price); 
            GameManager.Instance.shipSpeedLevel++;
            foreach (GameObject ship in GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().Ships)
            {
                ship.GetComponent<ShipController>().setSpeed(gameConfig.levels[0].upgrades["ship_speed"].levels[GameManager.Instance.shipSpeedLevel - 1]); ;
            }
            if (GameManager.Instance.shipSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["ship_speed"].prices[GameManager.Instance.shipSpeedLevel - 1];
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
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
            GameManager.Instance.quantityLevel++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
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
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ConveyorTable.GetComponent<Conveyor>().beltSpeed = gameConfig.levels[0].upgrades["conveyor_speed"].levels[GameManager.Instance.convayorSpeedLevel - 1];
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
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ConveyorTable.transform.Find("Scanner").GetComponent<Scanner>().scanningDuration = gameConfig.levels[0].upgrades["conveyor_scanning_speed"].levels[GameManager.Instance.scanningSpeedLevel - 1];
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
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && GameManager.Instance.tableStackLevel < 3)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().BoxTable.transform.Find("Table").GetComponent<TableCarrier>().addBoxPlace();
            GameManager.Instance.tableStackLevel++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
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
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            foreach (GameObject npc in GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().HandyMan)
            {
                npc.GetComponent<TableNPC>().waitTime = (int)gameConfig.levels[0].upgrades["handyman_speed"].levels[GameManager.Instance.openBoxTimeNpc - 1];
            }
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
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            foreach (GameObject npc in GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().HandyMan)
            {
                npc.GetComponent<TableNPC>().AwarenessSeconds = (int)gameConfig.levels[0].upgrades["handyman_awarness"].levels[GameManager.Instance.awarenessTimeNpc - 1];
            }
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
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
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
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
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
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ForkLift.GetComponent<NavMeshAgent>().speed = gameConfig.levels[0].upgrades["forklift_speed"].levels[GameManager.Instance.forklifSpeedLevel - 1];
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
            GameManager.Instance.forkliftBoxQuantityLevel++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ForkLift.GetComponent<ForkliftCarrier>().addBoxPlace();     
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
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ForkLift.GetComponent<ForkliftMover>().FuelUpgrade((int)gameConfig.levels[0].upgrades["forklift_fuel_tank"].levels[GameManager.Instance.forkliftFuelTankLevel - 1]);
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
