using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortLoader : MonoBehaviour
{
    public GameObject[] Ships;
    public GameObject[] ShipsBuyers;
    public GameObject[] HandyMan;
    public GameObject[] HandyManBuyers;
    public GameObject ForkLift;
    private void Start()
    {
        for (int i = 0; i < GameManager.Instance.ShipNumber - 1; i++)
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
    }
}
