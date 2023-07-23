using System;
using UnityEngine;

public class TableActionZone : MonoBehaviour
{
    public ScratchBoard scratchBoard; // Drag the Canvas GameObject with the script attached here in the inspector

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is tagged as "Player"
        if (other.CompareTag("Player") && !scratchBoard.scratchBoard.activeSelf)
        {
            // Call the openCanvas function from the canvas script
            scratchBoard.Open();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the object that stays in the trigger is tagged as "Player"
        if (other.CompareTag("Player") && !scratchBoard.scratchBoard.activeSelf)
        {
            // Continuously call the openCanvas function from the canvas script
            scratchBoard.Open();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object that entered the trigger is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Call the openCanvas function from the canvas script
            scratchBoard.Close();
        }
    }
}