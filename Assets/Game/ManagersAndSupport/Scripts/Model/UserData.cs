using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public Dictionary<string, Item> unlockedItems;
    public int money;
    public int stars;
    public int experience;
    public int shipSpeedLevel;
    public int quantityLevel;
    public int qualityLevel;
    public int convayorSpeedLevel;
    public int scanningSpeedLevel;
    public int tableStackLevel;
    public int openBoxTimeNpc;
    public int awarenessTimeNpc;
    public int playerSpeedLevel;
    public int playerBoxPlacesLevel;
    public int forklifSpeedLevel;
    public int forkliftBoxQuantityLevel;
    public int forkliftFuelTankLevel;
    public int scratchSizeScaleLevel;
    public bool ForkliftIsEnabled;
    public int HandyManNumber;
    public int ShipNumber;
}