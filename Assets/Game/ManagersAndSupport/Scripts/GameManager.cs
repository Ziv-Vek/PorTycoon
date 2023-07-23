using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
    }

    private void OnApplicationQuit()
    {
        StateManager.SaveData(CurrentLevel, money, experience, UnlockedItems);
    }
}