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
    }
}
