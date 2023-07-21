using System;
using UnityEngine;

public class Pier: Carrier
{
    private ITransferBoxes currentTransferPartner = null;
    
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
}
