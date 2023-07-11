using System.Collections.Generic;

[System.Serializable]
public class Carrier
{
    public string uID;
    public CarriersTypes carrierType;
    public int numOfMaxSlots;
    public List<string> boxesNames;

    
    
    public Carrier(string uID, CarriersTypes carrierType, int numOfMaxSlots, List<string> boxesNames)
    {
        this.uID = this.uID;
        this.carrierType = this.carrierType;
        this.numOfMaxSlots = this.numOfMaxSlots;
        this.boxesNames = boxesNames;
    }

    public class SimpleCarrier
    {
        public string uID;
        public CarriersTypes carrierType;
        public int numOfMaxSlots;
        public List<string> boxesNames;
    }
}
