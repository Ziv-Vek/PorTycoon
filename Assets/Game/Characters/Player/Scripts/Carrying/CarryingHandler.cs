using System;
using UnityEngine;

public class CarryingHandler : MonoBehaviour, IGiveCargo, IReceiveCargo, ITransferCargo
{
    // stats config:
    [SerializeField] private int maxCargoCapacity = 1;
    [SerializeField] private Transform[] cargoPlaces;
    public int CurrentNumOfCargoHolding { get; private set; } = 0;
    [SerializeField] private CarriersTypes carrierType = CarriersTypes.player;
    public GameObject[] boxes;

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

    public bool CanReceiverAcceptCargo => (cargoPlaces.Length - CurrentNumOfCargoHolding > 0);
    
    
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
        Debug.Log("called CheckTryTransferCargo()");
        return true;
    }

    public void ReceiveCargo(GameObject cargo)
    {
        int index = Array.FindIndex(boxes, i => i == null);
        boxes[index] = cargo;
        cargo.transform.SetParent(cargoPlaces[index]);
        cargo.transform.localPosition = Vector3.zero;
    }

    public GameObject GiveCargo()
    {
        int index = Array.FindLastIndex(boxes, obj => obj != null);

        GameObject box = boxes[index];
        boxes[index] = null;
        return box;
    }
}