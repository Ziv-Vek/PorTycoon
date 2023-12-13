using UnityEngine;


public class PortBox : MonoBehaviour, IItemContainer
{
    public bool CanBeOpened { get; set; } = true;
    public string Type { get; private set; }
    public int level; //port level
    public bool isPurchasedBox = false;

    public void Awake()
    {
        Type = ConfigManager.Instance.GetBoxTypeRandomly(null);
    }

    public void ActivateCollider() { 
        GetComponent<BoxCollider>().enabled = true;
       }

    public void DeactivateCollider()
    {
        GetComponent<BoxCollider>().enabled = false;
       }


}