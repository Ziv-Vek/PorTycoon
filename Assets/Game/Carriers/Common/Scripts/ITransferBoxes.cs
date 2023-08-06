using System.Collections;
using UnityEngine;

public interface ITransferBoxes
{
    public PortBox GiveBox();
    public void ReceiveBox(PortBox box);
    public bool CheckCanReceiveBoxes();
    public bool CheckCanGiveBoxes();
    public CarriersTypes GetCarrierType();

     public bool IsAttemptingToGiveCargo { get; set; }
}
