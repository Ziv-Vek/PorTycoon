using UnityEngine;


public class PortBox : MonoBehaviour, IItemContainer
{
    public bool CanBeOpened { get; set; } = true;
    public string Type { get; private set; }
    public bool isPurchasedBox = false;

    public void Awake()
    {
        Type = ConfigManager.Instance.GetBoxTypeRandomly(null);
    }
}