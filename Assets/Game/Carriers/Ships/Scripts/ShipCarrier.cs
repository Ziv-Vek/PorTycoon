using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCarrier: Carrier
{
    [SerializeField] Pier pier;
    private ITransferBoxes boxesReceiver;

    
    
    public override void Awake()
    {
        boxesReceiver = pier.GetComponent<ITransferBoxes>();
        if (boxesPlaces == null) throw new Exception("No IReceiveCargo component found.");
    }
    
    public void InstantiateCargo()
    {
        boxes = BoxesManager.Instance.GetBoxesByQuantity(maxBoxesCapacity);
        RenderBoxes();
    }
    
    public IEnumerator TransferBoxesToPier()
    {
        IsAttemptingToGiveCargo = true;
        
        yield return BoxesTransferHandler.Instance.CheckTransfer(boxesReceiver, this);
    }
    
    private void RenderBoxes()
    {
        for (int i = 0; i < maxBoxesCapacity; i++)
        {
            if (!boxesPlaces[i]) throw new Exception("no cargoPlace found in range for the number of boxes");

            boxes[i].transform.SetParent(boxesPlaces[i], false);
            boxes[i].GetComponent<MeshRenderer>().enabled = true;
        }
    }
    
    public override GameObject GiveBox()
    {
        int index = Array.FindLastIndex(boxes, i => i != null);

        GameObject box = boxes[index];
        boxes[index] = null;

        if (!CheckCanGiveBoxes())
        {
            IsAttemptingToGiveCargo = false;
        }
        
        return box;
    }
}
