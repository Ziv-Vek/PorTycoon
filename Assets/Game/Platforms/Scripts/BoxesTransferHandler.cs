using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BoxesTransferHandler : MonoBehaviour
{
    [SerializeField] private float delayBetweenCargoTransfers = 0.3f;
    public static BoxesTransferHandler Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public IEnumerator CheckTransfer(ITransferCargo stationaryCarrier, ITransferCargo mobileCarrier)
    {
        CarriersTypes stationaryCarrierType = stationaryCarrier.GetCarrierType();
        CarriersTypes mobileCarrierType = mobileCarrier.GetCarrierType();

        // Checks for transfer between two mobile characters
        if ((stationaryCarrierType == CarriersTypes.forklift || 
             stationaryCarrierType == CarriersTypes.carrierNPC ||
             stationaryCarrierType == CarriersTypes.player) &&
            (mobileCarrierType == CarriersTypes.forklift || 
             mobileCarrierType == CarriersTypes.carrierNPC ||
             mobileCarrierType == CarriersTypes.player))
        {
            yield return null;
        }

        if (stationaryCarrierType == CarriersTypes.shipDropDownArea)
        {
            if (mobileCarrierType == CarriersTypes.ship)
            {
                yield return StartCoroutine(ProcessTransfer(mobileCarrier, stationaryCarrier, mobileCarrier));
            }
            else
            {
                yield return StartCoroutine(ProcessTransfer(stationaryCarrier, mobileCarrier, stationaryCarrier));
                
            }
        }

        if (stationaryCarrierType == CarriersTypes.conveyor)
        {
            yield return StartCoroutine(ProcessTransfer(mobileCarrier, stationaryCarrier, stationaryCarrier));
        }

        if (stationaryCarrierType == CarriersTypes.table)
        {
            yield return StartCoroutine(ProcessTransfer(mobileCarrier, stationaryCarrier, stationaryCarrier));
        }
    }

    private IEnumerator ProcessTransfer(ITransferCargo giver, ITransferCargo receiver, ITransferCargo transferLead)
    {
        while (transferLead.CheckTryTransferCargo())
        {
            yield return new WaitForSeconds(delayBetweenCargoTransfers);

            if (receiver.CheckCanReceiveCargo() && giver.CheckCanGiveCargo())
            {
                TransferSingleCargo(giver, receiver);
            }
        }

        CompleteCargoTransfer();

        yield return null;
    }


    private void TransferSingleCargo(ITransferCargo giver, ITransferCargo receiver)
    {
        receiver.ReceiveCargo(giver.GiveCargo());
    }
    
    private void CompleteCargoTransfer()
    {
        Debug.Log("cargo transfer completed");
        //TODO: Do something
    }
    
}
