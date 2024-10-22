using UnityEngine;

public class ConveyorEnd : MonoBehaviour
{
    [SerializeField] private Conveyor conveyor;

    private void OnTriggerEnter(Collider other)
    {
        var box = other.gameObject.GetComponent<PortBox>();
        if (!other.CompareTag("Box") || !conveyor.HasBoxInstance(box)) return;

        StartCoroutine(conveyor.TransferBoxToTableHandler());
    }
}