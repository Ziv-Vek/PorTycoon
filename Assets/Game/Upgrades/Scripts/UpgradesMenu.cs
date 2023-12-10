using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
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
        Money.text = "$" + GameManager.Instance.money;
        if (Money.text.Length > 8)
            Money.text = "$" + Money.text.Substring(0, 7) + "..";
    }

    private void OnEnable()
    {
        if (GameManager.Instance.HandyManNumber < 1 && gameObject.name == "HR Upgrades Canvas")
            transform.Find("UI Holder").Find("1 HandyMan Button").gameObject.GetComponent<Button>().interactable =
                false;
        else if (gameObject.name == "HR Upgrades Canvas")
            transform.Find("UI Holder").Find("1 HandyMan Button").gameObject.GetComponent<Button>().interactable = true;

        if (gameObject.name == "logistic Upgrades Canvas")
            transform.Find("UI Holder").Find("2 ForkLift Button").gameObject.GetComponent<Button>().interactable =
                GameManager.Instance.ForkliftIsEnabled;
    }

    public void RunCloseAnimation()
    {
        transform.Find("UI Holder").GetComponent<Animator>().Play("Close UI", 0);
        VibrationManager.Instance.LightVibrate();
        AudioManager.Instance.Play("Close UI Window");
    }

    public void Exit()
    {
        for (int i = 0; i < Panels.Length; i++)
        {
            if (Panels[i].activeSelf)
                Panels[i].SetActive(false);
        }

        Panels[0].SetActive(true);
        if (!NewItemCanvas.activeSelf)
        {
            PlayerMover playerMover = GameObject.Find("Player").GetComponent<PlayerMover>();
            try
            {
                playerMover = GameObject.Find("Player_New").GetComponent<PlayerMover>();
            }
            catch
            {
            }

            playerMover.ToggleMovement(true);
            playerMover.ShowJoystick();
            playerMover.joystick.DeactivateJoystick();
        }

        gameObject.SetActive(false);
    }

    public void PanelChoose(GameObject Button)
    {
        foreach (var t in Panels)
        {
            if (t.activeSelf)
                t.SetActive(false);
        }

        Panels[int.Parse(Button.name[0].ToString())].SetActive(true);
        VibrationManager.Instance.LightVibrate();
        AudioManager.Instance.Play("Panel Selected");
    }

    public void ShipSpeed(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.shipSpeedLevel < 5)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.shipSpeedLevel++;
            foreach (GameObject ship in GameObject.Find(GameManager.Instance.currentLevel + "Port")
                         .GetComponent<PortLoader>().Ships)
            {
                ship.GetComponent<ShipController>().setSpeed(gameConfig.levels[0].upgrades["ship_speed"]
                    .levels[GameManager.Instance.shipSpeedLevel - 1]);
                ;
            }

            if (GameManager.Instance.shipSpeedLevel < 5)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0].upgrades["ship_speed"]
                    .prices[GameManager.Instance.shipSpeedLevel - 1];
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.shipSpeedLevel == 5)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void ShipQuantity(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.quantityLevel < 4)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            foreach (GameObject ship in GameObject.Find(GameManager.Instance.currentLevel + "Port")
                         .GetComponent<PortLoader>().Ships)
            {
                ship.GetComponent<ShipCarrier>().AddBoxPlace();
            }

            GameManager.Instance.quantityLevel++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            if (GameManager.Instance.quantityLevel < 4)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["ship_box_quantity"].prices[GameManager.Instance.quantityLevel - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.qualityLevel == 4)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void ConveyorSpeed(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.convayorSpeedLevel < 5)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.convayorSpeedLevel++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ConveyorTable
                .GetComponent<Conveyor>().beltSpeed = gameConfig.levels[0].upgrades["conveyor_speed"]
                .levels[GameManager.Instance.convayorSpeedLevel - 1];
            if (GameManager.Instance.convayorSpeedLevel < 5)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["conveyor_speed"].prices[GameManager.Instance.convayorSpeedLevel - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.convayorSpeedLevel == 5)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void ScanningSpeed(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.scanningSpeedLevel < 5)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.scanningSpeedLevel++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ConveyorTable
                .transform.Find("Scanner").GetComponent<Scanner>().scanningDuration = gameConfig.levels[0]
                .upgrades["conveyor_scanning_speed"].levels[GameManager.Instance.scanningSpeedLevel - 1];
            if (GameManager.Instance.scanningSpeedLevel < 5)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["conveyor_scanning_speed"].prices[GameManager.Instance.scanningSpeedLevel - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.scanningSpeedLevel == 5)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void BoxStackTable(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.tableStackLevel < 3)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().BoxTable.transform
                .Find("Table").GetComponent<TableCarrier>().AddBoxPlace();
            GameManager.Instance.tableStackLevel++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            if (GameManager.Instance.tableStackLevel < 5)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["table_quantity"].prices[GameManager.Instance.tableStackLevel - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.tableStackLevel == 5)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void NPC_OpenBoxTime(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.openBoxTimeNpc < 5)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.openBoxTimeNpc++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            foreach (GameObject npc in GameObject.Find(GameManager.Instance.currentLevel + "Port")
                         .GetComponent<PortLoader>().HandyMan)
            {
                npc.GetComponent<TableNPC>().waitTime = (int)gameConfig.levels[0].upgrades["handyman_speed"]
                    .levels[GameManager.Instance.openBoxTimeNpc - 1];
            }

            if (GameManager.Instance.openBoxTimeNpc < 5)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["handyman_speed"].prices[GameManager.Instance.openBoxTimeNpc - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.openBoxTimeNpc == 5)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void NPC_AwarenessTime(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.awarenessTimeNpc < 5)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.awarenessTimeNpc++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            foreach (GameObject npc in GameObject.Find(GameManager.Instance.currentLevel + "Port")
                         .GetComponent<PortLoader>().HandyMan)
            {
                npc.GetComponent<TableNPC>().AwarenessSeconds = (int)gameConfig.levels[0].upgrades["handyman_awarness"]
                    .levels[GameManager.Instance.awarenessTimeNpc - 1];
            }

            if (GameManager.Instance.awarenessTimeNpc < 5)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["handyman_awarness"].prices[GameManager.Instance.awarenessTimeNpc - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.awarenessTimeNpc == 5)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void PlayerSpeed(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.playerSpeedLevel < 5)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);

            GameManager.Instance.playerSpeedLevel++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            GameObject.Find("Player").GetComponent<PlayerMover>().maxMovementSpeed = gameConfig.levels[0]
                .upgrades["player_speed"].levels[GameManager.Instance.playerSpeedLevel - 1];
            if (GameManager.Instance.playerSpeedLevel < 5)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["player_speed"].prices[GameManager.Instance.playerSpeedLevel - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.playerSpeedLevel == 5)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void PlayerBoxPlaces(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.playerBoxPlacesLevel < 5)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.playerBoxPlacesLevel++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            GameObject.Find("Player").GetComponent<PlayerCarrier>().addBoxPlace();
            if (GameManager.Instance.playerBoxPlacesLevel < 5)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["player_box_quantity"].prices[GameManager.Instance.playerBoxPlacesLevel - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.playerBoxPlacesLevel == 5)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void ForkliftSpeed(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.forklifSpeedLevel < 5)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.forklifSpeedLevel++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ForkLift
                .GetComponent<NavMeshAgent>().speed = gameConfig.levels[0].upgrades["forklift_speed"]
                .levels[GameManager.Instance.forklifSpeedLevel - 1];
            if (GameManager.Instance.forklifSpeedLevel < 5)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["forklift_speed"].prices[GameManager.Instance.forklifSpeedLevel - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.forklifSpeedLevel == 5)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void ForkliftBoxPlaces(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.forkliftBoxQuantityLevel < 5)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.forkliftBoxQuantityLevel++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ForkLift
                .GetComponent<ForkliftCarrier>().addBoxPlace();
            if (GameManager.Instance.forkliftBoxQuantityLevel < 4)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["forklift_box_quantity"].prices[GameManager.Instance.forkliftBoxQuantityLevel - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.forkliftBoxQuantityLevel == 4)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void ForkliftFuelTank(GameObject button)
    {
        if ((button.transform.parent.GetComponent<Product>().Price <= GameManager.Instance.money ||
             button.name == "FreeButton") && GameManager.Instance.forkliftFuelTankLevel < 5)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -=
                    button.transform.parent.GetComponent<Product>().Price);
            GameManager.Instance.forkliftFuelTankLevel++;
            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");
            GameObject.Find(GameManager.Instance.currentLevel + "Port").GetComponent<PortLoader>().ForkLift
                .GetComponent<ForkliftMover>().FuelUpgrade((int)gameConfig.levels[0].upgrades["forklift_fuel_tank"]
                    .levels[GameManager.Instance.forkliftFuelTankLevel - 1]);
            if (GameManager.Instance.forklifSpeedLevel < 5)
                button.transform.parent.GetComponent<Product>().Price = (int)gameConfig.levels[0]
                    .upgrades["forklift_fuel_tank"].prices[GameManager.Instance.forkliftFuelTankLevel - 1];
        }
        else
        {
            if (button.transform.parent.GetComponent<Product>().Price > GameManager.Instance.money)
                Debug.Log("dont have enough money to upgrade: " + button.transform.parent.name);
            if (GameManager.Instance.forkliftFuelTankLevel == 5)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }
}