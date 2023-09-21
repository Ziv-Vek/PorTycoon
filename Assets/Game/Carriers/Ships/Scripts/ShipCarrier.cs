using System;
using System.Collections;
using UnityEngine;

public class ShipCarrier: Carrier
{
    [SerializeField] Pier pier;
    private ITransferBoxes boxesReceiver;
    [SerializeField] MoneyPile moneyPile;
    
    
    public override void Awake()
    {
        boxesReceiver = pier.GetComponent<ITransferBoxes>();
        if (boxesPlaces == null) throw new Exception("No IReceiveCargo component found.");
    }
    
    public void InstantiateCargo()
    {
        boxes = BoxesManager.Instance.GetBoxesByQuantity(maxBoxesCapacity);
        RenderBoxes();
    }
    
    public IEnumerator TransferBoxesToPier()
    {
        IsAttemptingToGiveCargo = true;
        
        yield return BoxesTransferHandler.Instance.CheckTransfer(boxesReceiver, this);
    }
    
    private void RenderBoxes()
    {
        for (int i = 0; i < maxBoxesCapacity; i++)
        {
            if (!boxesPlaces[i]) throw new Exception("no cargoPlace found in range for the number of boxes");

            boxes[i].transform.SetParent(boxesPlaces[i], false);
            boxes[i].transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }
    
    public override PortBox GiveBox()
    {
        int index = Array.FindLastIndex(boxes, i => i != null);

        PortBox box = boxes[index];
        boxes[index] = null;
        Bank.Instance.AddMoneyToPile(moneyPile,"Cargo");
        if (!CheckCanGiveBoxes())
        {
            IsAttemptingToGiveCargo = false;
        }
        
        return box;
    }
    public void addBoxPlace() 
    {
        Transform[] ArrayPlaces = new Transform[boxesPlaces.Length + 1];
        for (int i = 0; i < boxesPlaces.Length; i++)
        {
            ArrayPlaces[i] = boxesPlaces[i];
        }
        Vector3 place;
        //when the port loading...
        if (new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().DeclaringType.Name == "PortLoader")
        {
            if(CargoPlacesHolder.childCount == 1)
            {
               place = CargoPlacesHolder.GetChild(CargoPlacesHolder.childCount - 1).position;
               place = new Vector3(place.x + 6.3f, place.y, place.z);
            }
            else
            {
                place = CargoPlacesHolder.GetChild(CargoPlacesHolder.childCount - 2).position;
                place = new Vector3(place.x, place.y, place.z - 6.5f);
            }
        }
        //when buying the upgrade
        else
        {
            if (GameManager.Instance.quantityLevel == 1)
            {
                place = CargoPlacesHolder.GetChild(CargoPlacesHolder.childCount - 1).position;
                place = new Vector3(place.x + 6.3f, place.y, place.z);
            }
            else
            {
                place = CargoPlacesHolder.GetChild(CargoPlacesHolder.childCount - 2).position;
                place = new Vector3(place.x, place.y, place.z - 6.5f);
            }
        }
        GameObject newPlace = Instantiate(CargoPlacesHolder.GetChild(CargoPlacesHolder.childCount - 1).gameObject, place, Quaternion.identity);
        newPlace.transform.parent = CargoPlacesHolder;

        newPlace.name = "CargoPlace (" + (CargoPlacesHolder.childCount - 1) + ")";
        newPlace.transform.localScale = new Vector3(1,1,1f);
        ArrayPlaces[ArrayPlaces.Length - 1] = newPlace.transform;

        boxesPlaces = ArrayPlaces;
        try { Destroy(newPlace.transform.GetChild(0).gameObject); }
        catch{ }
        maxBoxesCapacity++;
        AddBox();
    }
}
