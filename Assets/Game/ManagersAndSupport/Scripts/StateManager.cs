using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private Dictionary<string, object> CapturedState { get; set; }


    public void SaveData(LevelData currentLevel, int money, int experience, List<UnlockedItem> items)
    {
        PlayerPrefs.SetString(PlayerPrefsKeys.CurrentLevel, currentLevel.id);
        PlayerPrefs.SetInt(PlayerPrefsKeys.Money, money);
        PlayerPrefs.SetInt(PlayerPrefsKeys.Experience, experience);

        // Save unlocked items
        string unlockedItemsJson = JsonConvert.SerializeObject(items);
        PlayerPrefs.SetString(PlayerPrefsKeys.UnlockedItems, unlockedItemsJson);

        //Save saveableEntities
        CapturedState ??= new Dictionary<string, object>();

        foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
        {
            // TODO: add a situation where more then one ISaveable component is present on a GameObject
            if (CapturedState.ContainsKey(saveableEntity.GetUID()))
            {
                CapturedState[saveableEntity.GetUID()] = saveableEntity.GetComponent<ISaveable>().CaptureState();
            }
            else
            {
                CapturedState.Add(saveableEntity.GetUID(), saveableEntity.GetComponent<ISaveable>().CaptureState());
            }
        }

        //Dictionary<string, object> capturedState = CarriersManager.Instance.CaptureState();
        string stateJson = JsonConvert.SerializeObject(CapturedState);
        PlayerPrefs.SetString(PlayerPrefsKeys.SaveableGameObjects, stateJson);

        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey((PlayerPrefsKeys.SaveableGameObjects)))
        {
            string capturedStateJson = PlayerPrefs.GetString(PlayerPrefsKeys.SaveableGameObjects);
            CapturedState = JsonConvert.DeserializeObject<Dictionary<string, object>>(capturedStateJson);

            foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                string uId = saveableEntity.GetUID();

                if (CapturedState.ContainsKey(uId))
                {
                    //saveableEntity.GetComponent<ISaveable>().RestoreState(capturedState[uId]);
                    /*foreach (ISaveable saveableComponent in saveableEntity.GetComponents<ISaveable>())
                    {
                        saveableEntity.GetComponent<ISaveable>().RestoreState(carriersState[uId]);    
                    }*/
                }
            }
        }

        // if (PlayerPrefs.HasKey(PlayerPrefsKeys.CurrentLevel))
        // {
        //     CurrentLevel = ConfigManager.GetLevelConfig(PlayerPrefs.GetString(PlayerPrefsKeys.CurrentLevel));
        // }
        //
        // if (PlayerPrefs.HasKey(PlayerPrefsKeys.Money))
        // {
        //     money = PlayerPrefs.GetInt(PlayerPrefsKeys.Money);
        // }
        //
        // if (PlayerPrefs.HasKey(PlayerPrefsKeys.Experience))
        // {
        //     experience = PlayerPrefs.GetInt(PlayerPrefsKeys.Experience);
        // }
        // // Load unlocked items
        //
        // if (PlayerPrefs.HasKey(PlayerPrefsKeys.UnlockedItems))
        // {
        //     string unlockedItemsJson = PlayerPrefs.GetString(PlayerPrefsKeys.UnlockedItems);
        //     items = JsonConvert.DeserializeObject<List<UnlockedItem>>(unlockedItemsJson);
        // }
        //
        // // Load boxes
        // if (PlayerPrefs.HasKey(PlayerPrefsKeys.Boxes))
        // {
        //     string boxesJson = PlayerPrefs.GetString(PlayerPrefsKeys.Boxes);
        //     //boxes = JsonConvert.DeserializeObject<List<BoxData>>(boxesJson);
        // }

        // Load carriers
    }
}