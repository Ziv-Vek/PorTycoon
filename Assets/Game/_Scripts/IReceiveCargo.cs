using UnityEngine;

public interface IReceiveCargo
{
    public bool CanReceiverAcceptCargo { get;}
    public void ReceiveCargo(GameObject cargo);
}