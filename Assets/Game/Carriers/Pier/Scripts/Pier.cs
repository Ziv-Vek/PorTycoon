using System;
using UnityEngine;

public class Pier: Carrier
{
    private ITransferBoxes currentTransferPartner = null;
    
    public event Action<CarriersTypes> onBoxDrop;
    
    private void OnTriggerEnter(Collider other)
    {
        if (currentTransferPartner != null) return;
        
        if (other.TryGetComponent<ITransferBoxes>(out currentTransferPartner))
        {
            IsAttemptingToGiveCargo = true;
        
            StartCoroutine(BoxesTransferHandler.Instance.CheckTransfer(this, currentTransferPartner));    
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<ITransferBoxes>(out ITransferBoxes otherTransferer)
            && (otherTransferer != currentTransferPartner)) return;
        
        IsAttemptingToGiveCargo = false;
        currentTransferPartner = null;
    }
    
    public override void ReceiveBox(GameObject cargo)
    {
        int index = Array.FindIndex(boxes, i => i == null);
        boxes[index] = cargo;
        cargo.transform.SetParent(boxesPlaces[index]);
        cargo.transform.localPosition = Vector3.zero;
        cargo.transform.localRotation = gameObject.transform.rotation;
        
        onBoxDrop?.Invoke(CarriersTypes.pier);
    }
    
    
}
