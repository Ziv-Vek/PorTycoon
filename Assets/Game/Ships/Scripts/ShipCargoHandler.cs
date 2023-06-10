using System;
using System.Collections;
using UnityEngine;

public class ShipCargoHandler: MonoBehaviour
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform[] cargoPlaces;
    [SerializeField] GameObject cargoReceivingPlatform;
    private IReceiveCargo cargoReceiver;
    
    [SerializeField] private float timeDelayBetweenEachCargoDrop;
    [SerializeField] private int currentNumOfCargoHolding = 0;      // serialized for debugging

    #region EVENTS
    public event Action onCargoTransferCompleted;
    #endregion

    private void Awake()
    {
        cargoReceiver = cargoReceivingPlatform.GetComponent<IReceiveCargo>();
        if (cargoPlaces == null) throw new Exception("No IReceiveCargo component found.");
    }

    // instantiate new cargo on the ship
    public void InstantiateCargo()
    {
        foreach (var cargoPlace in cargoPlaces)
        {
            if (cargoPlace.childCount == 0)
            {
                Instantiate(boxPrefab, cargoPlace, false);
                currentNumOfCargoHolding++;
            }
        }
    }

    public IEnumerator HandleCargoTransfer(int cargoIndex)
    {
        while (currentNumOfCargoHolding > 0)
        {
            yield return new WaitForSeconds(timeDelayBetweenEachCargoDrop);

            if (cargoReceiver.CanReceiverAcceptCargo)
            {
                TransferSingleCargo(cargoIndex);

                currentNumOfCargoHolding--;
                cargoIndex++;
            }
        }
        CompleteCargoTransfer();
        yield return null;
    }

    private void TransferSingleCargo(int cargoPlacesIndex)
    {
        cargoReceiver.ReceiveCargo(cargoPlaces[cargoPlacesIndex].GetChild(0).gameObject);
    }

    private void CompleteCargoTransfer()
    {
        onCargoTransferCompleted?.Invoke();
    }
}