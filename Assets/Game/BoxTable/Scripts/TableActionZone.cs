using UnityEngine;

public class TableActionZone : MonoBehaviour
{
    [SerializeField] private TableCarrier tableCarrier;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is tagged as "Player"
        if (other.CompareTag("Player") &&
            !other.gameObject.GetComponent<PlayerCarrier>().scratchBoard.gameObject.activeSelf)
        {
            tableCarrier.SetPlayer(other.gameObject.GetComponent<PlayerCarrier>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object that entered the trigger is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            tableCarrier.RemovePlayer();
        }
    }
}