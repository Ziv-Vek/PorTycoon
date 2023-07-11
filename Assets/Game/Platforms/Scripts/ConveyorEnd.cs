using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorEnd : MonoBehaviour
{
    [SerializeField] private float delayBeforeTransferAttempts = 1f;
    
    public CarryingHandler table;
    public CargoDocking docking;
    
    public event Action OnConveyorEndFull;
    public event Action OnConveyorEndEmpty;
    
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("converyor end trigger");
        if (!other.CompareTag("Box")) return;

        OnConveyorEndFull?.Invoke();
        
        StartCoroutine(ProcessTransfer(other.gameObject));
    }

    IEnumerator ProcessTransfer(GameObject box)
    {
        while (!table.CheckCanReceiveCargo())
        {
            yield return new WaitForSeconds(delayBeforeTransferAttempts);
        }
        
        table.ReceiveCargo(box);
        docking.GiveCargo();
        
        OnConveyorEndEmpty?.Invoke();

        yield return null;
    }
}
