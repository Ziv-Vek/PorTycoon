using System;
using System.Collections;
using UnityEngine;

public class CargoTransferActionZone : MonoBehaviour
{
    [SerializeField] private PlatformCargoHandler connectedPlatformTransform;
    [SerializeField] private float timeDelayBetweenEachCargoDrop = 0.3f;
    
    [Tooltip("True if cargo will be transfered from a character to the Platform. False if in the other direction")]
    [SerializeField] private bool isPlatformReceivingCargo;

    private bool isCharacterOnPlatform = false;

    #region EVENTS
    public event Action onCargoTransferCompleted;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        isCharacterOnPlatform = true;
        
        if (isPlatformReceivingCargo)
        {
            // sets character as giver and platform as receiver
            var cargoGiver = other.gameObject.GetComponent<IGiveCargo>();
            if (cargoGiver == null) throw new Exception($"IGiveCargo missing at {other.gameObject.name}");
            
            var cargoReceiver = connectedPlatformTransform.GetComponent<IReceiveCargo>();
            if (cargoReceiver == null) throw new Exception($"IReceiveCargo missing at {connectedPlatformTransform.gameObject.name}");

            StartCargoTransfer(cargoGiver, cargoReceiver);
        }
        else
        {
            // sets platform as giver and character as receiver
            var cargoReceiver = other.gameObject.GetComponent<IReceiveCargo>();
            if (cargoReceiver == null) throw new Exception($"IReceiveCargo missing at {other.gameObject.name}");
            
            var cargoGiver = connectedPlatformTransform.GetComponent<IGiveCargo>();
            if (cargoGiver == null) throw new Exception($"IGiveCargo missing at {connectedPlatformTransform.gameObject.name}");
            
            StartCargoTransfer(cargoGiver, cargoReceiver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isCharacterOnPlatform = false;
    }

    private void StartCargoTransfer(IGiveCargo cargoGiver, IReceiveCargo cargoReceiver)
    {
        StartCoroutine(HandleCargoTransfer(cargoGiver, cargoReceiver));
    }
    
    private IEnumerator HandleCargoTransfer(IGiveCargo cargoGiver, IReceiveCargo cargoReceiver)
    {
        while (cargoReceiver.CanReceiverAcceptCargo && isCharacterOnPlatform)
        {
            yield return new WaitForSeconds(timeDelayBetweenEachCargoDrop);

            if (cargoGiver.GetNumOfCargoHolding() > 0)
            {
                TransferSingleCargo(cargoGiver, cargoReceiver);
            }
        }
        
        CompleteCargoTransfer();
        yield return null;
    }

    private void TransferSingleCargo(IGiveCargo cargoGiver, IReceiveCargo cargoReceiver)
    {
        cargoReceiver.ReceiveCargo(cargoGiver.GiveCargo());
    }

    private void CompleteCargoTransfer()
    {
        onCargoTransferCompleted?.Invoke();
    }
    
}
