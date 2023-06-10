using System;
using UnityEngine;

public class PlatformCargoHandler: MonoBehaviour, IReceiveCargo, IGiveCargo
{
    [SerializeField] private Transform[] cargoPlaces;
    private int currentNumOfCargoHolding = 0;

    public bool CanReceiverAcceptCargo => (cargoPlaces.Length - currentNumOfCargoHolding > 0);

    public void ReceiveCargo(GameObject cargo)
    {
        cargo.transform.SetParent(cargoPlaces[currentNumOfCargoHolding], false);
        currentNumOfCargoHolding++;
    }

    public GameObject GiveCargo()
    {
        return cargoPlaces[--currentNumOfCargoHolding].GetChild(0).gameObject;
    }

    public int GetNumOfCargoHolding()
    {
        return currentNumOfCargoHolding;
    }
}
