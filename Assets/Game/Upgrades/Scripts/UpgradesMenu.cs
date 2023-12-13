using System;
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
        CurrentlevelData = GameManager.Instance.LevelsData["Port" + GameManager.Instance.level];

        if (CurrentlevelData.HandyManNumber < 1 && gameObject.name == "HR Upgrades Canvas")
            transform.Find("UI Holder").Find("1 HandyMan Button").gameObject.GetComponent<Button>().interactable =
                false;

        else if (gameObject.name == "HR Upgrades Canvas")
            transform.Find("UI Holder").Find("1 HandyMan Button").gameObject.GetComponent<Button>().interactable = true;

        if (gameObject.name == "logistic Upgrades Canvas")
            transform.Find("UI Holder").Find("2 ForkLift Button").gameObject.GetComponent<Button>().interactable =
                CurrentlevelData.ForkliftIsEnabled;
    }

    public void RunCloseAnimation()
    {
        transform.Find("UI Holder").GetComponent<Animator>().Play("Close UI", 0);
        VibrationManager.Instance.LightVibrate();
        AudioManager.Instance.Play("Close UI Window");
    }

    public void Exit()
    {
        foreach (var t in Panels)
        {
            if (t.activeSelf)
                t.SetActive(false);
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

    public void PanelChoose(GameObject button)
    {
        foreach (var t in Panels)
        {
            if (t.activeSelf)
                t.SetActive(false);
        }

        Panels[int.Parse(button.name[0].ToString())].SetActive(true);
        VibrationManager.Instance.LightVibrate();
        AudioManager.Instance.Play("Panel Selected");
    }

    private void HandleUpgrade(GameObject button, Action<int> upgradeAction, string upgradeType, Func<int> getMaxLevel,
        Func<int, int> getPrice)
    {
        var product = button.transform.parent.GetComponent<Product>();
        var currentLevel = getMaxLevel();
        var maxLevel = gameConfig.levels[GameManager.Instance.level - 1].upgrades[upgradeType].levels.Count;

        if ((product.Price <= GameManager.Instance.money || button.name == "FreeButton") && currentLevel < maxLevel)
        {
            if (button.name != "FreeButton")
                UIManager.Instance.UpdateMoneyText(GameManager.Instance.money -= product.Price);

            upgradeAction(currentLevel + 1);

            VibrationManager.Instance.MediumVibrate();
            AudioManager.Instance.Play("Buying Upgrade");

            if (currentLevel + 1 < maxLevel)
                product.Price = getPrice(currentLevel);
        }
        else
        {
            if (product.Price > GameManager.Instance.money)
                Debug.Log("Don't have enough money to upgrade: " + button.transform.parent.name);
            if (currentLevel == maxLevel)
                Debug.Log("Max Level: " + button.transform.parent.name);
        }
    }

    public void ShipSpeed(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                CurrentlevelData.shipSpeedLevel = newLevel;
                foreach (GameObject ship in GameObject.Find(GameManager.Instance.level + "Port")
                             .GetComponent<PortLoader>().Ships)
                {
                    ship.GetComponent<ShipController>().setSpeed(gameConfig
                        .levels[GameManager.Instance.level - 1]
                        .upgrades["ship_speed"].levels[CurrentlevelData.shipSpeedLevel - 1]);
                }
            },
            "ship_speed",
            () => CurrentlevelData.shipSpeedLevel,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["ship_speed"]
                .prices[level - 1]
        );
    }

    public void ShipQuantity(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                CurrentlevelData.quantityLevel = newLevel;
                foreach (GameObject ship in GameObject.Find(GameManager.Instance.level + "Port")
                             .GetComponent<PortLoader>().Ships)
                {
                    ship.GetComponent<ShipCarrier>().AddBoxPlace();
                }
            },
            "ship_box_quantity",
            () => CurrentlevelData.quantityLevel,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["ship_box_quantity"]
                .prices[level - 1]
        );
    }

    public void ConveyorSpeed(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                CurrentlevelData.convayorSpeedLevel = newLevel;
                GameObject.Find(GameManager.Instance.level + "Port").GetComponent<PortLoader>().ConveyorTable
                    .GetComponent<Conveyor>().beltSpeed = gameConfig.levels[GameManager.Instance.level - 1]
                    .upgrades["conveyor_speed"].levels[CurrentlevelData.convayorSpeedLevel - 1];
            },
            "conveyor_speed",
            () => CurrentlevelData.convayorSpeedLevel,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["conveyor_speed"]
                .prices[level - 1]
        );
    }

    public void ScanningSpeed(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                CurrentlevelData.scanningSpeedLevel = newLevel;
                GameObject.Find(GameManager.Instance.level + "Port").GetComponent<PortLoader>().ConveyorTable
                    .transform.Find("Scanner").GetComponent<Scanner>().scanningDuration = gameConfig
                    .levels[GameManager.Instance.level - 1].upgrades["conveyor_scanning_speed"]
                    .levels[CurrentlevelData.scanningSpeedLevel - 1];
            },
            "conveyor_scanning_speed",
            () => CurrentlevelData.scanningSpeedLevel,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["conveyor_scanning_speed"]
                .prices[level - 1]
        );
    }

    public void BoxStackTable(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                CurrentlevelData.tableStackLevel = newLevel;
                GameObject.Find(GameManager.Instance.level + "Port").GetComponent<PortLoader>().BoxTable
                    .transform.Find("Table").GetComponent<TableCarrier>().AddBoxPlace();
            },
            "table_quantity",
            () => CurrentlevelData.tableStackLevel,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["table_quantity"]
                .prices[level - 1]
        );
    }

    public void NPC_OpenBoxTime(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                CurrentlevelData.openBoxTimeNpc = newLevel;
                foreach (GameObject npc in GameObject.Find(GameManager.Instance.level + "Port")
                             .GetComponent<PortLoader>().HandyMan)
                {
                    npc.GetComponent<TableNPC>().waitTime = (int)gameConfig
                        .levels[GameManager.Instance.level - 1]
                        .upgrades["handyman_speed"].levels[CurrentlevelData.openBoxTimeNpc - 1];
                }
            },
            "handyman_speed",
            () => CurrentlevelData.openBoxTimeNpc,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["handyman_speed"]
                .prices[level - 1]
        );
    }

    public void NPC_AwarenessTime(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                CurrentlevelData.awarenessTimeNpc = newLevel;
                foreach (GameObject npc in GameObject.Find(GameManager.Instance.level + "Port")
                             .GetComponent<PortLoader>().HandyMan)
                {
                    npc.GetComponent<TableNPC>().AwarenessSeconds = (int)gameConfig
                        .levels[GameManager.Instance.level - 1].upgrades["handyman_awarness"]
                        .levels[CurrentlevelData.awarenessTimeNpc - 1];
                }
            },
            "handyman_awarness",
            () => CurrentlevelData.awarenessTimeNpc,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["handyman_awarness"]
                .prices[level - 1]
        );
    }


    public void PlayerSpeed(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                GameManager.Instance.playerSpeedLevel = newLevel;
                GameObject.Find("Player").GetComponent<PlayerMover>().maxMovementSpeed = gameConfig
                    .levels[GameManager.Instance.level - 1].upgrades["player_speed"]
                    .levels[GameManager.Instance.playerSpeedLevel - 1];
            },
            "player_speed",
            () => GameManager.Instance.playerSpeedLevel,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["player_speed"]
                .prices[level - 1]
        );
    }

    public void PlayerBoxPlaces(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                GameManager.Instance.playerBoxPlacesLevel = newLevel;
                GameObject.Find("Player").GetComponent<PlayerCarrier>().addBoxPlace();
            },
            "player_box_quantity",
            () => GameManager.Instance.playerBoxPlacesLevel,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["player_box_quantity"]
                .prices[level - 1]
        );
    }

    public void ForkliftSpeed(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                CurrentlevelData.forklifSpeedLevel = newLevel;
                GameObject.Find(GameManager.Instance.level + "Port").GetComponent<PortLoader>().ForkLift
                    .GetComponent<NavMeshAgent>().speed = gameConfig.levels[GameManager.Instance.level - 1]
                    .upgrades["forklift_speed"].levels[CurrentlevelData.forklifSpeedLevel - 1];
            },
            "forklift_speed",
            () => CurrentlevelData.forklifSpeedLevel,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["forklift_speed"]
                .prices[level - 1]
        );
    }

    public void ForkliftBoxPlaces(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                CurrentlevelData.forkliftBoxQuantityLevel = newLevel;
                GameObject.Find(GameManager.Instance.level + "Port").GetComponent<PortLoader>().ForkLift
                    .GetComponent<ForkliftCarrier>().addBoxPlace();
            },
            "forklift_box_quantity",
            () => CurrentlevelData.forkliftBoxQuantityLevel,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["forklift_box_quantity"]
                .prices[level - 1]
        );
    }

    public void ForkliftFuelTank(GameObject button)
    {
        HandleUpgrade(
            button,
            newLevel =>
            {
                CurrentlevelData.forkliftFuelTankLevel = newLevel;
                GameObject.Find(GameManager.Instance.level + "Port").GetComponent<PortLoader>().ForkLift
                    .GetComponent<ForkliftMover>().FuelUpgrade((int)gameConfig
                        .levels[GameManager.Instance.level - 1]
                        .upgrades["forklift_fuel_tank"].levels[CurrentlevelData.forkliftFuelTankLevel - 1]);
            },
            "forklift_fuel_tank",
            () => CurrentlevelData.forkliftFuelTankLevel,
            level => (int)gameConfig.levels[GameManager.Instance.level - 1].upgrades["forklift_fuel_tank"]
                .prices[level - 1]
        );
    }
}