using System;
using System.Linq;
using UnityEngine;

public class Carrier : MonoBehaviour, ITransferBoxes
{
    // config:
    public int maxBoxesCapacity;
    public CarriersTypes carrierType;
    public Transform[] boxesPlaces;

    // cached ref:
    [HideInInspector] public PortBox[] boxes;

    // stats:
    public bool IsAttemptingToGiveCargo { get; set; }

    public virtual void Awake()
    {
        boxes = new PortBox[maxBoxesCapacity];
    }

    private void Start()
    {
        if (boxesPlaces.Length != maxBoxesCapacity)
        {
            throw new Exception($"{gameObject.name}: Number of boxesPlaces is different from maximum boxes allowed");
        }
    }

    public virtual PortBox GiveBox()
    {
        int index = Array.FindLastIndex(boxes, i => i != null);

        PortBox box = boxes[index];
        boxes[index] = null;

        return box;
    }

    public virtual void ReceiveBox(PortBox box)
    {
        int index = Array.FindIndex(boxes, i => i == null);
        boxes[index] = box;
        box.transform.SetParent(boxesPlaces[index]);
        box.transform.localPosition = Vector3.zero;
        box.transform.localRotation = gameObject.transform.rotation;
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

    public void AddBox(PortBox box)
    {
        PortBox[] ArrayBoxes = new PortBox[boxes.Length + 1];
        for (int i = 0; i < boxes.Length; i++)
        {
            ArrayBoxes[i] = boxes[i];
        }

        //  ArrayBoxes[ArrayBoxes.Length - 1] = box;
        boxes = ArrayBoxes;
    }

    public void RemoveBox(PortBox box)
    {
        // Find the box in the array and set its spot to null
        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i] == box)
            {
                Destroy(box.transform.gameObject);
                boxes[i] = null;
                break;
            }
        }
    }

    protected PortBox GetAvailableBox()
    {
        return Array.Find(boxes, box => box != null && box.CanBeOpened);
    }
}