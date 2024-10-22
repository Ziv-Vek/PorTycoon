using System;
using System.Collections.Generic;
using System.Linq;
//using DigitalOpus.MB.Core;
using UnityEngine;
using System.Threading;


public class TableCarrier : Carrier
{
    // Priority queue of Box Openers Available
    private readonly Queue<IBoxOpener> _boxOpeners = new();
    private IBoxOpener _player;
    [SerializeField] GameObject[] LockedPlaces;

    public void SetPlayer(IBoxOpener playerOpener)
    {
        _player = playerOpener;
    }

    public void RemovePlayer()
    {
        _player = null;
    }

    public void AddBoxOpener(IBoxOpener boxOpener)
    {
        if (_boxOpeners.Any(opener => opener == boxOpener)) return;
        _boxOpeners.Enqueue(boxOpener);
    }

    private IBoxOpener GetBoxOpener()
    {
        if (_player != null)
        {
            return _player;
        }

        if (_boxOpeners.Count > 0)
        {
            return _boxOpeners.Dequeue();
        }

        return null;
    }

    public override void ReceiveBox(PortBox box)
    {
        int index = Array.FindIndex(boxes, i => i == null);
        boxesPlaces[index].Find("Place Visual").gameObject.SetActive(false);
        boxes[index] = box;
        box.transform.SetParent(boxesPlaces[index]);
        box.transform.localPosition = Vector3.zero;
        // box.transform.localRotation = boxesPlaces[index].transform.rotation;
    }

    public override void RemoveBox(PortBox box)
    {
        // Find the box in the array and set its spot to null
        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i] == box)
            {
                Destroy(box.transform.gameObject);
                if (boxesPlaces[i].GetChild(1) != null)
                    Destroy(boxesPlaces[i].GetChild(1).gameObject);
                boxes[i] = null;
                boxesPlaces[i].Find("Place Visual").gameObject.SetActive(true);
                break;
            }
        }
    }

    private void GiveBoxToOpener(PortBox box)
    {
        if (_boxOpeners.Count == 0 && _player == null) return;

        // Open the box
        IBoxOpener boxOpener = GetBoxOpener();
        bool isOpening = boxOpener.OpenBox(box);
        if (!isOpening) _boxOpeners.Enqueue(boxOpener);
    }

    public void Update()
    {
        PortBox box = GetAvailableBox();
        if (box != null)
        {
            GiveBoxToOpener(box);
        }
    }

    public void AddBoxPlace()
    {
        Transform[] arrayPlaces = new Transform[boxesPlaces.Length + 1];
        for (int i = 0; i < boxesPlaces.Length; i++)
        {
            arrayPlaces[i] = boxesPlaces[i];
        }

        var place = CargoPlacesHolder.transform.GetChild(CargoPlacesHolder.transform.childCount - 1).transform.position;
        place = new Vector3(place.x, place.y, place.z + 5.5f);

        GameObject newPlace =
            Instantiate(CargoPlacesHolder.transform.GetChild(CargoPlacesHolder.transform.childCount - 1).gameObject,
                place, CargoPlacesHolder.transform.GetChild(CargoPlacesHolder.transform.childCount - 1).rotation,
                CargoPlacesHolder.transform);

        newPlace.name = "CargoPlace (" + (CargoPlacesHolder.transform.childCount - 1) + ")";
        newPlace.transform.localScale = new Vector3(1, 1, 1f);
        arrayPlaces[arrayPlaces.Length - 1] = newPlace.transform;

        boxesPlaces = arrayPlaces;
        Destroy(LockedPlaces[boxesPlaces.Length - 2]);
        maxBoxesCapacity++;
        AddBox();
    }
}