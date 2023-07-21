using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorEnd : MonoBehaviour
{
    [SerializeField] private Conveyor conveyor;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Box") || !conveyor.HasBoxInstance(other.gameObject)) return;

        conveyor.ProcessTransferToTable(other.gameObject);
    }
}
