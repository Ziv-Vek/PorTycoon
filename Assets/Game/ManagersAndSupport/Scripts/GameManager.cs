using System;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Managers
    private ConfigManager ConfigManager { get; set; }
    private StateManager StateManager { get; set; }


    public GameConfig GameConfig { get; set; }
    public LevelData CurrentLevel { get; set; }

    // player settings
    public int money;
    public int stars;
    public int experience;
    public int ShipSpeedLevel = 1;   
    public int QuantityLevel = 1;
    public int QualityLevel = 1;

    public int ConvayorSpeedLevel = 1;
    public int ScanningSpeedLevel = 1;
    public int TableStackLevel = 1;

    public int OpenBoxTime_NPC = 1;
    public int AwarenessTime_NPC = 1;

    public int PlayerSpeedLevel = 1;
    public int PlayerBoxPlacesLevel;

    public int ForkliftBoxQuantityLevel = 1;
    public int ForkliftFuelTankLevel = 1;


    public List<UnlockedItem> UnlockedItems { get; set; }

    //public List<BoxData> boxes = new List<BoxData>();
    //public List<BoxesCarrier> carriers;
    //[SerializeField] private Item[] items;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        StateManager = gameObject.AddComponent<StateManager>();

        UnlockedItems = new List<UnlockedItem>();
        //carriers = new List<BoxesCarrier>();

        LoadConfig();
        StateManager.LoadData(); // Load saved data when the GameManager starts
    }

    public void LoadConfig()
    {
        ConfigManager = new ConfigManager();
        GameConfig = ConfigManager.Config;
        CurrentLevel = ConfigManager.GetLevelConfig("1");
    }


    public void UnlockItem(Item item)
    {
        UnlockedItem unlockedItem = new UnlockedItem(item, DateTime.UtcNow);
        UnlockedItems.Add(unlockedItem);
        stars++;
        UIManager.Instance.UpdateStarsText(stars);
    }

    private void OnApplicationQuit()
    {
        // StateManager.SaveData(CurrentLevel, money, experience, UnlockedItems);
    }
}