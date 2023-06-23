using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Player progress and data variables
    public int currentLevel = 1;
    public int money;
    public int experience;
    public List<UnlockedItem> items;
    public List<BoxData> boxes = new List<BoxData>();
    //public List<BoxesCarrier> carriers;

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

        items = new List<UnlockedItem>();
        //carriers = new List<BoxesCarrier>();

        LoadData(); // Load saved data when the GameManager starts
    }

    public void SaveData()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevel, currentLevel);
        PlayerPrefs.SetInt(PlayerPrefsKeys.Money, money);
        PlayerPrefs.SetInt(PlayerPrefsKeys.Experience, experience);

        // Save unlocked items
        string unlockedItemsJson = JsonConvert.SerializeObject(items);
        PlayerPrefs.SetString(PlayerPrefsKeys.UnlockedItems, unlockedItemsJson);

        PlayerPrefs.Save();
    }
    
    public void LoadData()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.CurrentLevel))
        {
            currentLevel = PlayerPrefs.GetInt(PlayerPrefsKeys.CurrentLevel);
        }

        if (PlayerPrefs.HasKey(PlayerPrefsKeys.Money))
        {
            money = PlayerPrefs.GetInt(PlayerPrefsKeys.Money);
        }

        if (PlayerPrefs.HasKey(PlayerPrefsKeys.Experience))
        {
            experience = PlayerPrefs.GetInt(PlayerPrefsKeys.Experience);
        }
        // Load unlocked items

        if (PlayerPrefs.HasKey(PlayerPrefsKeys.UnlockedItems))
        {
            string unlockedItemsJson = PlayerPrefs.GetString(PlayerPrefsKeys.UnlockedItems);
            items = JsonConvert.DeserializeObject<List<UnlockedItem>>(unlockedItemsJson);
        }

        // Load boxes
        if (PlayerPrefs.HasKey(PlayerPrefsKeys.Boxes))
        {
            string boxesJson = PlayerPrefs.GetString(PlayerPrefsKeys.Boxes);
            boxes = JsonConvert.DeserializeObject<List<BoxData>>(boxesJson);
        }
        
        // Load carriers
        
        
    }


    public void UnlockItem(Item item)
    {
        if (!IsItemUnlocked(item))
        {
            UnlockedItem unlockedItem = new UnlockedItem(item, DateTime.UtcNow);
            items.Add(unlockedItem);
            // Save data if necessary
            SaveData();
        }
    }

    public bool IsItemUnlocked(Item item)
    {
        return items.Exists(unlockedItem => unlockedItem.item == item); 
    }
    
    private void OnApplicationQuit()
    {
        SaveData();
    }
}