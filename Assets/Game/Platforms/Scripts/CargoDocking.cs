using System;
using UnityEngine;

public class CargoDocking: MonoBehaviour, ITransferCargo, ICallTransferEvents
{
    [SerializeField] private Transform[] cargoPlaces;
    [SerializeField] int maxCargoCapacity = 4;
    [SerializeField] private CarriersTypes carrierType = CarriersTypes.shipDropDownArea;
    
    public GameObject[] boxes;
    
    private bool isCharacterOnPlatform = false;

    #region Events
    public event Action<GameObject> OnSingleTransferComplete;
    public event Action OnSingleTransferStart;
    public event Action OnTransfersComplete;
    #endregion
    
    private void Awake()
    {
        boxes = new GameObject[maxCargoCapacity];
    }

    private void Start()
    {
        if (cargoPlaces.Length != maxCargoCapacity)
        {
            throw new Exception("Number of cargo places is different from maximum cargo allowed");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isCharacterOnPlatform = true;

        StartCoroutine(BoxesTransferHandler.Instance.CheckTransfer(this, other.GetComponent<ITransferCargo>()));
    }

    private void OnTriggerExit(Collider other)
    {
        isCharacterOnPlatform = false;
    }

    public GameObject GiveCargo()
    {
        int index = Array.FindLastIndex(boxes, i => i != null);

        GameObject box = boxes[index];
        boxes[index] = null;
        
        return box;
    }

    public void ReceiveCargo(GameObject cargo)
    {
        int index = Array.FindIndex(boxes, i => i == null);
        boxes[index] = cargo;
        cargo.transform.SetParent(cargoPlaces[index]);
        cargo.transform.localPosition = Vector3.zero;
        
        OnSingleTransferComplete?.Invoke(cargo);
    }

    public bool CanReceiverAcceptCargo { get; }
    
    public CarriersTypes GetCarrierType()
    {
        return carrierType;
    }

    public bool CheckCanReceiveCargo()
    {
        if (boxes == null) return false;
        
        foreach (var box in boxes)
        {
            if (box == null) return true;
        }

        return false;
    }

    public bool CheckCanGiveCargo()
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i] != null) break;
            if (i == (boxes.Length - 1) && boxes[i] == null)
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckTryTransferCargo()
    {
        return isCharacterOnPlatform;
    }


}
