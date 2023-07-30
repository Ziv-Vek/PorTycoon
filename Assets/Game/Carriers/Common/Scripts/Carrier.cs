using System;
using UnityEngine;

public class Carrier: MonoBehaviour, ITransferBoxes
{
    // config:
    public int maxBoxesCapacity;
    public CarriersTypes carrierType;
    public Transform[] boxesPlaces;

    // cached ref:
    [HideInInspector] public GameObject[] boxes;
    
    // stats:
    public bool IsAttemptingToGiveCargo { get; set; }
    
    public virtual void Awake()
    {
        boxes = new GameObject[maxBoxesCapacity];
    }

    private void Start()
    {
        if (boxesPlaces.Length != maxBoxesCapacity)
        {
            throw new Exception($"{gameObject.name}: Number of boxesPlaces is different from maximum boxes allowed");
        }
    }

    public virtual GameObject GiveBox()
    {
        int index = Array.FindLastIndex(boxes, i => i != null);

        GameObject box = boxes[index];
        boxes[index] = null;
        
        return box;
    }

    public virtual void ReceiveBox(GameObject cargo)
    {
        int index = Array.FindIndex(boxes, i => i == null);
        boxes[index] = cargo;
        cargo.transform.SetParent(boxesPlaces[index]);
        cargo.transform.localPosition = Vector3.zero;
        cargo.transform.localRotation = gameObject.transform.rotation;
    }

    public CarriersTypes GetCarrierType()
    {
        return carrierType;
    }

    

    public bool CheckCanReceiveBoxes()
    {
        if (boxes == null) return false;
        
        foreach (var box in boxes)
        {
            if (box == null) return true;
        }

        return false;
    }

    public bool CheckCanGiveBoxes()
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
    public void AddBox(GameObject box)
    {
        GameObject[] ArrayBoxes = new GameObject[boxes.Length + 1];
        for (int i = 0; i < boxes.Length; i++)
        {
            ArrayBoxes[i] = boxes[i];
        }
      //  ArrayBoxes[ArrayBoxes.Length - 1] = box;
        boxes = ArrayBoxes;
    }
}
