using System;
using System.Collections.Generic;
using UnityEngine;

public class CarriersManager : MonoBehaviour, ISaveable
{
    private const int BaseSlotNumForShip = 4;
    
    public static CarriersManager Instance;
    
    private Dictionary<string, CarrierData> carriers;
   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        } 
        else
        {
            Destroy(this);
        }
    }

    public void SetCarrierData(string boxesGetterID, CarriersTypes carrierType)
    {
        
    }

    /*public void UpdateCarrierBoxes(string carrierID, List<GameObject> boxes)
    {
        if (carriers != null && carriers.ContainsKey(carrierID))
        {
            CarrierData newData = new CarrierData();
            newData.uID = carrierID;
            newData.carrierType = CarriersTypes.ship;
            newData.numOfMaxSlots = BaseSlotNumForShip;
            newData.boxes = boxes;

            carriers[carrierID] = newData;
        }
        else
        {
            carriers = new Dictionary<string, CarrierData>();

            CarrierData shipData = new CarrierData();
            shipData.uID = carrierID;
            shipData.carrierType = CarriersTypes.ship;
            shipData.numOfMaxSlots = BaseSlotNumForShip;
            shipData.boxes = boxes;
            
            carriers.Add(carrierID, shipData);
        }
    }*/

    public object CaptureState()
    {
        return carriers;
    }

    public void RestoreState(object state)
    {
        /*carriers = (Dictionary<string, CarrierData>)state;
        Debug.Log("restoring carrierManager" + state);*/
    }
}
