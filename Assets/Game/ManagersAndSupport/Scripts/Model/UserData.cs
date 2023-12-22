using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

[Serializable]
public class UserData
{
    public Dictionary<string, Item> unlockedItems;
    public bool GoneThroughTutorial;
    public bool Sound = true;
    public bool Music = true;
    public bool Vibration = true;
    public int money;
    public int stars;
    public int experience = 1;
    public int playerSpeedLevel = 1;
    public int playerBoxPlacesLevel = 1;
    public Dictionary<string, LevelData> LevelsData;
    public int currentLevel = 1;

    public UserData()
    {
        money = ConfigManager.Instance.Config.defaultValues.money;
    }
}

public class LevelData
{
    public int shipSpeedLevel = 1;
    public int quantityLevel = 1;
    public int qualityLevel = 1;
    public int convayorSpeedLevel = 1;
    public int scanningSpeedLevel = 1;
    public int tableStackLevel = 1;
    public int openBoxTimeNpc = 1;
    public int awarenessTimeNpc = 1;
    public int forklifSpeedLevel = 1;
    public int forkliftBoxQuantityLevel = 1;
    public int forkliftFuelTankLevel = 1;
    public int scratchSizeScaleLevel = 1;
    public bool ForkliftIsEnabled = false;
    public int HandyManNumber = 0;
    public int ShipNumber = 0;
}