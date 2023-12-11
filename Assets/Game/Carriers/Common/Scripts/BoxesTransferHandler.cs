using System.Collections;
using UnityEngine;

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

    public IEnumerator CheckTransfer(ITransferBoxes stationaryCarrier, ITransferBoxes mobileCarrier)
    {
        CarriersTypes stationaryCarrierType = stationaryCarrier.GetCarrierType();
        CarriersTypes mobileCarrierType = mobileCarrier.GetCarrierType();

        // Checks for transfer between two mobile characters
        if ((stationaryCarrierType == CarriersTypes.Npc ||
             stationaryCarrierType == CarriersTypes.Player) &&
            (mobileCarrierType == CarriersTypes.Npc ||
             mobileCarrierType == CarriersTypes.Player))
        {
            yield return null;
        }

        if (stationaryCarrierType == CarriersTypes.Pier)
        {
            // ship tries to give to the pier
            if (mobileCarrierType == CarriersTypes.Ship)
            {
                yield return StartCoroutine(ProcessTransfer(mobileCarrier, stationaryCarrier, mobileCarrier));
            }
            // pier tries to give to the player/NPC
            else
            {
                yield return StartCoroutine(ProcessTransfer(stationaryCarrier, mobileCarrier, stationaryCarrier));
            }
        }

        if (stationaryCarrierType == CarriersTypes.Conveyor)
        {
            yield return StartCoroutine(ProcessTransfer(mobileCarrier, stationaryCarrier, stationaryCarrier));
        }

        if (stationaryCarrierType == CarriersTypes.Table)
        {
            yield return StartCoroutine(ProcessTransfer(mobileCarrier, stationaryCarrier, stationaryCarrier));
        }
    }

    private IEnumerator ProcessTransfer(ITransferBoxes giver, ITransferBoxes receiver, ITransferBoxes transferLead)
    {
        while (transferLead.IsAttemptingToGiveCargo)
        {
            yield return new WaitForSeconds(delayBetweenCargoTransfers);

            if (receiver.CheckCanReceiveBoxes() && giver.CheckCanGiveBoxes())
            {
                TransferSingleCargo(giver, receiver);
            }
        }

        yield return null;
    }

    private void TransferSingleCargo(ITransferBoxes giver, ITransferBoxes receiver)
    {
        receiver.ReceiveBox(giver.GiveBox());
    }
}