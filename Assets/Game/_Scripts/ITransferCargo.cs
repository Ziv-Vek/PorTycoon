using System.Collections;
using UnityEngine;

public interface ITransferCargo
{
    public GameObject GiveCargo();
    public void ReceiveCargo(GameObject cargo);
    public bool CanReceiverAcceptCargo { get;}

    public CarriersTypes GetCarrierType();

    public bool CheckCanReceiveCargo();
    public bool CheckCanGiveCargo();
    public bool CheckTryTransferCargo();
}
