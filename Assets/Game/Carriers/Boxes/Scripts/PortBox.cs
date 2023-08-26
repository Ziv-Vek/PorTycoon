using UnityEngine;


public class PortBox : MonoBehaviour, IItemContainer
{
    public bool CanBeOpened { get; set; } = true;
    public string Type { get; set; } = "wood";
}