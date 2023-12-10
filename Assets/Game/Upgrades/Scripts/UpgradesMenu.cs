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
    LevelData CurrentlevelData;
   
    private void Start()
    {
        gameConfig = ConfigManager.Instance.Config;
    } 
    private void Update()
    {
        Money.text = "$" + GameManager.Instance.money;
        if (Money.text.Length > 8)
        Money.text = "$" + Money.text.Substring(0, 7) + "..";
    }
    private void OnEnable()
    {
        CurrentlevelData = GameManager.Instance.levelsData["Port" + GameManager.Instance.currentLevel];

        if (CurrentlevelData.HandyManNumber < 1 && gameObject.name == "HR Upgrades Canvas")
            transform.Find("UI Holder").Find("1 HandyMan Button").gameObject.GetComponent<Button>().interactable = false;   

        else if (gameObject.name == "HR Upgrades Canvas")
            transform.Find("UI Holder").Find("1 HandyMan Button").gameObject.GetComponent<Button>().interactable = true;

        if (gameObject.name == "logistic Upgrades Canvas")
            transform.Find("UI Holder").Find("2 ForkLift Button").gameObject.GetComponent<Button>().interactable = CurrentlevelData.ForkliftIsEnabled;
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
            playerMover.joystick.DeactivateJoystick();
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
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && CurrentlevelData.shipSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            CurrentlevelData.shipSpeedLevel++;
            foreach (GameObject ship in GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().Ships)
            {
                ship.GetComponent<ShipController>().setSpeed(gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["ship_speed"].levels[CurrentlevelData.shipSpeedLevel - 1]); ;
            }
            if (CurrentlevelData.shipSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["ship_speed"].prices[CurrentlevelData.shipSpeedLevel - 1];
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (CurrentlevelData.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ShipQuantity(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && CurrentlevelData.quantityLevel < 4)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            foreach (GameObject ship in GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().Ships)
            {
                ship.GetComponent<ShipCarrier>().addBoxPlace();
            }
            CurrentlevelData.quantityLevel++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            if (CurrentlevelData.quantityLevel < 4)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["ship_box_quantity"].prices[CurrentlevelData.quantityLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (CurrentlevelData.qualityLevel == 4)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ConveyorSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && CurrentlevelData.convayorSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            CurrentlevelData.convayorSpeedLevel++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ConveyorTable.GetComponent<Conveyor>().beltSpeed = gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["conveyor_speed"].levels[CurrentlevelData.convayorSpeedLevel - 1];
            if(CurrentlevelData.convayorSpeedLevel < 5)
            Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["conveyor_speed"].prices[CurrentlevelData.convayorSpeedLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (CurrentlevelData.convayorSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ScanningSpeed(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && CurrentlevelData.scanningSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            CurrentlevelData.scanningSpeedLevel++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ConveyorTable.transform.Find("Scanner").GetComponent<Scanner>().scanningDuration = gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["conveyor_scanning_speed"].levels[CurrentlevelData.scanningSpeedLevel - 1];
            if (CurrentlevelData.scanningSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["conveyor_scanning_speed"].prices[CurrentlevelData.scanningSpeedLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (CurrentlevelData.scanningSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void BoxStackTable(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && CurrentlevelData.tableStackLevel < 3)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().BoxTable.transform.Find("Table").GetComponent<TableCarrier>().addBoxPlace();
            CurrentlevelData.tableStackLevel++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            if (CurrentlevelData.tableStackLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["table_quantity"].prices[CurrentlevelData.tableStackLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (CurrentlevelData.tableStackLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void NPC_OpenBoxTime(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && CurrentlevelData.openBoxTimeNpc < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            CurrentlevelData.openBoxTimeNpc++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            foreach (GameObject npc in GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().HandyMan)
            {
                npc.GetComponent<TableNPC>().waitTime = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["handyman_speed"].levels[CurrentlevelData.openBoxTimeNpc - 1];
            }
            if (CurrentlevelData.openBoxTimeNpc < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["handyman_speed"].prices[CurrentlevelData.openBoxTimeNpc - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (CurrentlevelData.openBoxTimeNpc == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
  
    } 
    public void NPC_AwarenessTime(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && CurrentlevelData.awarenessTimeNpc < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            CurrentlevelData.awarenessTimeNpc++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            foreach (GameObject npc in GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().HandyMan)
            {
                npc.GetComponent<TableNPC>().AwarenessSeconds = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["handyman_awarness"].levels[CurrentlevelData.awarenessTimeNpc - 1];
            }
            if (CurrentlevelData.awarenessTimeNpc < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["handyman_awarness"].prices[CurrentlevelData.awarenessTimeNpc - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (CurrentlevelData.awarenessTimeNpc == 5)
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
            GameObject.Find("Player").GetComponent<PlayerMover>().maxMovementSpeed = gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["player_speed"].levels[GameManager.Instance.playerSpeedLevel - 1];
            if (GameManager.Instance.playerSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["player_speed"].prices[GameManager.Instance.playerSpeedLevel - 1];
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
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["player_box_quantity"].prices[GameManager.Instance.playerBoxPlacesLevel - 1];
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
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && CurrentlevelData.forklifSpeedLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            CurrentlevelData.forklifSpeedLevel++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ForkLift.GetComponent<NavMeshAgent>().speed = gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["forklift_speed"].levels[CurrentlevelData.forklifSpeedLevel - 1];
            if (CurrentlevelData.forklifSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["forklift_speed"].prices[CurrentlevelData.forklifSpeedLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (CurrentlevelData.forklifSpeedLevel == 5)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ForkliftBoxPlaces(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && CurrentlevelData.forkliftBoxQuantityLevel < 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            CurrentlevelData.forkliftBoxQuantityLevel++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ForkLift.GetComponent<ForkliftCarrier>().addBoxPlace();     
            if (CurrentlevelData.forkliftBoxQuantityLevel < 4)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["forklift_box_quantity"].prices[CurrentlevelData.forkliftBoxQuantityLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (CurrentlevelData.forkliftBoxQuantityLevel == 4)
                Debug.Log("Max Level: " + Button.transform.parent.name);
        }
    }
    public void ForkliftFuelTank(GameObject Button)
    {
        if ((Button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money || Button.name == "FreeButton") && CurrentlevelData.forkliftFuelTankLevel< 5)
        {
            if (Button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= Button.transform.parent.GetComponent<Product>().Price);
            CurrentlevelData.forkliftFuelTankLevel++;
            VibrationManager.Instance.MediumeVivrate();
            AudioManager.inctece.play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ForkLift.GetComponent<ForkliftMover>().FuelUpgrade((int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["forklift_fuel_tank"].levels[CurrentlevelData.forkliftFuelTankLevel - 1]);
            if (CurrentlevelData.forklifSpeedLevel < 5)
                Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["forklift_fuel_tank"].prices[CurrentlevelData.forkliftFuelTankLevel - 1];
        }
        else
        {
            if (Button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + Button.transform.parent.name);
            if (CurrentlevelData.forkliftFuelTankLevel == 5)
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

    //        GameObject.Find("ScratchCard").GetComponent<ScratchCardManagerInspector>() = gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["forklift_fuel_tank"].levels[GameManager.Instance.forkliftFuelTankLevel - 1];
    //        if (GameManager.Instance.forklifSpeedLevel < 5)
    //            Button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[GameManager.Instance.currentLevel - 1].upgrades["forklift_fuel_tank"].prices[GameManager.Instance.forkliftFuelTankLevel - 1];
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
