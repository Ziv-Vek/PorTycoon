using UnityEngine;

public class TableActionZone : MonoBehaviour
{
    public ScratchBoard scratchBoard; // Drag the Canvas GameObject with the script attached here in the inspector
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is tagged as "Player"
        //TODO: check if there is a box on the table
        if (other.CompareTag("Player"))
        {
            // Call the openCanvas function from the canvas script
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