using System.Collections;
using UnityEngine;

public interface ITransferBoxes
{
    public GameObject GiveBox();
    public void ReceiveBox(GameObject box);
    public bool CheckCanReceiveBoxes();
    public bool CheckCanGiveBoxes();
    public CarriersTypes GetCarrierType();

     public bool IsAttemptingToGiveCargo { get; set; }
}
