using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PortLoader : MonoBehaviour
{
    public int PortLevel;

    GameConfig gameConfig;

    public GameObject[] Ships;
    public GameObject[] ShipsBuyers;
    public GameObject[] HandyMan;
    public GameObject[] HandyManBuyers;
    public GameObject ForkLift;
    public GameObject ForkLiftBuyer;
    public GameObject ConveyorTable;
    public GameObject BoxTable;
    public GameObject Player;
    LevelData CurrentLevelData;
    public GameObject Gates;
    public Transform PlayerSpawnPoint;

    private void Start()
    {
        Player = GameObject.Find("Player");
        string portKey = "Port" + PortLevel;
        GameManager.Instance.LevelsData.TryAdd(portKey, new LevelData());

        CurrentLevelData = GameManager.Instance.LevelsData["Port" + PortLevel];
        gameConfig = ConfigManager.Instance.Config;
        
        //ships values
        for (int i = 0; i < CurrentLevelData.ShipNumber; i++)
        {
            ShipsBuyers[i].GetComponent<Buyer>().ActiveProduct(false);
        }

        for (int i = 0; i < CurrentLevelData.quantityLevel - 1; i++)
        {
            foreach (GameObject ship in Ships)
            {
                ship.GetComponent<ShipCarrier>().AddBoxPlace();
            }
        }

        for (int i = 0; i < CurrentLevelData.shipSpeedLevel; i++)
        {
            foreach (GameObject ship in Ships)
            {
                ship.GetComponent<ShipController>().setSpeed(gameConfig.levels[PortLevel - 1].upgrades["ship_speed"]
                    .levels[CurrentLevelData.shipSpeedLevel - 1]);
            }
        }

        //forklift values
        if (CurrentLevelData.ForkliftIsEnabled)
        {
            ForkLiftBuyer.GetComponent<Buyer>().ActiveProduct(false);
        }

        ForkLift.GetComponent<NavMeshAgent>().speed = gameConfig.levels[PortLevel - 1].upgrades["forklift_speed"]
            .levels[CurrentLevelData.forklifSpeedLevel - 1];
        for (int i = 0; i < CurrentLevelData.forkliftBoxQuantityLevel - 1; i++)
        {
            ForkLift.GetComponent<ForkliftCarrier>().addBoxPlace();
        }

        ForkLift.GetComponent<ForkliftMover>().FuelUpgrade((int)gameConfig.levels[PortLevel - 1]
            .upgrades["forklift_fuel_tank"].levels[CurrentLevelData.forkliftFuelTankLevel - 1]);
        //conveyorTable values
        ConveyorTable.GetComponent<Conveyor>().beltSpeed = gameConfig.levels[PortLevel - 1].upgrades["conveyor_speed"]
            .levels[CurrentLevelData.convayorSpeedLevel - 1];
        ConveyorTable.transform.Find("Scanner").GetComponent<Scanner>().scanningDuration = gameConfig
            .levels[PortLevel - 1].upgrades["conveyor_scanning_speed"].levels[CurrentLevelData.scanningSpeedLevel - 1];
        for (int i = 0; i < CurrentLevelData.tableStackLevel - 1; i++)
            BoxTable.transform.Find("Table").GetComponent<TableCarrier>().AddBoxPlace();
        //handyman values
        for (int i = 0; i < CurrentLevelData.HandyManNumber; i++)
        {
            HandyManBuyers[i].GetComponent<Buyer>().ActiveProduct(false);
        }

        foreach (GameObject npc in HandyMan)
        {
            npc.GetComponent<TableNPC>().waitTime = (int)gameConfig.levels[PortLevel - 1].upgrades["handyman_speed"]
                .levels[CurrentLevelData.openBoxTimeNpc - 1];
            npc.GetComponent<TableNPC>().AwarenessSeconds = (int)gameConfig.levels[PortLevel - 1]
                .upgrades["handyman_awarness"].levels[CurrentLevelData.awarenessTimeNpc - 1];
        }

        if (gameObject.name == "1Port")
        {
            Player.GetComponent<PlayerMover>().maxMovementSpeed = gameConfig.levels[PortLevel - 1]
                .upgrades["player_speed"].levels[GameManager.Instance.playerSpeedLevel - 1];
            for (int i = 0; i < GameManager.Instance.playerBoxPlacesLevel - 1; i++)
            {
                Player.GetComponent<PlayerCarrier>().addBoxPlace();
            }

            if (GameManager.Instance.GoneThroughTutorial)
                FindAnyObjectByType<TutorialM>().DestroyItSelf();
        }
        if (GameManager.Instance.experience > PortLevel)
            OpenGates();
    }

    public void OpenGatesWithCelebrating()
    {
        Gates.transform.Find("Gate 1").GetComponent<Animator>().Play("OpenGate Animation", 0);
        Gates.transform.Find("Gate 2").GetComponent<Animator>().Play("OpenGate Animation", 0);
        Gates.transform.Find("Effects").gameObject.SetActive(true);

        Gates.transform.Find("Gate Camera").gameObject.SetActive(true);
        StartCoroutine(WaitForXSeconds(5f));
        if(PortLevel + 1 <= GameManager.Instance.AmountOfLevels)
        {
            GameManager.Instance.experience = PortLevel + 1;
            GameManager.Instance.CurrentLevel = PortLevel + 1;
        }

        UserDataManager.Instance.SaveUserData();
    }

    public void OpenGates()
    {
        Gates.transform.Find("Gate 1").GetComponent<Animator>().Play("OpenGate Animation", 0);
        Gates.transform.Find("Gate 2").GetComponent<Animator>().Play("OpenGate Animation", 0);
    }

    IEnumerator WaitForXSeconds(float time)
    {
        // Wait for 4 seconds
        yield return new WaitForSeconds(time);
        Gates.transform.Find("Gate Camera").gameObject.SetActive(false);
    }

    private IEnumerator ShowNextPort(GameObject Port)
    {
        Port.transform.position = new Vector3(Port.transform.position.x, -50, Port.transform.position.z);
        Vector3 Target = new Vector3(Port.transform.position.x, 5, Port.transform.position.z);
        while (Vector3.Distance(Target, Port.transform.position) > 1f)
        {
            // Calculate the new position using Lerp
            Vector3 newPosition = Vector3.Lerp(Port.transform.position, Target, 4f * Time.deltaTime);
            Port.transform.position = newPosition;
            yield return null;
        }

        Target = new Vector3(Port.transform.position.x, 0, Port.transform.position.z);
        while (Vector3.Distance(Target, Port.transform.position) > 0.001f)
        {
            // Calculate the new position using Lerp
            Vector3 newPosition = Vector3.Lerp(Port.transform.position, Target, 5f * Time.deltaTime);
            Port.transform.position = newPosition;
            yield return null;
        }
    }
    
    public ShipCarrier[] GetShipsInPort()
    {
        return Ships.Select(ship => ship.GetComponent<ShipCarrier>()).ToArray();
    }

    public Conveyor GetConveyorInPort()
    {
        return ConveyorTable.GetComponent<Conveyor>();
    }

    public GameObject GetPlayer()
    {
        return Player;
    }
}