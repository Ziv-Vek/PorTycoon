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
    //public List<BoxData> boxes = new List<BoxData>();
    //public List<BoxesCarrier> carriers;
    //[SerializeField] private Item[] items;
    public Dictionary<string, object> capturedState;

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

        //items = new List<UnlockedItem>();
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

        //Save saveableEntities
        if (capturedState == null)
        {
            capturedState = new Dictionary<string, object>();
        }
        
        foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
        {
            // TODO: add a situation where more then one ISaveable component is present on a GameObject
            if (capturedState.ContainsKey(saveableEntity.GetUID()))
            {
                capturedState[saveableEntity.GetUID()] = saveableEntity.GetComponent<ISaveable>().CaptureState();
            } 
            else
            {
                capturedState.Add(saveableEntity.GetUID(), saveableEntity.GetComponent<ISaveable>().CaptureState());
            }
        }
        
        //Dictionary<string, object> capturedState = CarriersManager.Instance.CaptureState();
        string stateJson = JsonConvert.SerializeObject(capturedState);
        PlayerPrefs.SetString(PlayerPrefsKeys.SaveableGameObjects, stateJson);
        
        PlayerPrefs.Save();
    }
    
    public void LoadData()
    {
        if (PlayerPrefs.HasKey((PlayerPrefsKeys.SaveableGameObjects)))
        {
            string capturedStateJson = PlayerPrefs.GetString(PlayerPrefsKeys.SaveableGameObjects);
            capturedState = JsonConvert.DeserializeObject<Dictionary<string, object>>(capturedStateJson);

            print(capturedStateJson);
            print(capturedState);

            foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                string uId = saveableEntity.GetUID();

                if (capturedState.ContainsKey(uId))
                {
                    //saveableEntity.GetComponent<ISaveable>().RestoreState(capturedState[uId]);
                    /*foreach (ISaveable saveableComponent in saveableEntity.GetComponents<ISaveable>())
                    {
                        saveableEntity.GetComponent<ISaveable>().RestoreState(carriersState[uId]);    
                    }*/
                }
            }
        }

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
            //boxes = JsonConvert.DeserializeObject<List<BoxData>>(boxesJson);
        }
        
        // Load carriers
    }

    /*public void UnlockItem(Item item)
    {
        if (!IsItemUnlocked(item))
        {
            UnlockedItem unlockedItem = new UnlockedItem(item, DateTime.UtcNow);
            items.Add(unlockedItem);
            // Save data if necessary
            SaveData();
        }
    }*/

    /*public bool IsItemUnlocked(Item item)
    {
        return items.Exists(unlockedItem => unlockedItem.item == item); 
    }*/
   
    private void OnApplicationQuit()
    {
        SaveData();
    }
}

