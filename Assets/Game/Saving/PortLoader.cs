using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PortLoader : MonoBehaviour
{
    GameConfig gameConfig;

    public GameObject[] Ships;
    public GameObject[] ShipsBuyers;
    public GameObject[] HandyMan;
    public GameObject[] HandyManBuyers;
    public GameObject ForkLift;
    public GameObject ForkLiftBuyer;
    public GameObject ConveyorTable;
    public GameObject BoxTable;
    private void Start()
    {
        gameConfig = ConfigManager.Instance.Config;
        //ships values
        for (int i = 0; i < GameManager.Instance.ShipNumber; i++)
        {
            ShipsBuyers[i].GetComponent<Buyer>().ActiveProduct();
        }
        for (int i = 0; i < GameManager.Instance.quantityLevel - 1; i++)
        {
            foreach (GameObject ship in Ships)
            {
                ship.GetComponent<ShipCarrier>().addBoxPlace();
            }
        }
        for (int i = 0; i < GameManager.Instance.shipSpeedLevel; i++)
        {
            foreach (GameObject ship in Ships)
            {
                ship.GetComponent<ShipController>().setSpeed(gameConfig.levels[0].upgrades["ship_speed"].levels[GameManager.Instance.shipSpeedLevel - 1]);
            }
        }
        //forklift values
        if (GameManager.Instance.ForkliftIsEnabled)
        {
            ForkLiftBuyer.GetComponent<Buyer>().ActiveProduct();
        }
        GameObject.Find("Forklift").GetComponent<NavMeshAgent>().speed = gameConfig.levels[0].upgrades["forklift_speed"].levels[GameManager.Instance.forklifSpeedLevel - 1];
        for (int i = 0; i < GameManager.Instance.forkliftBoxQuantityLevel - 1; i++)
        {
           ForkLift.GetComponent<ForkliftCarrier>().addBoxPlace();
        }
        FindObjectOfType<ForkliftMover>().FuelUpgrade((int)gameConfig.levels[0].upgrades["forklift_fuel_tank"].levels[GameManager.Instance.forkliftFuelTankLevel - 1]);
        //conveyorTable values
        ConveyorTable.GetComponent<Conveyor>().beltSpeed = gameConfig.levels[0].upgrades["conveyor_speed"].levels[GameManager.Instance.convayorSpeedLevel - 1];
        ConveyorTable.transform.FindChild("Scanner").GetComponent<Scanner>().scanningDuration = gameConfig.levels[0].upgrades["conveyor_scanning_speed"].levels[GameManager.Instance.scanningSpeedLevel - 1];
        for (int i = 0; i < GameManager.Instance.tableStackLevel - 1; i++)
            BoxTable.transform.FindChild("Table").GetComponent<TableCarrier>().addBoxPlace();

    }
}
