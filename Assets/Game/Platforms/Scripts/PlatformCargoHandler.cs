using System;
using UnityEngine;

public class PlatformCargoHandler: MonoBehaviour, IReceiveCargo, IGiveCargo
{
    [SerializeField] private Transform[] cargoPlaces;
    public int CurrentNumOfCargoHolding { get; private set; } = 0;

    public bool CanReceiverAcceptCargo => (cargoPlaces.Length - CurrentNumOfCargoHolding > 0);

    public void ReceiveCargo(GameObject cargo)
    {
        cargo.transform.SetParent(cargoPlaces[CurrentNumOfCargoHolding], false);
        CurrentNumOfCargoHolding++;
    }

    public GameObject GiveCargo()
    {
        if (CurrentNumOfCargoHolding - 1 <= 0)
            throw new Exception($"{gameObject.name} trying to give cargo, but has ${CurrentNumOfCargoHolding} cargo.");

        return cargoPlaces[--CurrentNumOfCargoHolding].GetChild(0).gameObject;
    }
    
}
