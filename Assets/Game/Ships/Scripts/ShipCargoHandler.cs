using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ShipCargoHandler: MonoBehaviour, ISaveable, ITransferCargo
{
    [SerializeField] private CarriersTypes carrierType = CarriersTypes.ship;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform[] cargoPlaces;
    [SerializeField] CargoDocking cargoReceivingPlatform;
    private ITransferCargo cargoReceiver;
    public int numOfMaxSlots;
    public List<string> boxesNames;
    
    [SerializeField] private float timeDelayBetweenEachCargoDrop;
    [SerializeField] private int currentNumOfCargoHolding = 0;      // serialized for debugging
    [SerializeField] private int defaultCarryCapacity = 4;
    [SerializeField] private int maxCargoCapacity = 4;
    private string shipUID; 
    [SerializeField] private Carrier carrier = null;    // serialized for debugging
    [SerializeField] private List<GameObject> boxesCarrying = new List<GameObject>();
    public bool isTransferingCargo = false;

    #region EVENTS
    public event Action onCargoTransferCompleted;
    #endregion

    private void Awake()
    {
        cargoReceiver = cargoReceivingPlatform.GetComponent<ITransferCargo>();
        if (cargoPlaces == null) throw new Exception("No IReceiveCargo component found.");
    }

    private void Start()
    {
        shipUID = GetComponent<SaveableEntity>().GetUID();

        //InstantiateCargo();
    }

    // instantiate new cargo on the ship
    public void InstantiateCargo()
    {
        boxesCarrying = BoxesManager.Instance.GetBoxesByQuantity(maxCargoCapacity, out carrier.boxesNames);
        
        
        /*if (carrier == null)
        {
            carrier = new Carrier(shipUID, CarriersTypes.ship, defaultCarryCapacity, new List<string>());
        }

        List<GameObject> boxes = new List<GameObject>();
        
        
        
        
        if (carrier.boxesNames != null && carrier.boxesNames.Count > 0)
        {
            boxes = BoxesManager.Instance.GetBoxesByName(carrier.boxesNames);
        }
        else
        {
            List<string> newBoxesNames = new List<string>();
            
            //List<GameObject> newBoxes = new List<GameObject>();
            boxesCarrying = BoxesManager.Instance.GetBoxesByQuantity(maxCargoCapacity, out carrier.boxesNames);
            //carrier.boxesNames = newBoxesNames;
            
            //BoxesManager.BoxesStruct boxesStruct = BoxesManager.Instance.GetBoxesByQuantity(carrier.numOfMaxSlots);
            //boxes = boxesStruct.boxes;
            //carrier.boxesNames = boxesStruct.boxesNames;
            //boxes = newBoxes;
        }*/
        
        RenderBoxes();
        
        /*foreach (var cargoPlace in cargoPlaces)
        {
            
            
            if (cargoPlace.childCount == 0)
            {
                
                Instantiate(boxPrefab, cargoPlace, false);
                currentNumOfCargoHolding++;
            }
        }*/
    }

    public IEnumerator HandleCargoTransfer()
    {
        isTransferingCargo = true;
        yield return BoxesTransferHandler.Instance.CheckTransfer(cargoReceiver, this);
    }

    /*public IEnumerator HandleCargoTransfer(int cargoIndex)
    {
        isTransferingCargo = true;
        //BoxesTransferHandler.Instance.CheckTransfer();
        while (boxesCarrying.Count > 0)
        {
            yield return new WaitForSeconds(timeDelayBetweenEachCargoDrop);

            if (cargoReceiver.CheckCanReceiveCargo())
            {
                TransferSingleCargo(cargoIndex);

                currentNumOfCargoHolding--;
                cargoIndex++;
            }
        }
        CompleteCargoTransfer();
        yield return null;
        
    }*/
    
    private void RenderBoxes()
    {
        for (int i = 0; i < boxesCarrying.Count; i++)
        {
            if (!cargoPlaces[i]) throw new Exception("no cargoPlace found in range for the number of boxes");

            boxesCarrying[i].transform.SetParent(cargoPlaces[i], false);
            boxesCarrying[i].GetComponent<MeshRenderer>().enabled = true;
        }
    }
    
    private void TransferSingleCargo(int cargoPlacesIndex)
    {
        if (boxesCarrying.Count > 0)
        {
            cargoReceiver.ReceiveCargo(GiveCargo());
        }
        else
        {
            throw new Exception("Trying to give cargo but no cargo to give.");
        }

        if (boxesCarrying.Count == 0)
        {
            isTransferingCargo = false;
        }
    }

    private void CompleteCargoTransfer()
    {
        onCargoTransferCompleted?.Invoke();
    }
    
    /*[Serializable]
    struct ShipSaveData
    {
        public string uID;
        public CarriersTypes carrierType;
        public int numOfMaxSlots;
        public List<string> boxesNames; 
    }*/
    
    public class ShipSaveData
    {
        public string uID;
        public CarriersTypes carrierType;
        public int numOfMaxSlots;
        public List<string> boxesNames;
    }

    public object CaptureState()
    {
        /*if (carrier == null)
        {
            carrier = new Carrier(GetComponent<SaveableEntity>().GetUID(), CarriersTypes.garbage, defaultCarryCapacity,
                new List<string>());
        }*/

        //object obj = carrier.numOfMaxSlots;
        
        //Dictionary<string, object> data = new Dictionary<string, object>();
        
        /*data["type"] = Enum.ToObject(typeof(CarriersTypes), (int)carrier.carrierType);
        data["capacity"] = carrier.numOfMaxSlots.ToString();
        data["boxesNames"] = carrier.boxesNames;*/
        
        /*data.Add("type", Enum.ToObject(typeof(CarriersTypes), (int)carrier.carrierType));
        data.Add("capacity", carrier.numOfMaxSlots.ToString());
        data.Add("boxesNames", carrier.boxesNames);

        var testObj = new { ShipType = carrier.carrierType };*/
        
        //return carrier;
        return null;

        //return  

        /*Carrier.CarrierData carrierData = new Carrier.CarrierData();
        carrierData.uID = carrier.uID;
        carrierData.carrierType = carrier.carrierType;
        carrierData.numOfMaxSlots = carrier.numOfMaxSlots;
        carrierData.boxesNames = carrier.boxesNames;*/

        /*ShipSaveData data = new ShipSaveData();
        data.uID = GetComponent<SaveableEntity>().GetUID();
        data.carrierType = carrierType;
        data.numOfMaxSlots = numOfMaxSlots;
        data.boxesNames = boxesNames;*/

        // Carrier.SimpleCarrier simpleCarrier = new Carrier.SimpleCarrier();
        // simpleCarrier.uID = carrier.uID;
        // simpleCarrier.carrierType = carrier.carrierType;
        // simpleCarrier.numOfMaxSlots = carrier.numOfMaxSlots;
        // simpleCarrier.boxesNames = carrier.boxesNames;
        //
        // SimpleData simpleData = new SimpleData();
        // simpleData.uID = carrier.uID;
        // simpleData.carrierType = carrier.carrierType;
        // simpleData.numOfMaxSlots = carrier.numOfMaxSlots;
        // simpleData.boxesNames = carrier.boxesNames;

        //object boxedData = data;


        /*
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["id"] = carrier.uID;
        data["type"] = (int)carrier.carrierType;
        */

        //return boxedData;
    }

    public void RestoreState(object state)
    {
        //var fff = new { state };
        
        //Debug.Log(fff("ShipType"));
        
        //ShipSaveData data = (ShipSaveData)state;
        //print("ship state" + state);

        /*if (state is Dictionary<string, object>data)
        {
            Debug.Log("state is of the correct type");
        }
        else if (state == null)
        {
            Debug.Log("state is null");
        }
        else
        {
            Debug.Log("state is not the correct type");
        }*/
        //Dictionary<string, object> data = (Dictionary<string, object>)state;
        
        
        /*int typeInt = Convert.ToInt32(data["type"]);
        
        carrier = new Carrier(
            GetComponent<SaveableEntity>().GetUID(),
            (CarriersTypes)typeInt,
            Int32.Parse((string)(data["capacity"])),
            (List<string>)data["boxesNames"]);


        Debug.Log(carrier.uID);
        Debug.Log("type: " + carrier.carrierType);
        Debug.Log("capacity: " + carrier.numOfMaxSlots);
        Debug.Log("capacity: " + carrier.boxesNames);*/
        
        
        
        //data.uID = state.uID;

        //Carrier test1 = new Carrier(null, CarriersTypes.carrierOnTable, 0, null);

        //var test = test1.SimpleCarrier(state);

        // Carrier.SimpleCarrier data = new Carrier.SimpleCarrier();
        // data = Carrier.SimpleCarrier(data);
        //
        // data.uID = state.uID;
        //ShipCargoSaveData data = ShipCargoSaveData(state);

        //carrier = new Carrier(string(state.uID), CarriersTypes(state.carrierType), int(state.numOfMaxSlots), List<string>(state.boxesNames));
        //carrier = new Carrier((string)state.uID, CarriersTypes(state.carrierType), state.numOfMaxSlots, state.boxesNames);
    }

    public GameObject GiveCargo()
    {
        if (boxesCarrying.Count <= 0)
        {
            throw new Exception("Trying to give cargo but no cargo to give.");
        }

        int index = boxesCarrying.FindIndex(i => i != null);
        GameObject box = boxesCarrying[index];
        boxesCarrying.RemoveAt(index);

        if (boxesCarrying.Count == 0)
        {
            isTransferingCargo = false;
        }
        
        return box;
    }

    public void ReceiveCargo(GameObject cargo)
    {
        throw new NotImplementedException();
    }

    public bool CanReceiverAcceptCargo { get; }
    public CarriersTypes GetCarrierType()
    {
        return carrierType;
    }

    public bool CheckCanReceiveCargo()
    {
        return boxesCarrying.Count < maxCargoCapacity;
    }

    public bool CheckCanGiveCargo()
    {
        return boxesCarrying.Count > 0;
    }

    public bool CheckTryTransferCargo()
    {
        return isTransferingCargo;
    }
}