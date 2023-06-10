using System;
using UnityEngine;

public class CarryingHandler : MonoBehaviour, IGiveCargo, IReceiveCargo
{
    // stats config:
    [SerializeField] private int maxCargoAllowed = 1;
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
        return cargoPlaces[currentNumOfCargoHolding - 1].GetChild(0).gameObject;
    }

    public int GetNumOfCargoHolding()
    {
        return currentNumOfCargoHolding;
    }
}