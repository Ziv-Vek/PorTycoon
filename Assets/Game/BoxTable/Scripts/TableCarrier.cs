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

    public void SetPlayer(IBoxOpener playerOpener)
    {
        this._player = playerOpener;
    }

    public void RemovePlayer()
    {
        this._player = null;
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
        else if (_boxOpeners.Count > 0)
        {
            return _boxOpeners.Dequeue();
        }

        return null;
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
    public void addBoxPlace()
    {
        Transform[] ArrayPlaces = new Transform[boxesPlaces.Length + 1];
        for (int i = 0; i < boxesPlaces.Length; i++)
        {
            ArrayPlaces[i] = boxesPlaces[i];
        }

        GameObject ParentObject = transform.parent.transform.GetChild(4).gameObject;
        Debug.Log(ParentObject.name); 
        Vector3 place;
        place = ParentObject.transform.GetChild(ParentObject.transform.childCount - 1).transform.position;
        place = new Vector3(place.x, place.y, place.z + 3);

        GameObject newPlace = Instantiate(ParentObject.transform.GetChild(ParentObject.transform.childCount - 1).gameObject, place, Quaternion.identity);
        newPlace.transform.parent = ParentObject.transform;

        newPlace.name = "CargoPlace (" + (ParentObject.transform.childCount - 1) + ")";
        newPlace.transform.localScale = new Vector3(1, 1, 1f);
        ArrayPlaces[ArrayPlaces.Length - 1] = newPlace.transform;

        boxesPlaces = ArrayPlaces;
        try { Destroy(newPlace.transform.GetChild(0).gameObject); }
        catch { }
        maxBoxesCapacity++;
        AddBox();
        // GC.Collect();
    }
}